using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.System;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP20_AutomaticVM(IRecipeRepository recipeRepository, PlcConnection plcConnection, IOP10_TraceabilityRepository oP10_TraceabilityRepository, IServiceProvider serviceProvider, IOP20_TraceabilityRepository oP20_TraceabilityRepository) : BaseVM
    {

        #region General Properties

        public string? imagesFolder;

        private readonly PlcConnection _plcConnection = plcConnection;

        private readonly RecipeRepository _recipeRepository = (RecipeRepository)recipeRepository;

        private readonly OP10_TraceabilityRepository _op10TraceabilityRepository = (OP10_TraceabilityRepository)oP10_TraceabilityRepository;

        private CancellationTokenSource? _cancellationTokenSource;

        private bool _isScrapEnabled;

        public bool IsScrapEnabled
        {
            get { return _isScrapEnabled; }
            set
            {

                _isScrapEnabled = value;
                if (_isScrapEnabled)
                {
                    ScrapButtonText = "Gaiola Habilitada";
                    _ = _plcConnection.EnableScrapCage();
                }
                else
                {
                    if (VerifyDisableScrapCage())
                    {
                        ScrapButtonText = "Gaiola Desabilitada";
                        _ = _plcConnection.DisableScrapCage();
                    }
                    else
                    {
                        IsScrapEnabled = true;
                    }
                }
                OnPropertyChanged(nameof(IsScrapEnabled));
            }
        }

        public string _scrapButtonText = "Gaiola Desabilitada";

        public string ScrapButtonText
        {
            get { return _scrapButtonText; }
            set
            {
                _scrapButtonText = value;
                OnPropertyChanged(nameof(ScrapButtonText));
            }
        }

        //Observable collection de recipes para binding
        private ObservableCollection<Recipe>? _recipes;
        public ObservableCollection<Recipe>? Recipes
        {
            get => _recipes;
            set
            {
                _recipes = value;
                OnPropertyChanged(nameof(Recipes));
            }
        }

        //Selected recipe
        private Recipe? _selectedRecipe;
        public Recipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                _selectedRecipe = value;
                if (value != null)
                {
                    _ = _plcConnection.WriteOP20Recipe(value);
                }
                OnPropertyChanged(nameof(SelectedRecipe));
            }
        }

        private OP20_AutomaticCommomR _op20AutomaticCommomR = new();
        public OP20_AutomaticCommomR OP20AutomaticCommomR
        {
            get => _op20AutomaticCommomR;
            set
            {
                _op20AutomaticCommomR = value;
                OnPropertyChanged(nameof(OP20AutomaticCommomR));
            }
        }

        private readonly Dictionary<ushort, string[]> stepStringDict = new()
        {
            [0] = ["Posicione o módulo e acione o botão para iniciar", "modulo posicionado"],
            [5] = ["Reiniciando as variáveis"],
            [10] = ["Iniciando a leitura do código do radiador"],
            [15] = ["Lendo código do radiador", "codigo de barras 1"],
            [20] = ["Verificando código do radiador", "codigo de barras 1"],
            [25] = ["Iniciando teste", "teste condensador"],
            [30] = ["Avançando conjunto de vedação", "teste radiador"],
            [35] = ["Produto em teste", "teste radiador"],
            [40] = ["Recuando conjunto de vedação", "bocais recuados"],
            [45] = ["Retirar pinças, colocar os tampões do condensador e girar a mesa", "inserir tampas do condensador"],
            [46] = ["Aguardando giro da mesa", "rotacionando mesa"],
            [47] = ["Acione o botão para verificação das tampas do condensador", "tampas do condensador"],
            [55] = ["Verificando tampões", "tampas do condensador"],
            [56] = ["Ausência das tampas do condensador. Favor inserir e acionar o botão iniciar para nova verificação ou rearme para refugar a peça.", "ausencia tampas do condensador"],
            [57] = ["Cole a etiqueta e acione o botão para verificação do código", "colando etiqueta"],
            [65] = ["Lendo código de rastreabilidade", "codigo de barras 2"],
            [70] = ["Verificando código de rastreabilidade", "codigo de barras 2"],
            [71] = ["Código da etiqueta de rastreabilidade incompatível com a impressa, cole a correta e pressione início ou rearme para refugar a peça.", "etiqueta errada"],
            [75] = ["Aguardando novo módulo e giro da mesa", "modulo posicionado"],
            [100] = ["Peça ou etiqueta faltando", "etiqueta 1 faltando"],
            [101] = ["Operação cancelada"],
            [102] = ["Módulo com defeito", "produto com defeito"],
            [103] = ["Tampões do radiador ausentes", "etiqueta 2 faltando"],
            [104] = ["Falha na leitura da etiqueta de rastreabilidade", "etiqueta 2 faltando"],
            [1000] = ["Máquina em emergência", "emergencia"]
        };

        #endregion

        #region L1 and L2 Properties

        //Properties from L1 Plc
        private OP20_Automatic_Read _l1AutomaticRead = new();
        public OP20_Automatic_Read L1AutomaticRead
        {
            get => _l1AutomaticRead;
            set
            {
                _l1AutomaticRead = value;
                OnPropertyChanged(nameof(L1AutomaticRead));
            }
        }

        private OP20_Automatic_Read _l2AutomaticRead = new();
        public OP20_Automatic_Read L2AutomaticRead
        {
            get => _l2AutomaticRead;
            set
            {
                _l2AutomaticRead = value;
                OnPropertyChanged(nameof(L2AutomaticRead));
            }
        }

        private OP20_CurrentProduction _l1CurrentProduction = new();
        public OP20_CurrentProduction L1CurrentProduction
        {
            get => _l1CurrentProduction;
            set
            {
                _l1CurrentProduction = value;
                OnPropertyChanged(nameof(L1CurrentProduction));
            }
        }

        private OP20_CurrentProduction _l2CurrentProduction = new();
        public OP20_CurrentProduction L2CurrentProduction
        {
            get => _l2CurrentProduction;
            set
            {
                _l2CurrentProduction = value;
                OnPropertyChanged(nameof(L2CurrentProduction));
            }
        }

        #endregion

        // Method to monitor plc async
        private async Task MonitorPLCAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_plcConnection.Plc.IsConnected && SelectedRecipe != null)
                {
                    var l1Read = await _plcConnection.ReadOP20AutomaticL1();
                    var l2Read = await _plcConnection.ReadOP20AutomaticL2();
                    var commonRead = await _plcConnection.ReadOP20AutomaticCommon();


                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        L1AutomaticRead = l1Read!;
                        L2AutomaticRead = l2Read!;
                        OP20AutomaticCommomR = commonRead!;
                        await ProcessRead();
                    });
                }



                await Task.Delay(10, cancellationToken);
            }
        }

        private async Task ProcessRead()
        {
            ProcessScrapCage(OP20AutomaticCommomR);

            await ProcessStep(L1AutomaticRead, L1CurrentProduction, _plcConnection.SetL1RadiatorLabelOK, _plcConnection.SetL1RadiatorLabelNOK, _plcConnection.SetL1TraceabilityLabelOK, _plcConnection.SetL1TraceabilityLabelNOK);

            OnPropertyChanged(nameof(L1CurrentProduction));

            await ProcessStep(L2AutomaticRead, L2CurrentProduction, _plcConnection.SetL2RadiatorLabelOK, _plcConnection.SetL2RadiatorLabelNOK, _plcConnection.SetL2TraceabilityLabelOK, _plcConnection.SetL2TraceabilityLabelNOK);

            OnPropertyChanged(nameof(L2CurrentProduction));
        }

        private async Task ProcessStep(OP20_Automatic_Read automaticRead, OP20_CurrentProduction currentProduction, Func<Task> setRadiatorLabelOK, Func<Task> setRadiatorLabelNOK, Func<Task> setTraceabilityLabelOK, Func<Task> setTraceabilityLabelNOK)
        {
            string[] stepContent = stepStringDict.GetValueOrDefault(automaticRead.Step, Array.Empty<string>());
            if (stepContent.Length > 0)
            {
                currentProduction.StepText = stepContent[0];
                if (automaticRead.Step >= 100)
                {
                    currentProduction.ErrorState = 2;
                }
                else if (automaticRead.Step == 30 || automaticRead.Step == 40)
                {
                    currentProduction.ErrorState = 1;
                }
                else
                {
                    currentProduction.ErrorState = 0;
                }
            }

            if (stepContent.Length > 1)
            {
                string imageNameWithoutExtension = stepContent[1];
                string jpgPath = Path.Combine(imagesFolder, imageNameWithoutExtension + ".jpg");
                string pngPath = Path.Combine(imagesFolder, imageNameWithoutExtension + ".png");

                string imagePath = File.Exists(jpgPath) ? jpgPath : (File.Exists(pngPath) ? pngPath : null);

                if (imagePath != null)
                {
                    currentProduction.StepImage = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    currentProduction.StepImage = null;
                }
            }


            if (automaticRead.Step == 102 || automaticRead.Step == 104 || automaticRead.Step == 103)
            {
                currentProduction.StepText += ". Insira o produto na gaiola de refugo antes de prosseguir.";
            }


            if (automaticRead.Step == 0)
            {
                currentProduction = new OP20_CurrentProduction();
            }


            //The function verify if the step is 20 and if the code is in OP10 database, if not, the function will show a message to the user and wait for the user to allow the operation to continue


            if (automaticRead.Step == 20)
            {

                currentProduction.RadiatorCode = automaticRead.RadiatorLabel.Trim();

                currentProduction.OP10 = await _op10TraceabilityRepository.GetTraceabilityByRadiatorCode(currentProduction.RadiatorCode);


                if (currentProduction.OP10 != null && currentProduction.OP10.OP20_Executed == false)
                {
                    await setRadiatorLabelOK();
                }
                else
                {
                    AllowScreenVM allowScreenVM = (AllowScreenVM)serviceProvider.GetService(typeof(AllowScreenVM))!;
                    OP20_MainWindowVM mainVM = serviceProvider.GetRequiredService<OP20_MainWindowVM>();

                    if (currentProduction.OP10 == null)
                    {
                        allowScreenVM.Message = "Operação 10 ainda não realizada, favor retirar o produto ou solicitar a aprovação da liderança.";
                    }
                    else if (currentProduction.OP10.OP20_Executed == true)
                    {
                        allowScreenVM.Message = "Operação 20 já realizada, para repetição do teste favor solicitar aprovação da liderança";
                    }

                    AllowScreen allowScreen = new()
                    {
                        DataContext = allowScreenVM
                    };

                    var tsc = new TaskCompletionSource<bool>();

                    allowScreen.Closed += (s, e) => tsc.SetResult(true);

                    allowScreen.Show();

                    mainVM.ScreenControl = false;

                    await tsc.Task;

                    mainVM.ScreenControl = true;

                    if (allowScreenVM.IsAllowed)
                    {
                        await setRadiatorLabelOK();
                    }
                    else
                    {
                        await setRadiatorLabelNOK();
                    }

                }

            }

            if (automaticRead.Step == 47)
            {
                currentProduction.labelPrinted = false;
            }

            if (automaticRead.Step == 57 && !currentProduction.labelPrinted)
            {
                (currentProduction.LabelTraceabilityCode, currentProduction.ProductionDateTime) = LabelPrinter.PrintLabelAndReturnTraceabilityCode(SelectedRecipe!);

                if (SelectedRecipe!.VerifyRadiatorLabel && currentProduction.OP10 != null && SelectedRecipe!.ReadRadiatorLabelOP10)
                {
                    currentProduction.OP10!.OP20_Executed = true;
                    await _op10TraceabilityRepository.UpdateTraceability(currentProduction.OP10);
                }

                currentProduction.labelPrinted = true;
                await _plcConnection.SetLabelPrinted();
            }

            if (automaticRead.Step == 70)
            {
                currentProduction.TraceabilityCode = automaticRead.TraceabilityLabel.Trim();

                if (currentProduction.LabelTraceabilityCode != null && currentProduction.LabelTraceabilityCode.Trim().Contains(currentProduction.TraceabilityCode))
                {
                    await setTraceabilityLabelOK();
                    currentProduction.traceabilitySaved = false;
                }
                else
                {
                    await setTraceabilityLabelNOK();
                }
            }

            if (automaticRead.Step == 75 && !currentProduction.traceabilitySaved)
            {
                await SaveTraceability(automaticRead, currentProduction, SelectedRecipe!);
                currentProduction.traceabilitySaved = true;
            }

            //if(automaticRead.Step == 102 && !currentProduction.traceabilitySaved)
            //{
            //    await SaveTraceability(automaticRead, currentProduction, SelectedRecipe!, false);
            //    currentProduction.traceabilitySaved = true;
            //}



        }

        private void ProcessScrapCage(OP20_AutomaticCommomR oP20_AutomaticCommomR)
        {
            if (oP20_AutomaticCommomR.ScrapCageNOK)
            {
                if (Application.Current.Windows.OfType<AllowScreen>().FirstOrDefault() == null)
                {
                    AllowScreenVM screenVM = (AllowScreenVM)serviceProvider.GetService(typeof(AllowScreenVM))!;
                    screenVM.Message = "Produto faltando na gaiola de refugos.";
                    screenVM.QuestionVisibility = Visibility.Hidden;

                    AllowScreen allowScreen = new()
                    {
                        DataContext = screenVM,
                    };

                    allowScreen.Title = "ScrapCageNOK";

                    allowScreen.Show();
                }
            }
            
            if (oP20_AutomaticCommomR.ScrapCageFull)
            {
                if (Application.Current.Windows.OfType<AllowScreen>().FirstOrDefault() == null)
                {
                    AllowScreenVM screenVM = (AllowScreenVM)serviceProvider.GetService(typeof(AllowScreenVM))!;
                    screenVM.Message = "A gaiola de refugos está cheia, esvazie antes de continuar.";
                    screenVM.QuestionVisibility = Visibility.Hidden;

                    AllowScreen allowScreen = new()
                    {
                        DataContext = screenVM,
                    };

                    allowScreen.Title = "ScrapCageFull";

                    allowScreen.Show();
                }
            }
            
            if (!OP20AutomaticCommomR.ScrapCageNOK)
            {
                var screens = Application.Current.Windows.OfType<AllowScreen>();

                foreach (Window screen in screens)
                {
                    if (screen.Title == "ScrapCageNOK")
                        screen.Close();
                }

            }
           
            if (!oP20_AutomaticCommomR.ScrapCageFull)
            {
                var screens = Application.Current.Windows.OfType<AllowScreen>();

                foreach (Window screen in screens)
                {
                    if (screen.Title == "ScrapCageFull")
                        screen.Close();
                }
            }

        }

        // Method to load recipes async
        public async void Start()
        {
            Recipes = await _recipeRepository.GetAllRecipes();
            _cancellationTokenSource = new CancellationTokenSource();

            if (SelectedRecipe != null)
            {
                _ = _plcConnection.WriteOP20Recipe(SelectedRecipe);
            }

            _ = Task.Run(() => MonitorPLCAsync(_cancellationTokenSource.Token));
            if (SelectedRecipe != null)
            {
                await _plcConnection.ActivateOP20Automatic();
            }

            IsScrapEnabled = await _plcConnection.ReadScrapCageStatus();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            imagesFolder = Path.Combine(appDataPath, "Supervisorio Banco Renault", "ImagesOP20");

        }

        public async void Stop()
        {
            _cancellationTokenSource?.Cancel();

            await _plcConnection.DeactivateOP20Automatic();
        }

        internal async void ResetScrapCage()
        {

            OP20_MainWindowVM mainVM = serviceProvider.GetRequiredService<OP20_MainWindowVM>();

            AllowScreenVM allowScreenVM = (AllowScreenVM)serviceProvider.GetService(typeof(AllowScreenVM))!;
            allowScreenVM.Message = "É necessário a aprovação da liderança para prosseguir. Favor fazer a leitura do crachá.";

            AllowScreen allowScreen = new()
            {
                DataContext = allowScreenVM
            };

            var tsc = new TaskCompletionSource<bool>();

            allowScreen.Closed += (s, e) => tsc.SetResult(true);

            allowScreen.Show();

            mainVM.ScreenControl = false;

            await tsc.Task;

            mainVM.ScreenControl = true;

            if (allowScreenVM.IsAllowed)
            {
                await _plcConnection.ResetScrapCage();
            }

            OP20_MainWindowVM vm = serviceProvider.GetRequiredService<OP20_MainWindowVM>();
            vm.ChangePage("OP20_Automatic");
        }

        internal async void ResetProductsCount()
        {
            await _plcConnection.ResetOP20ProductsCount();
        }

        private async Task SaveTraceability(OP20_Automatic_Read oP20_Automatic_Read, OP20_CurrentProduction currentProduction, Recipe recipe)
        {

            OP20_MainWindowVM mainVM = serviceProvider.GetRequiredService<OP20_MainWindowVM>();

            OP20_TraceabilityModel oP20_Traceability = new()
            {
                CondenserVerified = recipe.VerifyCondenser,
                ModuleCode = recipe.ModuleCode!,
                ClientCode = recipe.ClientCode!,
                CondenserCoversVerified = recipe.VerifyCondenserCovers,
                RadiatorVerified = recipe.VerifyRadiator,
                RadiatorCodeVerified = recipe.VerifyRadiatorLabel,
                TraceabilityCodeVerified = recipe.VerifyTraceabilityLabel,
                DateTimeOP20 = currentProduction.ProductionDateTime,
                FinalCondenserPressure = (float)Math.Round(oP20_Automatic_Read.CondenserAteqPressure, 2),
                FinalCondenserLeak = oP20_Automatic_Read.CondenserAteqLeak,
                FinalCondenserPSRead = (float)Math.Round(oP20_Automatic_Read.CondenserPS, 2),
                FinalRadiatorPressure = (float)Math.Round(oP20_Automatic_Read.RadiatorAteqPressure, 2),
                FinalRadiatorLeak = oP20_Automatic_Read.RadiatorAteqLeak,
                FinalRadiatorPSRead = (float)Math.Round(oP20_Automatic_Read.RadiatorPS, 2),
                RadiatorCode = currentProduction.RadiatorCode,
                TraceabilityCode = currentProduction.LabelTraceabilityCode.Trim(),
                UserName = mainVM.LoggedUser.Name!
            };

            await oP20_TraceabilityRepository.AddTraceability(oP20_Traceability);
        }

        private bool VerifyDisableScrapCage()
        {
            AllowScreenVM allowScreenVM = (AllowScreenVM)serviceProvider.GetService(typeof(AllowScreenVM))!;
            OP20_MainWindowVM mainVM = serviceProvider.GetRequiredService<OP20_MainWindowVM>();

            allowScreenVM.Message = "Para desabilitar a gaiola de refugos é necessário a aprovação da liderança.";

            AllowScreen allowScreen = new()
            {
                DataContext = allowScreenVM
            };

            mainVM.ScreenControl = false;

            allowScreen.ShowDialog();

            mainVM.ScreenControl = true;

            return allowScreenVM.IsAllowed;
        }

    }
}

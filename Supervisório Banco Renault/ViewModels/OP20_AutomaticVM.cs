using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP20_AutomaticVM(IRecipeRepository recipeRepository, PlcConnection plcConnection, IOP10_TraceabilityRepository oP10_TraceabilityRepository, IServiceProvider serviceProvider, IOP20_TraceabilityRepository oP20_TraceabilityRepository) : BaseVM
    {

        #region General Properties

        private readonly PlcConnection _plcConnection = plcConnection;

        private readonly RecipeRepository _recipeRepository = (RecipeRepository)recipeRepository;

        private readonly OP10_TraceabilityRepository _op10TraceabilityRepository = (OP10_TraceabilityRepository)oP10_TraceabilityRepository;

        private CancellationTokenSource? _cancellationTokenSource;


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

        private readonly Dictionary<ushort, string> stepStringDict = new()
        {
            [0] = "Acione o botão para iniciar",
            [5] = "Reiniciando as variáveis",
            [10] = "Iniciando a leitura do código do radiador",
            [15] = "Lendo código do radiador",
            [20] = "Verificando código do radiador",
            [25] = "Iniciando teste do radiador",
            [30] = "Iniciando teste do condensador",
            [35] = "Produto em teste",
            [40] = "Recuando conjunto de vedação",
            [45] = "Aguardando giro da mesa",
            [46] = "Aguardando giro da mesa",
            [47] = "Acione o botão para verificação das tampas do condensador",
            [55] = "Verificando tampões",
            [56] = "Cole a etiqueta e acione o botão para verificação do código",
            [65] = "Lendo código de rastreabilidade",
            [70] = "Verificando código de rastreabilidade",
            [71] = "Código da etiqueta de rastreabilidade incompatível com a impressa, cole a correta e pressione start",
            [75] = "Aguardando novo módulo e giro da mesa",
            [100] = "Falha na leitura da etiqueta do módulo",
            [101] = "Módulo com pendência na operação 10",
            [102] = "Módulo com defeito",
            [103] = "Ausência das tampas do condensador",
            [104] = "Falha na leitura da etiqueta de rastreabilidade",
            [1000] = "Máquina em emergência"
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



                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        L1AutomaticRead = l1Read;
                        L2AutomaticRead = l2Read;
                        OP20AutomaticCommomR = commonRead;
                        ProcessRead();
                    });
                }


                await Task.Delay(100, cancellationToken);
            }
        }

        private async void ProcessRead()
        {
           await ProcessStep(L1AutomaticRead, L1CurrentProduction, _plcConnection.SetL1RadiatorLabelOK, _plcConnection.SetL1RadiatorLabelNOK, _plcConnection.SetL1TraceabilityLabelOK, _plcConnection.SetL1TraceabilityLabelNOK);

            OnPropertyChanged(nameof(L1CurrentProduction));

           await ProcessStep(L2AutomaticRead, L2CurrentProduction, _plcConnection.SetL2RadiatorLabelOK, _plcConnection.SetL2RadiatorLabelNOK, _plcConnection.SetL2TraceabilityLabelOK, _plcConnection.SetL2TraceabilityLabelNOK);

            OnPropertyChanged(nameof(L2CurrentProduction));
        }

        private async Task ProcessStep(OP20_Automatic_Read automaticRead, OP20_CurrentProduction currentProduction, Func<Task> setRadiatorLabelOK, Func<Task> setRadiatorLabelNOK, Func<Task> setTraceabilityLabelOK, Func<Task> setTraceabilityLabelNOK)
        {
            currentProduction.StepText = stepStringDict.GetValueOrDefault(automaticRead.Step, "");
            currentProduction.Error = automaticRead.Step >= 100;

            if (automaticRead.Step >= 100 && automaticRead.Step < 1000)
            {
                currentProduction.StepText += ". Insira o produto na gaiola de refugo antes de prosseguir.";
            }


            if(automaticRead.Step == 0)
            {
                currentProduction = new OP20_CurrentProduction();
            }

            //The function verify if the step is 20 and if the code is in OP10 database, if not, the function will show a message to the user and wait for the user to allow the operation to continue

            if (automaticRead.Step == 20)
            {
                currentProduction.RadiatorCode = automaticRead.RadiatorLabel.Substring(SelectedRecipe.InitialCharacter - 1, SelectedRecipe.CodeLength);

                OP10_Traceability? op10Data = await _op10TraceabilityRepository.GetTraceabilityByRadiatorCode(currentProduction.RadiatorCode);

                if (op10Data != null && op10Data.OP20_Executed == false)
                {
                    op10Data.OP20_Executed = true;
                    await setRadiatorLabelOK();
                    await _op10TraceabilityRepository.UpdateTraceability(op10Data);
                }
                else
                {
                    AllowScreenVM allowScreenVM = (AllowScreenVM)serviceProvider.GetService(typeof(AllowScreenVM))!;
                    OP20_MainWindowVM mainVM = serviceProvider.GetRequiredService<OP20_MainWindowVM>();

                    if (op10Data == null)
                    {
                        allowScreenVM.Message = "Operação 10 ainda não realizada, favor retirar o produto ou solicitar a aprovação da liderança.";
                    }
                    else if (op10Data.OP20_Executed == true)
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

            if (automaticRead.Step == 56)
            {
                currentProduction.VerifyTraceabilityCode = LabelPrinter.PrintLabelAndReturnTraceabilityCode(SelectedRecipe);
            }

            if (automaticRead.Step == 70)
            {
                currentProduction.TraceabilityCode = automaticRead.TraceabilityLabel;

                if (currentProduction.VerifyTraceabilityCode.Contains(automaticRead.TraceabilityLabel))
                {
                    await setTraceabilityLabelOK();
                    await SaveTraceability(automaticRead, currentProduction, SelectedRecipe);
                }
                else
                {
                    await setTraceabilityLabelNOK();
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
        }

        internal async void ResetProductsCount()
        {
            await _plcConnection.ResetOP20ProductsCount();
        }

        private async Task SaveTraceability(OP20_Automatic_Read oP20_Automatic_Read, OP20_CurrentProduction currentProduction, Recipe recipe)
        {

            OP20_MainWindowVM mainVM = serviceProvider.GetRequiredService<OP20_MainWindowVM>();

            OP20_Traceability oP20_Traceability = new()
            {
                CondenserVerified = recipe.VerifyCondenser,
                CondenserCoversVerified = recipe.VerifyCondenserCovers,
                RadiatorVerified = recipe.VerifyRadiator,
                RadiatorCodeVerified = recipe.VerifyRadiatorLabel,
                TraceabilityCodeVerified = recipe.VerifyTraceabilityLabel,
                DateTimeOP20 = DateTime.Now,
                FinalCondenserPressure = oP20_Automatic_Read.CondenserAteqPressure,
                FinalCondenserLeak = oP20_Automatic_Read.CondenserAteqLeak,
                FinalCondenserPSRead = oP20_Automatic_Read.CondenserPS,
                FinalRadiatorPressure = oP20_Automatic_Read.RadiatorAteqPressure,
                FinalRadiatorLeak = oP20_Automatic_Read.RadiatorAteqLeak,
                FinalRadiatorPSRead = oP20_Automatic_Read.RadiatorPS,
                RadiatorCode = currentProduction.RadiatorCode,
                TraceabilityCode = currentProduction.TraceabilityCode,
                User = mainVM.LoggedUser
            };

            await oP20_TraceabilityRepository.AddTraceability(oP20_Traceability);

        }

    }
}

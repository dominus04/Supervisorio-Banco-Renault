using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP20_AutomaticVM : BaseVM
    {
        #region Properties

        private PlcConnection _plcConnection;

        private RecipeRepository _recipeRepository;

        private OP10_TraceabilityRepository _op10TraceabilityRepository;

        private CancellationTokenSource? _cancellationTokenSource;

        private Dictionary<ushort, string> stepStringDict = new()
        {
            [0] = "Aguardando botão para iniciar",
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
            [47] = "Aguardando botão para verificação das tampas do condensador",
            [55] = "Verificando tampões",
            [60] = "Aguardando botão para verificação da etiqueta de rastreabilidade",
            [65] = "Lendo código de rastreabilidade",
            [70] = "Verificando código de rastreabilidade",
            [75] = "Aguardando novo módulo e giro da mesa",
            [100] = "Falha na leitura da etiqueta do módulo",
            [101] = "Módulo com pendência na operação 10",
            [102] = "Módulo com defeito",
            [103] = "Ausência das tampas do condensador",
            [104] = "Falha na leitura da etiqueta de rastreabilidade",
            [105] = "Código da etiqueta de rastreabilidade incompatível com a impressa",
            [1000] = "Máquina em emergência"
        };

        private bool _l1Error;
        public bool L1Error
        {
            get => _l1Error;
            set
            {
                _l1Error = value;
                OnPropertyChanged(nameof(L1Error));
            }
        }

        private bool _l2Error;
        public bool L2Error
        {
            get => _l2Error;
            set
            {
                _l2Error = value;
                OnPropertyChanged(nameof(L2Error));
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
                    LabelPrinter.PrintLabel(value);
                }
                OnPropertyChanged(nameof(SelectedRecipe));
            }
        }

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

        //Properties from L2 Plc
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

        //Propertie to set text of step L1
        private string _l1Text = "";
        public string L1Text
        {
            get => _l1Text;
            set
            {
                _l1Text = value;
                OnPropertyChanged(nameof(L1Text));
            }
        }

        //Propertie to set text of step L2
        private string _l2Text = "";
        public string L2Text
        {
            get => _l1Text;
            set
            {
                _l1Text = value;
                OnPropertyChanged(nameof(L2Text));
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


        #endregion

        public OP20_AutomaticVM(IRecipeRepository recipeRepository, PlcConnection plcConnection, IOP10_TraceabilityRepository oP10_TraceabilityRepository)
        {
            _recipeRepository = (RecipeRepository)recipeRepository;
            _plcConnection = plcConnection;
            _op10TraceabilityRepository = (OP10_TraceabilityRepository)oP10_TraceabilityRepository;

            //Initialize recipe list
        }

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
            (L1Text, L1Error) = await ProcessStep(L1AutomaticRead, _plcConnection.SetL1RadiatorLabelOK, _plcConnection.SetL1RadiatorLabelNOK);
            (L2Text, L2Error) = await ProcessStep(L2AutomaticRead, _plcConnection.SetL2RadiatorLabelOK, _plcConnection.SetL2RadiatorLabelNOK);
        }

        private async Task<(string, bool)> ProcessStep(OP20_Automatic_Read automaticRead, Func<Task> setLabelOK, Func<Task> setLabelNOK)
        {
            var stepText = stepStringDict.GetValueOrDefault(automaticRead.Step, "");
            var error = automaticRead.Step >= 100;
            if (automaticRead.Step >= 100 && automaticRead.Step < 1000)
            {
                stepText += ". Insira o produto na gaiola de refugo antes de prosseguir.";
            }

            if(automaticRead.Step == 20)
            {
                var radiatorCode = automaticRead.RadiatorLabel.Substring(SelectedRecipe.InitialCharacter - 1, SelectedRecipe.CodeLength);
                OP10_Traceability op10Data = await _op10TraceabilityRepository.GetTraceabilityByRadiatorCode(radiatorCode);
                if (op10Data != null)
                {
                    op10Data.OP20_Executed = true;
                    await setLabelOK();
                    await _op10TraceabilityRepository.UpdateTraceability(op10Data);
                }
                else
                {
                    await setLabelNOK();
                }

            }

            return (stepText, error);

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
            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();

            await _plcConnection.DeactivateOP20Automatic();
        }

        internal async void ResetScrapCage()
        {
            await _plcConnection.ResetScrapCage();
        }

        internal async void ResetProductsCount()
        {
            await _plcConnection.ResetOP20ProductsCount();
        }
    }
}

using S7.Net;
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

        private CancellationTokenSource? _cancellationTokenSource;

        private Dictionary<ushort, string> stepStringDict = new()
        {
            [0] = "Aguardando botão para iniciar",
            [5] = "Reiniciando as variáveis",
            [10] = "Iniciando a leitura do código do módulo",
            [15] = "Lendo código do módulo",
            [20] = "Verificando código do módulo",
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

        //Observable collection de recipes para binding
        private ObservableCollection<Recipe>? _recipes;
        public ObservableCollection<Recipe>? Recipes
        {
            get { return _recipes; }
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
            get { return _selectedRecipe; }
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

        //Properties from L1 Plc
        private OP20_Automatic_Read _l1AutomaticRead = new();
        public OP20_Automatic_Read L1AutomaticRead 
        {
            get { return _l1AutomaticRead; }
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
            get { return _l2AutomaticRead; }
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
            get { return _l1Text; }
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
            get { return _l1Text; }
            set
            {
                _l1Text = value;
                OnPropertyChanged(nameof(L2Text));
            }
        }

        private OP20_AutomaticCommomR _op20AutomaticCommomR = new();
        public OP20_AutomaticCommomR OP20AutomaticCommomR
        {
            get { return _op20AutomaticCommomR; }
            set
            {
                _op20AutomaticCommomR = value;
                OnPropertyChanged(nameof(OP20AutomaticCommomR));
            }
        }


        #endregion

        public OP20_AutomaticVM(IRecipeRepository recipeRepository, PlcConnection plcConnection)
        {
            _recipeRepository = (RecipeRepository)recipeRepository;
            _plcConnection = plcConnection;

            //Initialize recipe list
        }

        // Method to monitor plc async
        private async Task MonitorPLCAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if(_plcConnection.Plc.IsConnected)
                {
                    var l1Read = await _plcConnection.ReadOP20AutomaticL1();
                    var l2Read = await _plcConnection.ReadOP20AutomaticL2();
                    var commonRead = await _plcConnection.ReadOP20AutomaticCommon();
 
                

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        L1AutomaticRead = l1Read;
                        L2AutomaticRead = l2Read;
                        OP20AutomaticCommomR = commonRead;
                        UpdateUI();
                    });
                }


                await Task.Delay(100, cancellationToken);
            }
        }


        private void UpdateUI()
        {
            L1Text = stepStringDict.GetValueOrDefault(L1AutomaticRead.Step, "");
            L2Text = stepStringDict.GetValueOrDefault(L2AutomaticRead.Step, "");
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
               await  _plcConnection.ActivateOP20Automatic();
            }
        }

        public async void Stop()
        {
            if(_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();

            await _plcConnection.DeactivateOP20Automatic();
        }

        internal async void ResetScrapCage()
        {
            await _plcConnection.ResetScrapCage();
        }
    }
}

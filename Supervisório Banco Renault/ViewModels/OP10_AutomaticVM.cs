using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP10_AutomaticVM : BaseVM
    {

        #region Properties

        private PlcConnection _plcConnection;

        private RecipeRepository _recipeRepository;

        private CancellationTokenSource? _cancellationTokenSource;

        private OP10_MainWindowVM _oP10_MainWindowVM;

        private OP10_TraceabilityRepository _op10_TraceabilityRepository;

        private OP10_Automatic_Read _op10AutomaticRead;
        public OP10_Automatic_Read OP10AutomaticRead
        {
            get => _op10AutomaticRead;
            set
            {
                _op10AutomaticRead = value;
                OnPropertyChanged(nameof(OP10AutomaticRead));
            }
        }

        //Error property
        private bool _oP10_Error;
        public bool OP10_Error
        {
            get => _oP10_Error;
            set
            {
                _oP10_Error = value;
                OnPropertyChanged(nameof(OP10_Error));
            }
        }

        //Step text property
        private string stepText = "";
        public string StepText
        {
            get => stepText;
            set
            {
                stepText = value;
                OnPropertyChanged(nameof(StepText));
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
                    _ = _plcConnection.ActivateOP10Automatic();
                }
                OnPropertyChanged(nameof(SelectedRecipe));
            }
        }

        private Dictionary<ushort, string> stepStringDict = new()
        {
            [0] = "Coloque o radiador e avance alavanca para travar",
            [5] = "Indexando produto",
            [10] = "Reiniciando variáveis",
            [15] = "Iniciando leitura do código do radiador",
            [20] = "Lendo código do radiador",
            [25] = "Rotacione o produto para a posição de montagem do condensador",
            [30] = "Pegue o parafuso na caixa",
            [35] = "Parafuse o condensador no módulo",
            [40] = "Lendo código do condensador",
            [45] = "Rotacione o produto para a posição inicial",
            [50] = "Gravando informações no banco de dados",
            [55] = "Recue a alavanca e retire o módulo",
            [100] = "Falha na leitura da etiqueta do radiador",
            [101] = "Falha na leitura da etiqueta do condensador",
            [1000] = "Máquina em emergência"
        };

        #endregion

        public OP10_AutomaticVM(PlcConnection plcConnection, IRecipeRepository recipeRepository, OP10_MainWindowVM oP10_MainWindowVM, IOP10_TraceabilityRepository oP10_TraceabilityRepository)
        {
            _plcConnection = plcConnection;
            _recipeRepository = (RecipeRepository)recipeRepository;
            _oP10_MainWindowVM = oP10_MainWindowVM;
            _op10_TraceabilityRepository = (OP10_TraceabilityRepository)oP10_TraceabilityRepository;
        }

        private async Task MonitorPLCAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_plcConnection.Plc.IsConnected)
                {

                    var op10Read = await _plcConnection.ReadOP10Automatic();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        OP10AutomaticRead = op10Read;
                        ProcessRead();
                    });
                }


                await Task.Delay(100, cancellationToken);
            }
        }

        private async void ProcessRead()
        {
            StepText = stepStringDict.GetValueOrDefault(OP10AutomaticRead.Step, "");
            OP10_Error = OP10AutomaticRead.Step >= 100;
            if(OP10AutomaticRead.Step == 50)
            {
                var op10_Data = new OP10_Traceability 
                {
                    RadiatorCode = OP10AutomaticRead.RadiatorLabel.Substring(SelectedRecipe.InitialCharacter - 1, SelectedRecipe.CodeLength),
                    CondenserCode = OP10AutomaticRead.CondenserLabel,
                    UserId = _oP10_MainWindowVM.LoggedUser.Id
                };
                if( await _op10_TraceabilityRepository.AddTraceability(op10_Data))
                {
                    await _plcConnection.SetOP10DataSaved();
                }
            }
        }

        // Method to load recipes async
        public async void Start()
        {
            Recipes = await _recipeRepository.GetAllRecipes();
            _cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Run(() => MonitorPLCAsync(_cancellationTokenSource.Token));

            if (SelectedRecipe != null)
            {
                await _plcConnection.ActivateOP10Automatic();
            }
        }

        public async void EndCycle()
        {
            await _plcConnection.EndCycle();
        }

        public async void Stop()
        {
            await _plcConnection.DeactivateOP10Automatic();
        }
    }
}

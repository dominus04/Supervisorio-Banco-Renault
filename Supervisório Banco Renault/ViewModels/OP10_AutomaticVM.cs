using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP10_AutomaticVM : BaseVM
    {

        #region Properties

        private bool traceabilitySaved = false;

        public string? imagesFolder;

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

        private BitmapImage _stepImage;
        public BitmapImage StepImage 
        {
            get { return _stepImage; }
            set 
            {
                _stepImage = value;
                OnPropertyChanged(nameof(StepImage));
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
                    _ = _plcConnection.WriteOP10Recipe(value);
                }
                OnPropertyChanged(nameof(SelectedRecipe));
            }
        }

        private Dictionary<ushort, string[]> stepStringDict = new()
        {
            [0] = ["Coloque o radiador e avance alavanca para travar", "radiador posicionado 1"],
            [5] = ["Indexando produto"],
            [10] = ["Reiniciando variáveis"],
            [15] = ["Lendo etiqueta do radiador", "etiqueta radiador"],
            [20] = ["Lendo etiqueta do radiador", "etiqueta radiador"],
            [23] = ["Verificando etiqueta do radiador", "etiqueta radiador"],
            [25] = ["Rotacione o produto para a posição de montagem do condensador", "radiador posicionado 2"],
            [30] = ["Monte o condensador e pegue o parafuso na caixa", "caixa parafusos aberta"],
            [35] = ["Parafuse o condensador no módulo", "parafusamento"],
            [40] = ["Lendo código do condensador", "etiqueta condensador"],
            [45] = ["Rotacione o produto para a posição inicial", "rotacionando produto"],
            [50] = ["Gravando informações no banco de dados"],
            [55] = ["Recue a alavanca e retire o módulo", "alavanca"],
            [100] = ["Falha na leitura da etiqueta do radiador", "etiqueta 1 faltando"],
            [101] = ["Falha na leitura da etiqueta do condensador", "etiqueta 2 faltando"],
            [102] = ["O radiador não é compatível com a receita escolhida, cancele o ciclo e reinicie com o produto correto", "etiqueta 2 faltando"],
            [1000] = ["Máquina em emergência", "emergencia"]
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

            if(OP10AutomaticRead.Step == 0)
                traceabilitySaved = false;

            string[] stepContent = stepStringDict.GetValueOrDefault(OP10AutomaticRead.Step, Array.Empty<string>());
            if (stepContent.Length > 0)
            {
                StepText = stepContent[0];
                OP10_Error = OP10AutomaticRead.Step >= 100;
            }
            
            if(stepContent.Length > 1)
            {

                if (stepContent.Length > 1)
                {
                    string imageNameWithoutExtension = stepContent[1];
                    string jpgPath = Path.Combine(imagesFolder, imageNameWithoutExtension + ".jpg");
                    string pngPath = Path.Combine(imagesFolder, imageNameWithoutExtension + ".png");

                    string imagePath = File.Exists(jpgPath) ? jpgPath : (File.Exists(pngPath) ? pngPath : null);

                    if (imagePath != null)
                    {
                        StepImage = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        StepImage = null;
                    }
                }
            }

            if(OP10AutomaticRead.Step == 23)
            {
                if (OP10AutomaticRead.RadiatorLabel.Trim().StartsWith(SelectedRecipe!.RadiatorModel.Trim()))
                {
                    await _plcConnection.SetOP10RadiatorLabelOK();
                }
                else
                {
                    await _plcConnection.SetOP10RadiatorLabelNOK();
                }
            }

            if(OP10AutomaticRead.Step == 50 && !traceabilitySaved)
            {

                if (SelectedRecipe!.ReadRadiatorLabelOP10)
                {
                    var op10_Data = new OP10_TraceabilityModel
                    {
                        //RadiatorCode = OP10AutomaticRead.RadiatorLabel.Substring(SelectedRecipe.InitialCharacter - 1, SelectedRecipe.CodeLength),
                        RadiatorCode = OP10AutomaticRead.RadiatorLabel.Trim(),
                        CondenserCode = OP10AutomaticRead.CondenserLabel.Trim(),
                        UserName = _oP10_MainWindowVM.LoggedUser.Name!
                    };


                    if (await _op10_TraceabilityRepository.AddTraceability(op10_Data))
                    {
                        await _plcConnection.SetOP10DataSaved();
                        traceabilitySaved = true;
                    }
                }
                else
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

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            imagesFolder = Path.Combine(appDataPath, "Supervisorio Banco Renault", "ImagesOP10");
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

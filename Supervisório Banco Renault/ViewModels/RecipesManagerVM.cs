using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class RecipesManagerVM : BaseVM
    {

        #region Properties

        private RecipeRepository _recipeRepository;

        private readonly IServiceProvider _serviceProvider;

        private LabelRepository _labelRepository;

        private ObservableCollection<Recipe> _recipes = [];
        public ObservableCollection<Recipe> Recipes
        {
            get => _recipes;
            set
            {
                _recipes = value;
                OnPropertyChanged(nameof(Recipes));
            }
        }

        private Recipe? _selectedRecipe;
        public Recipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                _selectedRecipe = value;
                OnPropertyChanged(nameof(SelectedRecipe));
            }
        }

        #endregion

        public RecipesManagerVM(IRecipeRepository recipeRepository, ILabelRepository labelRepository, IServiceProvider serviceProvider)
        {
            _recipeRepository = (RecipeRepository)recipeRepository;
            _labelRepository = (LabelRepository)labelRepository;
            _serviceProvider = serviceProvider;

            LoadRecipes();
        }

        public void AddOrUpdateRecipe(Type t, bool isUpdate)
        {
            Application.Current.Windows.OfType<RecipeEdit>().FirstOrDefault()?.Close();

            RecipeEditVM vm = new(_recipeRepository, _labelRepository);
            WindowBaseVM mainVM = (WindowBaseVM)_serviceProvider.GetService(typeof(OP20_MainWindowVM))!;

            if (isUpdate && SelectedRecipe != null)
            {
                vm = new(_recipeRepository, SelectedRecipe, _labelRepository);
            }
            else if (isUpdate && SelectedRecipe == null)
            {
                MessageBox.Show("Selecione uma receita para editar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RecipeEdit recipeEdit = new()
            {
                DataContext = vm,
                Top = 140
            };

            recipeEdit.Left = (1920 - recipeEdit.Width) / 2;

            if (t == typeof(OP10_MainWindow))
            {
                recipeEdit.Left += 1920;
                mainVM = (WindowBaseVM)_serviceProvider.GetService(typeof(OP10_MainWindowVM))!;
            }

            mainVM.ScreenControl = false;

            recipeEdit.Show();

            recipeEdit.Closed += (sender, e) =>
            {
                LoadRecipes();
                mainVM.ScreenControl = true;
            };
        }

        public async void LoadRecipes()
        {
            Recipes = await _recipeRepository.GetAllRecipes();
        }

        public async void RemoveRecipe()
        {
            if (SelectedRecipe != null)
            {
                try
                {
                    await _recipeRepository.RemoveRecipe(SelectedRecipe);
                    LoadRecipes();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Erro ao remover receita: " + e.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione uma receita para remover", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class RecipesVM : BaseVM
    {

        public RecipeRepository _recipeRepository;

        public RecipesVM(IRecipeRepository recipeRepository)
        {
            _recipeRepository = (RecipeRepository)recipeRepository;

            LoadRecipes();
        }

        public void AddOrUpdateRecipe(Type t, bool isUpdate)
        {
            Application.Current.Windows.OfType<RecipeEdit>().FirstOrDefault()?.Close();

            RecipeEditVM vm = new(_recipeRepository);

            if (isUpdate && SelectedRecipe != null)
            {
                vm = new(_recipeRepository, SelectedRecipe);
            }
            else if (isUpdate && SelectedRecipe == null)
            {
                MessageBox.Show("Selecione uma receita para editar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RecipeEdit recipeEdit = new();
            recipeEdit.DataContext = vm;
            recipeEdit.Top = 140;

            //if (t == typeof(OP20_MainWindow))
            //    recipeEdit.Left = ((1920 - recipeEdit.Width) / 2) - 1920;
            //else
            //    recipeEdit.Left = (1920 - recipeEdit.Width) / 2;

            recipeEdit.Show();
          
            recipeEdit.Closed += (sender, e) =>
            {
                LoadRecipes();
            };
        }

        private void ParentWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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

    }
}

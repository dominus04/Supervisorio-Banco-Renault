using S7.Net;
using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Services;
using System.Collections.ObjectModel;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP20_AutomaticVM : BaseVM
    {

        private PlcConnection _plcConnection;

        private RecipeRepository _recipeRepository;

        public OP20_AutomaticVM(IRecipeRepository recipeRepository, PlcConnection plcConnection)
        {
            _recipeRepository = (RecipeRepository)recipeRepository;
            _plcConnection = plcConnection;

            //Initialize recipe list
            LoadRecipes();

        }

        // Method to load recipes async
        public async void LoadRecipes()
        {
            Recipes = await _recipeRepository.GetAllRecipes();
        }

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

        private Recipe? _selectedRecipe;
        public Recipe? SelectedRecipe
        {
            get { return _selectedRecipe; }
            set
            {
                _selectedRecipe = value;
                _plcConnection.WriteOP20Recipe(SelectedRecipe);
                OnPropertyChanged(nameof(SelectedRecipe));
            }
        }

    }
}

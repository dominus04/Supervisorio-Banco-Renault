using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class RecipeEditVM : BaseVM
    {
        private bool isUpdate = false;

        private readonly IRecipeRepository _recipeRepository;

        private Recipe? _recipe;

        public Recipe? Recipe
        {
            get => _recipe;
            set
            {
                _recipe = value;
                OnPropertyChanged(nameof(Recipe));
            }
        }

        public RecipeEditVM(IRecipeRepository recipeRepository, Recipe recipe)
        {
            _recipeRepository = recipeRepository;
            Recipe = recipe;
            isUpdate = true;
        }

        public RecipeEditVM(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
            Recipe = new();
        }

        public async Task<bool> AddOrUpdateRecipe()
        {
            if (Recipe == null)
                return false;


            try
            {
                if (isUpdate)
                {
                    await _recipeRepository.UpdateRecipe(Recipe);
                }
                else
                {
                    await _recipeRepository.AddRecipe(Recipe);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


    }
}

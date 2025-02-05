using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class RecipeEditVM : BaseVM
    {

        #region Properties

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

        private ObservableCollection<Label>? _labels;
        public ObservableCollection<Label> Labels
        {
            get => _labels;
            set
            {
                _labels = value; 
                OnPropertyChanged(nameof(Labels));
            }
        }

        private LabelRepository _labelRepository;

        #endregion

        public RecipeEditVM(IRecipeRepository recipeRepository, Recipe recipe, ILabelRepository labelRepository)
        {
            _recipeRepository = recipeRepository;
            _labelRepository = (LabelRepository)labelRepository;
            Recipe = recipe;
            LoadLabels();
            isUpdate = true;
        }

        private async Task LoadLabels()
        {
            Labels = await _labelRepository.GetAllLabels();
        }

        public RecipeEditVM(IRecipeRepository recipeRepository, ILabelRepository labelRepository)
        {
            _recipeRepository = recipeRepository;
            _labelRepository = (LabelRepository)labelRepository;
            LoadLabels();
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

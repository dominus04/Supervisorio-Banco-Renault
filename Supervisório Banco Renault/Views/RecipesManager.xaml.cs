using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para Recipes.xam
    /// </summary>
    public partial class RecipesManager : UserControl
    {
        public RecipesManager()
        {
            InitializeComponent();
        }

        private void AddRecipeMouseDown(object sender, MouseButtonEventArgs e)
        {
            RecipesManagerVM vm = (RecipesManagerVM)DataContext;
            Type t = GetParentService.GetParent(this);
            //VirtualKeyboard.start(t);
            vm.AddOrUpdateRecipe(t, false);
        }

        private void RemoveRecipeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Deseja remover a receita selecionada?", "Remover", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            RecipesManagerVM vm = (RecipesManagerVM)DataContext;
            vm.RemoveRecipe();
        }

        private void EditRecipeMouseDown(object sender, MouseButtonEventArgs e)
        {
            RecipesManagerVM vm = (RecipesManagerVM)DataContext;
            Type t = GetParentService.GetParent(this);
            //VirtualKeyboard.start(t);
            vm.AddOrUpdateRecipe(t, true);
        }
    }
}

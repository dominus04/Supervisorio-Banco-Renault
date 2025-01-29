using Supervisório_Banco_Renault.Libraries;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Interação lógica para Recipes.xam
    /// </summary>
    public partial class Recipes : UserControl
    {
        public Recipes()
        {
            InitializeComponent();
        }

        private void AddRecipeMouseDown(object sender, MouseButtonEventArgs e)
        {
            RecipesVM vm = (RecipesVM)DataContext;
            Type t = GetParentService.GetParent(this);
            VirtualKeyboard.start(t);
            vm.AddOrUpdateRecipe(t, false);
        }

        private void RemoveRecipeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Deseja remover a receita selecionada?", "Remover", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            RecipesVM vm = (RecipesVM)DataContext;
            vm.RemoveRecipe();
        }

        private void EditRecipeMouseDown(object sender, MouseButtonEventArgs e)
        {
            RecipesVM vm = (RecipesVM)DataContext;
            Type t = GetParentService.GetParent(this);
            VirtualKeyboard.start(t);
            vm.AddOrUpdateRecipe(t, true);
        }
    }
}

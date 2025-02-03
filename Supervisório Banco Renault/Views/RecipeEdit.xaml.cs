using Supervisório_Banco_Renault.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Supervisório_Banco_Renault.Views
{
    /// <summary>
    /// Lógica interna para RecipeVM.xaml
    /// </summary>
    public partial class RecipeEdit : Window
    {
        public RecipeEdit()
        {
            InitializeComponent();
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Todos os dados serão perdidos. Deseja cancelar?", "Cancelar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private async void Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecipeEditVM vm = (RecipeEditVM)DataContext;
            if (MessageBox.Show("Deseja salvar?", "Salvar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (await vm.AddOrUpdateRecipe())
                {
                    Close();
                }
            }
        }
    }
}

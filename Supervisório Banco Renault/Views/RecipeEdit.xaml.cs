using Supervisório_Banco_Renault.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                this.Close();
            }
        }

        private async void Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecipeEditVM vm = (RecipeEditVM)DataContext;
            if (MessageBox.Show("Deseja salvar?", "Salvar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (await vm.AddOrUpdateRecipe())
                {
                    this.Close();
                }
            }
        }
    }
}

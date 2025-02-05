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
    /// Interação lógica para LabelsManager.xam
    /// </summary>
    public partial class LabelsManager : UserControl
    {
        public LabelsManager()
        {
            InitializeComponent();
        }

        private void AddLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            LabelsManagerVM vm = (LabelsManagerVM)DataContext;
            vm.AddOrUpdateLabel(false);
        }

        private void EditLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            LabelsManagerVM vm = (LabelsManagerVM)DataContext;
            vm.AddOrUpdateLabel(true);
        }

        private void RemoveLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Deseja remover a etiqueta selecionada?", "Remover", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            LabelsManagerVM vm = (LabelsManagerVM)DataContext;
            vm.RemoveLabel();
        }
    }
}

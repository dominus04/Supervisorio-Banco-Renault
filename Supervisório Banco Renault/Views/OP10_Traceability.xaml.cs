using ClosedXML.Excel;
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
    /// Interação lógica para OP10_Traceability.xam
    /// </summary>
    public partial class OP10_Traceability : UserControl
    {
        public OP10_Traceability()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Rastreabilidade OP20");

            // Cabeçalhos - aqui pegamos os nomes das colunas do DataGrid
            for (int col = 0; col < myDataGrid.Columns.Count; col++)
            {
                worksheet.Cell(1, col + 1).Value = myDataGrid.Columns[col].Header.ToString();
            }

            // Dados - aqui copiamos os dados do DataGrid para o Excel
            var itemsSource = myDataGrid.ItemsSource.Cast<object>().ToList();

            for (int row = 0; row < itemsSource.Count; row++)
            {
                var item = itemsSource[row];

                for (int col = 0; col < myDataGrid.Columns.Count; col++)
                {
                    var binding = (myDataGrid.Columns[col] as DataGridBoundColumn)?.Binding as System.Windows.Data.Binding;
                    if (binding != null)
                    {
                        var propertyName = binding.Path.Path;
                        var propertyValue = item.GetType().GetProperty(propertyName)?.GetValue(item, null);
                        worksheet.Cell(row + 2, col + 1).Value = propertyValue?.ToString();
                    }
                }
            }

            // Salvar arquivo
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = "Rastreabilidade OP10.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Exportação concluída com sucesso!");
            }
            
        }
    }
}

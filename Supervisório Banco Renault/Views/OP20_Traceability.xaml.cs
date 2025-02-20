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
    /// Interação lógica para OP20_Traceability.xam
    /// </summary>
    public partial class OP20_Traceability : UserControl
    {
        public OP20_Traceability()
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
            for (int row = 0; row < myDataGrid.Items.Count; row++)
            {
                var item = myDataGrid.Items[row];
                for (int col = 0; col < myDataGrid.Columns.Count; col++)
                {
                    var cellValue = myDataGrid.Columns[col].GetCellContent(item) as TextBlock;
                    worksheet.Cell(row + 2, col + 1).Value = cellValue?.Text; // Adiciona o valor da célula
                }
            }

            // Salvar arquivo
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = "Rastreabilidade OP20.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Exportação concluída com sucesso!");
            }
        }
    }
}

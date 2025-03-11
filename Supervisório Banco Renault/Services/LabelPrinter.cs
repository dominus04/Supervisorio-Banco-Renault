using Supervisório_Banco_Renault.Models;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Supervisório_Banco_Renault.Services
{
    public class LabelPrinter
    {
        static readonly string PRINTER_IP = "192.168.1.50";
        static readonly int PORT = 9100;

        static readonly Dictionary<int, string> yearCode = new Dictionary<int, string>()
        {
            [2024] = "H",
            [2025] = "J",
            [2026] = "K",
            [2027] = "L",
            [2028] = "M",
            [2029] = "N",
            [2030] = "P",
            [2031] = "Q",
            [2032] = "R",
            [2033] = "S",
            [2034] = "T",
            [2035] = "U",
            [2036] = "B",
            [2037] = "C",
            [2038] = "D",
            [2039] = "E",
            [2040] = "F",
            [2041] = "G",
            [2042] = "H",
            [2043] = "J",
            [2044] = "K",
            [2045] = "L",
            [2046] = "M",
            [2047] = "N",
            [2048] = "P",
            [2049] = "Q",
            [2050] = "R",
            [2051] = "S",
            [2052] = "T",
            [2053] = "U"
        };

        public static (string, DateTime) PrintLabelAndReturnTraceabilityCode(Recipe currentRecipe)
        {

            Label label = currentRecipe.Label;

            string sequential = GetSequential(label.SequentialFormat);

            var julianDate = label.JulianDateFormat.Replace("YY", DateTime.Now.ToString("yy"));
            julianDate = julianDate.Replace("DDD", DateTime.Now.DayOfYear.ToString("D3"));

            var currentDateTime = DateTime.Now;

            string pcode = currentDateTime.ToString("MM") + yearCode[currentDateTime.Year];

            var dateqr = currentDateTime.ToString(label.DateFormat);
            var date = currentDateTime.ToString("dd/MM/yyyy");
            var time = currentDateTime.ToString(label.TimeFormat);

            var turno = "";

            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime >= label.Tunr1Init && currentTime <= label.Tunr1End)
                turno = "1";
            else if (currentTime >= label.Tunr2Init && currentTime <= label.Tunr2End)
                turno = "2";
            else if (currentTime >= label.Tunr3Init && currentTime <= label.Tunr3End)
                turno = "3";

            string zplCommand = currentRecipe.Label.LabelBaseLayout;
            zplCommand = zplCommand.Replace("[C_MODULO]", currentRecipe.ModuleCode);
            zplCommand = zplCommand.Replace("[C_CLIENTE]", currentRecipe.ClientCode);
            zplCommand = zplCommand.Replace("[DATAJU]", julianDate);
            zplCommand = zplCommand.Replace("[DATA]", date);
            zplCommand = zplCommand.Replace("[DATAQR]", dateqr);
            zplCommand = zplCommand.Replace("[HORA]", time);
            zplCommand = zplCommand.Replace("[TURNO]", turno);
            zplCommand = zplCommand.Replace("[SEQUENCIAL]", sequential);
            zplCommand = zplCommand.Replace("[P_CODE]", pcode);

            while (true)
            {
                try
                {

                    using (TcpClient client = new(PRINTER_IP, PORT))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(zplCommand);
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                    }

                    break;

                }
                catch (Exception e)
                {
                    if (MessageBox.Show(
                        $"Erro ao conectar à impressora.\n" +
                        "Verifique a conexão e aperte 'Sim' para tentar novamente ou 'Não' para cancelar a impressão.",
                        "Erro de Conexão",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error) == MessageBoxResult.No)
                    {
                        break;
                    }

                    Thread.Sleep(2000);
                }
            }

            var traceabilityPattern = @"(?:\^BX.*\^FD|\^BQ.*\^FD)(.*)\^FS";

            return (Regex.Match(zplCommand, traceabilityPattern).Groups[1].Value, currentDateTime);

        }

        public static string GetSequential(int sequentialFormat)
        {

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDataDbFolder = Path.Combine(appDataPath, "Supervisorio Banco Renault");
            string sequentialFilePath = Path.Combine(appDataDbFolder, "sequential.txt");

            var sequentialDict = new Dictionary<int, int>
            {
                {1, 9 },
                {2, 99 },
                {3, 999 },
                {4, 9999 },
                {5, 99999 },
            };

            var limit = sequentialDict[sequentialFormat];

            int number;
            if (File.Exists(sequentialFilePath))
            {
                number = int.Parse(File.ReadAllText(sequentialFilePath));
            }
            else
            {
                number = 1;
            }

            int nextNumber = number + 1;
            if (nextNumber > limit)
                number = 1;

            File.WriteAllText(sequentialFilePath, (nextNumber).ToString());

            return number.ToString($"D{sequentialFormat}");

        }

        static string GetPrinterStatus()
        {
            using (TcpClient client = new TcpClient(PRINTER_IP, PORT))
            using (NetworkStream stream = client.GetStream())
            {
                byte[] command = Encoding.ASCII.GetBytes("~HS\r\n");
                stream.Write(command, 0, command.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                return Encoding.ASCII.GetString(buffer, 0, bytesRead);
            }
        }



    }
}

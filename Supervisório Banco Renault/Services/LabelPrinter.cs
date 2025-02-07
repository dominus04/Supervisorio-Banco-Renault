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

        public static string PrintLabelAndReturnTraceabilityCode(Recipe currentRecipe)
        {

            Label label = currentRecipe.Label;

            string sequential = GetSequential(label.SequentialFormat);

            var julianDate = label.JulianDateFormat.Replace("YY", DateTime.Now.ToString("yy"));
            julianDate = julianDate.Replace("DDD", DateTime.Now.DayOfYear.ToString("D3"));

            var date = DateTime.Now.ToString(label.DateFormat);
            var time = DateTime.Now.ToString(label.TimeFormat);

            var turno = "";

            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime >= label.Tunr1Init && currentTime <= label.Tunr1End)
                turno = "1";
            else if (currentTime >= label.Tunr2Init && currentTime <= label.Tunr2End)
                turno = "2";
            else if (currentTime >= label.Tunr3Init && currentTime <= label.Tunr3End)
                turno = "3";

            string zplCommand = currentRecipe.Label.LabelBaseLayout;
            zplCommand = zplCommand.Replace("[C_INTERNO]", currentRecipe.ModuleCode);
            zplCommand = zplCommand.Replace("[DATAJU]", julianDate);
            zplCommand = zplCommand.Replace("[DATA]", date);
            zplCommand = zplCommand.Replace("[HORA]", time);
            zplCommand = zplCommand.Replace("[TURNO]", turno);
            zplCommand = zplCommand.Replace("[SEQUENCIAL]", sequential);

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

            return Regex.Match(zplCommand, traceabilityPattern).Groups[1].Value;

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

using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.Libraries
{
    public class Keyboard
    {
        // Import necessary Windows API functions
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;

        public static void start(Type t)
        {
            try
            {
                // Start the on-screen keyboard without triggering elevation prompt
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "osk.exe",
                    UseShellExecute = true,
                    Verb = "runas" // Ensures it uses administrative privileges
                };

                Process.Start(psi);

                // Wait for the keyboard to launch
                System.Threading.Thread.Sleep(200);

                IntPtr keyboardWindow = IntPtr.Zero;

                // Find the window handle of the on-screen keyboard
                EnumWindows((hWnd, lParam) =>
                {
                    StringBuilder windowText = new StringBuilder(256);
                    GetWindowText(hWnd, windowText, windowText.Capacity);
                    if (windowText.ToString().Contains("Teclado")) // Ajuste para o nome correto
                    {
                        keyboardWindow = hWnd;
                        return false; // Para a enumeração
                    }
                    return true; // Continua enumerando
                }, IntPtr.Zero);

                // Verifica se encontrou a janela
                if (keyboardWindow != IntPtr.Zero)
                {
                    Console.WriteLine("Janela do teclado virtual encontrada.");

                    // Define a posição a janela do teclado virtual dependendo do tipo de janela
                    int width = 1500;
                    int height = 400;
                    int y = 500;
                    int x = t == typeof(OP10_MainWindow) ? -1710 : 210;


                    // Move o teclado virtual
                    SetWindowPos(keyboardWindow, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Um erro ocorreu:  {ex.Message}");
            }
        }
    }
}

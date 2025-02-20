using Microsoft.Extensions.DependencyInjection;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Models.Enums;
using Supervisório_Banco_Renault.Services;
using Supervisório_Banco_Renault.Views;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP20_MainWindowVM : WindowBaseVM
    {
        private readonly IServiceProvider serviceProvider;
        private readonly PlcConnection plcConnection;

        public OP20_MainWindowVM(IServiceProvider serviceProvider, PlcConnection plcConnection)
        {
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Automático", "OP20_Automatic"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Rastreabilidade", "OP20_Traceability"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Manutencao, "Manual", "OP20_Manual"));
            //MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "I/O", "OP20_IoView"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Receitas", "RecipesManager"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Etiquetas", "LabelsManager"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Usuários", "UsersManager"));
            //MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Configurações", "Item3"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.None, "Login", "Login"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Logoff", "Logoff"));
            this.serviceProvider = serviceProvider;
            this.plcConnection = plcConnection;
        }

        public async Task VerifyEmergency()
        {

            if (Application.Current.Windows.OfType<OP20_Emergency>().FirstOrDefault() != null)
                return;

            if (await plcConnection.IsPLCEmergencyOK())
                return;

            OP20_EmergencyVM emergencyVM = serviceProvider.GetRequiredService<OP20_EmergencyVM>();

            OP20_Emergency emergencyScreen = new()
            {
                DataContext = emergencyVM
            };

            emergencyVM.CurrentEmergencyWindow = emergencyScreen;

            var tsc = new TaskCompletionSource<bool>();

            emergencyScreen.Closed += (s, e) => tsc.SetResult(true);

            emergencyScreen.Show();

            ScreenControl = false;

            await tsc.Task;

            ScreenControl = true;

        }

    }
}

using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Models.Enums;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP20_MainWindowVM : WindowBaseVM
    {
        public OP20_MainWindowVM()
        {
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Automático", "OP20_Automatic"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Rastreabilidade", "OP20_Traceability"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Manutencao, "Manual", "OP20_Manual"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "I/O", "OP20_IoView"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Receitas", "Recipes"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Etiquetas", "OP20_Tags"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Usuários", "UsersManager"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Administrador, "Configurações", "Item3"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.None, "Login", "Login"));
            MenuItems?.Add(new MenuItemModel(AccessLevel.Operador, "Logoff", "Logoff"));
        }
    }
}

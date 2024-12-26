using Supervisório_Banco_Renault.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.Models
{
    public class MenuItemModel
    {

        public AccessLevel LoginLevel { get; set; }
        public string? MenuItemText { get; set; }
        public string? ViewName { get; set; }

        public MenuItemModel(AccessLevel loginLevel, string menuItemText, string viewName)
        {
            LoginLevel = loginLevel;
            MenuItemText = menuItemText;
            ViewName = viewName;
        }
    }
}

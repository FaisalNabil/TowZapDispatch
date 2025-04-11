using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Helper
{
    public static class DialogHelper
    {
        public static Task Show(string title, string message, string cancel = "OK")
            => Application.Current?.MainPage?.DisplayAlert(title, message, cancel) ?? Task.CompletedTask;
    }

}

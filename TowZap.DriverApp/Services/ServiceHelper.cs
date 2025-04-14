using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Services
{
    public static class ServiceHelper
    {
        public static T GetService<T>() => Current.GetService<T>();

        private static IServiceProvider Current =>
            App.Current.Handler.MauiContext.Services;
    }
}

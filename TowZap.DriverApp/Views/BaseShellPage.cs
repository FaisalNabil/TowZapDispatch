using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Views
{
    public abstract class BaseShellPage<TViewModel> : ContentPage where TViewModel : class
    {
        protected TViewModel ViewModel => BindingContext as TViewModel;

        protected BaseShellPage(TViewModel viewModel)
        {
            BindingContext = viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (ViewModel is IInitializable initVm)
                await initVm.InitializeAsync();
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
            // Add custom logic if needed
        }
    }

    public interface IInitializable
    {
        Task InitializeAsync();
    }
}

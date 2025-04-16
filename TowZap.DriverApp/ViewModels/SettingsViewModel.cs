using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TowZap.DriverApp.Services;

namespace TowZap.DriverApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly SessionManager _session;

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Role { get; set; }

        public ICommand LogoutCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        public SettingsViewModel(UserService userService, SessionManager sessionManager)
        {
            _userService = userService;
            _session = sessionManager;

            FullName = _session.FullName ?? "User";
            CompanyName = _session.CompanyName ?? "Company";
            Role = _session.Role ?? "Unknown";

            LogoutCommand = new Command(async () => await Logout());
            DeleteAccountCommand = new Command(async () => await DeleteAccountAsync());
        }

        private async Task Logout()
        {
            Preferences.Clear();
            await Shell.Current.GoToAsync("LoginPage");
        }

        private async Task DeleteAccountAsync()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Account", "Are you sure? This cannot be undone.",
                "Yes", "Cancel");

            if (confirm)
            {
                var result = await _userService.DeleteAccountAsync();
                if (result)
                {
                    Preferences.Clear();
                    await Shell.Current.GoToAsync("LoginPage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete account.", "OK");
                }
            }
        }
    }

}

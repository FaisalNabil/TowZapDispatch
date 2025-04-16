using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Input;
using TowZap.DriverApp.Services;
using TowZap.DriverApp.Models;
using TowZap.DriverApp.Helper;

namespace TowZap.DriverApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _password;
        private readonly AuthService _authService;
        private readonly SessionManager _session;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AuthService authService, SessionManager sessionManager)
        {
            _authService = authService;
            _session = sessionManager;
            LoginCommand = new Command(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            try
            {
                var result = await _authService.LoginAsync(new LoginRequest
                {
                    Email = Email,
                    Password = Password
                });

                if (result != null)
                {
                    await _session.SaveSessionAsync(result);

                    // Navigate to dashboard only if Shell is ready
                    if (Shell.Current != null) 
                        //Shell.Current.CurrentItem = Shell.Current.Items[0];
                        await Shell.Current.GoToAsync("//DashboardPage");
                    else
                        await DialogHelper.Show("Error", "Navigation failed: Shell not ready.");
                }
                else
                {
                    await DialogHelper.Show("Login Failed", "Invalid credentials");
                }
            }
            catch (Exception ex)
            {
                await DialogHelper.Show("Unexpected Error", ex.Message);
            }
        }


        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

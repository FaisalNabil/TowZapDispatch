using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowZap.DriverApp.Models;

namespace TowZap.DriverApp.Services
{
    public class SessionManager
    {
        public string? Token { get; private set; }
        public string? FullName { get; private set; }
        public string? Role { get; private set; }
        public string? CompanyName { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

        public async Task InitializeAsync()
        {
            Token = await SecureStorage.GetAsync("auth_token");
            FullName = Preferences.Get("user_fullname", "User");
            Role = Preferences.Get("user_role", "Unknown");
            CompanyName = Preferences.Get("company_name", "Company");
        }

        public async Task SaveSessionAsync(LoginResponse response)
        {
            await SecureStorage.SetAsync("auth_token", response.Token);
            Preferences.Set("user_fullname", response.FullName);
            Preferences.Set("user_role", response.Role);
            Preferences.Set("company_name", response.CompanyName);

            Token = response.Token;
            FullName = response.FullName;
            Role = response.Role;
            CompanyName = response.CompanyName;
        }

        public async Task LogoutAsync()
        {
            Preferences.Clear();
            SecureStorage.Remove("auth_token");

            Token = null;
            FullName = null;
            Role = null;
            CompanyName = null;

            // Navigate to login screen
            await Shell.Current.GoToAsync("LoginPage");
        }
    }
}

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
        public string? UserId { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

        public async Task InitializeAsync()
        {
            Token = await SecureStorage.GetAsync("auth_token");
            FullName = Preferences.Get("user_fullname", "User");
            Role = Preferences.Get("user_role", "Unknown");
            CompanyName = Preferences.Get("company_name", "Company");
            UserId = Preferences.Get("user_id", null);
        }

        public async Task SaveSessionAsync(LoginResponse response)
        {
            await SecureStorage.SetAsync("auth_token", response.Token);
            Preferences.Set("user_fullname", response.FullName);
            Preferences.Set("user_role", response.Role);
            Preferences.Set("company_name", response.CompanyName);
            Preferences.Set("user_id", response.UserId);

            Token = response.Token;
            FullName = response.FullName;
            Role = response.Role;
            CompanyName = response.CompanyName;
            UserId = response.UserId;
        }
        public async Task LogoutAsync()
        {
            // Clear persisted data
            Preferences.Clear();
            SecureStorage.Remove("auth_token");

            // Clear in-memory session
            Token = null;
            FullName = null;
            Role = null;
            CompanyName = null;
            UserId = null;

            // Reset navigation
            Application.Current.MainPage = new LoginShell();
        }

    }
}

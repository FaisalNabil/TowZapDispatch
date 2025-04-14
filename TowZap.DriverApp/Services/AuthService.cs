using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TowZap.DriverApp.Models;

namespace TowZap.DriverApp.Services
{
    public class AuthService : BaseApiService
    {
        public AuthService(HttpClient httpClient) : base(httpClient) { }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            return await PostAsync<LoginRequest, LoginResponse>("auth/login", request);
        }
    }
}

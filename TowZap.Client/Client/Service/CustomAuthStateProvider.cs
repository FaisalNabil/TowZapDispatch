using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using TowZap.Client.Client.Models;

namespace TowZap.Client.Client.Service
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly UserContext _userContext;

        public CustomAuthStateProvider(ILocalStorageService localStorage, UserContext userContext)
        {
            _localStorage = localStorage;
            _userContext = userContext;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var identity = new ClaimsIdentity();

            if (!string.IsNullOrWhiteSpace(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                if (jwt.ValidTo > DateTime.UtcNow)
                {
                    identity = new ClaimsIdentity(jwt.Claims, "jwt");

                    _userContext.Token = token;
                    _userContext.Role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    _userContext.FullName = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    _userContext.CompanyId = Guid.TryParse(jwt.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value, out var cid)
                        ? cid : Guid.Empty; 
                    _userContext.CompanyName = jwt.Claims
                        .FirstOrDefault(c => c.Type == "CompanyName")?.Value;

                }
                else
                {
                    await _localStorage.RemoveItemAsync("authToken");
                }
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            _userContext.Clear();
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
    }

}

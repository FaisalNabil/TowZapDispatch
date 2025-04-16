using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Services
{
    public class UserService : BaseApiService
    {
        public UserService(HttpClient httpClient) : base(httpClient) { }

        public async Task<bool> DeleteAccountAsync()
        {
            return await DeleteAsync("user/delete");
        }

    }
}

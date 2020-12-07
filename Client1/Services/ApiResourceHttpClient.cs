using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client1.Services
{
    public class ApiResourceHttpClient : IApiResourceHttpClient
    {
        private readonly HttpContext _httpContext;
        private HttpClient _httpClient;

        public ApiResourceHttpClient(IHttpContextAccessor httpContextAccesssor)
        {
            _httpContext = httpContextAccesssor.HttpContext;
            _httpClient = new HttpClient();
        }

        public async Task<HttpClient> GetHttpClient()
        {
            var accessToken = await _httpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            _httpClient.SetBearerToken(accessToken);

            return _httpClient;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using BaseLibrary.DTOs;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Helpers
{
    public class CustomHttpHandler
        (GetHttpClient getHttpClient, LocalStorageService localStorageService, IUserAccountService userAccountService) : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login");
            bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("register");
            bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("refresh-token");

            if (loginUrl || registerUrl || refreshTokenUrl) 
                return await base.SendAsync(request, cancellationToken);

            var res = await base.SendAsync(request, cancellationToken);
            if(res.StatusCode == HttpStatusCode.Unauthorized)
            {
                //Get token from local storage first
                var stringToken = await localStorageService.GetToken();
                if (stringToken == null) return res;
                //Check if the header contains token
                string token = string.Empty;
                try
                {
                    token = request.Headers.Authorization.Parameter!;
                }
                catch
                {

                }

                var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
                if(deserializedToken is null) return res;

                if (string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }

                //Call for refresh token
                var newJwtToken = await GetRefreshToken(deserializedToken.RefreshToken!);
                if (string.IsNullOrEmpty(newJwtToken)) return res;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newJwtToken);
                return await base.SendAsync(request, cancellationToken);
            }
            return res;
        }

        private async Task<string> GetRefreshToken(string refreshToken)
        {
            var res = await userAccountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
            string serializedToken = Serializations.SerializeObj(new UserSession() { Token = res.Token, RefreshToken = res.RefreshToken});
            await localStorageService.SetToken(serializedToken);
            return res.Token;
        }
    }
}

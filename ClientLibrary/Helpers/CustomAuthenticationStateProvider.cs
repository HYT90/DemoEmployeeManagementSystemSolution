using System.Security.Claims;
using BaseLibrary.DTOs;
using BaseLibrary.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
namespace ClientLibrary.Helpers
{
    public class CustomAuthenticationStateProvider(LocalStorageService localStorage) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //嘗試取得client local storage的 session string
            var stringToken = await localStorage.GetToken();
            if (string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(anonymous));

            //將session string反序列化為UserSession 物件, 包含Token 及 RefreshToken
            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
            if(deserializeToken == null) return await Task.FromResult(new AuthenticationState(anonymous));

            //將UserSession 物件 的Token解碼, 取得使用者calims
            var getUserClaims = DecryptToken(deserializeToken.Token!);
            if (getUserClaims == null) return await Task.FromResult(new AuthenticationState(anonymous));


            var claimsPrincipal = SetClaimsPrincipal(getUserClaims);
            //provider回傳以成功登入的 claimsPrincipal 建立的 AuthenticationState,
            //之後在Blazor 就能使用 AuthenticationState 來取得 User 的 Claims
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        //更新前端當前的登入者狀態，參數為登入者經過後端登入取得的 UserSession 資訊包含屬性 JWT 及 RefreshToken
        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (userSession.Token != null || userSession.RefreshToken != null)
            {
                //將 UserSession 序列化為字串儲存在 User Agent(瀏覽器) local storage 中
                var serializeSession = Serializations.SerializeObj(userSession);

                await localStorage.SetToken(serializeSession);
                //從 JWT 解密得到 User Claims 資訊並建立 ClaimsPrincipal 用於建立 AuthenticationState 認證狀態
                var getUserClaims = DecryptToken(userSession.Token!);
                claimsPrincipal = SetClaimsPrincipal(getUserClaims);
            }
            else
            {
                await localStorage.RemoveToken();
            }
            //建立 AuthenticationState 認證 User ClaimsPrincipal 登入狀態，並 Notify 登入狀態已改變
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private static CustomUserClaims DecryptToken(string jwtToken)
        {
            var token = JWTProvider.ReadJwt(jwtToken);

            var userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var name = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            var email = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var role = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            return new CustomUserClaims(userId!.Value, name!.Value, email!.Value, role!.Value);
        }

        public static ClaimsPrincipal SetClaimsPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();

            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, claims.Id!),
                        new(ClaimTypes.Name, claims.Name!),
                        new(ClaimTypes.Email, claims.Email!),
                        new(ClaimTypes.Role, claims.Role!),
                    }, "JwtAuth"));
        }
    }
}

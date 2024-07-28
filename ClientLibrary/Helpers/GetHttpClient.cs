using BaseLibrary.DTOs;
namespace ClientLibrary.Helpers
{
    public class GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorage)
    {
        private const string Header = "Authorization";
        public async Task<HttpClient> GetPrivateHttpClient()
        {
            var client = httpClientFactory.CreateClient("SystemApiClient");
            var stringToken = await localStorage.GetToken();//先從local storage 找尋 token, 無結果則直接返回
            if (string.IsNullOrEmpty(stringToken)) return client;

            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);// 找到的 token字串 反序列化並傳回, 反序列化失敗直接返回
            if(deserializeToken == null) return client;

            // request的header authorization 指派 Bearer(scheme) Token(parameter)
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializeToken.Token);
            return client;
        }

        // 當進行登入或註冊帳戶時, 在請求之前把原本儲存的 token 標頭移除
        public HttpClient GetPublicHttpClient()
        {
            var client = httpClientFactory.CreateClient("SystemApiClient");
            client.DefaultRequestHeaders.Remove(Header);
            return client;
        }
    }
}

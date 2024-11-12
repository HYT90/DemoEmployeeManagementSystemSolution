
namespace BaseLibrary.DTOs
{
    //調用 byte[] RandomNumberGenerator.GetBytes(int count) 所產生的隨機數，並調用 Convert.ToBase64String() 轉換為 字串。
    //Refresh Token 用於獲取新的 JWT，當原始 JWT 過期時，客戶端可以使用 Refresh Token 向服務器請求新的 JWT，而不需要重新登入。 
    public class RefreshToken
    {
        public string? Token { get; set; }
    }
}

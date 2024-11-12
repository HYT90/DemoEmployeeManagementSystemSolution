//自訂通用調用 API 回傳的 response (json物件結構的資料)，從 (HttpResponseMessage)res.Content.ReadFromJsonAsync<GeneralResponse> 存取 response 資訊
namespace BaseLibrary.Responses
{
    public record GeneralResponse(bool Flag, string Message = null!);
}

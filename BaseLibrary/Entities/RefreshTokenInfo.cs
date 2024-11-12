

namespace BaseLibrary.Entities
{
    public class RefreshTokenInfo
    {
        public int Id { get; set; }
        //Refersh Token 能找到映射的 UserId
        public string? Token { get; set; }
        public int UserId { get; set; }
    }
}

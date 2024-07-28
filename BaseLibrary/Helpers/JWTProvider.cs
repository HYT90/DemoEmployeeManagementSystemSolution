using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BaseLibrary.Helpers
{
    public class JWTProvider//JWT 包含 SymmetricSecurityKey claims expireDate credentials tokenDescriptor
    {
        private readonly SymmetricSecurityKey symmetricKey;
        private readonly SigningCredentials signingCredentials;
        public JWTProvider(string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            symmetricKey = new SymmetricSecurityKey(keyBytes);
            signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public string GenerateJWT(string? issuer, string? audience, List<Claim> claims, DateTime expireDate)
        {
            var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims: claims, expires: expireDate, signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public ClaimsPrincipal JWTDecode(string jwt)
        {
            var jh = new JwtSecurityTokenHandler();
            var tvp = new TokenValidationParameters();
            tvp.IssuerSigningKey = symmetricKey;
            tvp.ValidateIssuer = false;
            tvp.ValidateAudience = false;
            ClaimsPrincipal cp = jh.ValidateToken(jwt, tvp, out SecurityToken securityToken);

            return cp;
        }

        public static JwtSecurityToken ReadJwt(string jwt)
        {
            var jh = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jh.ReadJwtToken(jwt);
            return jwtSecurityToken;
        }
    }
}

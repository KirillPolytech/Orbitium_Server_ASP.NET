using BackEndForGame.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEndForGame.Services
{
    public class JwtService
    {
        private readonly IOptions<JwtAuthOptions> _options;                    
        public JwtService(IOptions<JwtAuthOptions> option)
        {
            _options = option;
        }

        public string GenerateToken(Guid uid)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, uid.ToString()),
                    //new Claim(ClaimTypes.Email, login),
            }),                
                //Expires = new DateTime(2024, 10, 10),                  
                Issuer = _options.Value.Issuer,
                Audience = _options.Value.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.Value.key)), 
                    SecurityAlgorithms.HmacSha256Signature)                
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}


/*
        public Guid? EncodeToken(JwtToken token)
        {
            SecurityToken tok;
            ClaimsPrincipal handler;
            try
            {
                    handler = new JwtSecurityTokenHandler().ValidateToken(token.Token,
                    new TokenValidationParameters()
                    {
                        IssuerSigningKey = _key,
                        ValidIssuer = _options.Value.Issuer,
                        ValidateIssuer = true,
                        ValidAudience = _options.Value.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = false                        
                    }, out tok);
            }
            catch (Exception e)
            {
                return null;
            }

            var a = handler.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            //var b = handler.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            return new Guid(a);
        }
        */
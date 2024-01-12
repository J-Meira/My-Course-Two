using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyAPI.Helpers;

public class AuthHelper(IConfiguration _config)
{
  public string CreateToken(Claim[] claims, DateTime expires)
  {
    string? tokenKeyString = _config.GetSection("JwtBearerTokenSettings:SecretKey").Value;

    SymmetricSecurityKey tokenKey = new(
      Encoding.UTF8.GetBytes(
        tokenKeyString ?? ""
      )
    );

    SigningCredentials credentials = new SigningCredentials(
      tokenKey,
      SecurityAlgorithms.HmacSha512Signature
    );

    SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
    {
      Subject = new ClaimsIdentity(claims),
      SigningCredentials = credentials,
      Audience = _config["JwtBearerTokenSettings:Audience"],
      Issuer = _config["JwtBearerTokenSettings:Issuer"],
      Expires = expires
    };

    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

    SecurityToken token = tokenHandler.CreateToken(descriptor);

    return tokenHandler.WriteToken(token);

  }

}

using bet_online.API.AuthDomain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace bet_online.API.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration configuration = null;

        public AuthController(IConfiguration config)
        {
            configuration = config ?? throw new ArgumentNullException(nameof(config));
        }
        [HttpPost("authenticate")]
        public ActionResult Auth(AuthInputDTO request)
        {
            var user = ValidateUserLogin(request?.username, request?.password);
            if (user == null)
            {
                return Unauthorized();
            }

            var Expires = DateTime.UtcNow.AddDays(1);
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(configuration["Authentication:SecreteKey"]));

            var signingCreadentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.GivenName, user.Username));

            var jwtSecurityToken = new JwtSecurityToken(
                configuration["Authentication:Issuer"],
                configuration["Authentication:Audience"],
                claims,
                DateTime.UtcNow,
                Expires,
                signingCreadentials
                );
            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var output = new AuthOutputDTO()
            {
                access_token = userToken,
                token_type = "bearer",
                expires_in = (int)Expires.Subtract(DateTime.Now).TotalSeconds
            };


            return Ok(output);
        }

        private UserInfo ValidateUserLogin(string? username, string? password)
        {
            if (username != configuration["Authentication:Username"] || password != configuration["Authentication:Password"])
            {
                return null;
            }
            return new UserInfo(username, password);
        }
    }
    public class UserInfo
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

        public UserInfo(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}

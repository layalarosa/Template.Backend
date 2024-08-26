using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Template.Backend.Dtos;

namespace Template.Backend.Controllers
{
    [Route("api/usuarios")]
    public class UserController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signManager, 
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signManager;
            this.configuration = configuration;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<AuthenticationResponseDto>> Register(UserCredentialsDto userCredentialsDto)
        {
            var user = new IdentityUser
            {
                Email = userCredentialsDto.Email,
                UserName = userCredentialsDto.Email,

            };

            var resultado = await userManager.CreateAsync(user, userCredentialsDto.Password);

            if (resultado.Succeeded)
            {
                return await BuildToken(user);
            }
            else 
            {
                return BadRequest(resultado.Errors);
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponseDto>> Login(UserCredentialsDto userCredentialsDto)
        {
            var user = await userManager.FindByIdAsync(userCredentialsDto.Email);

            if (user is null)
            {
                var errores = IncorrectLogin();
                return BadRequest(errores);

            }

            var result = await signInManager.CheckPasswordSignInAsync(user, 
                userCredentialsDto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(user);
            }
            else 
            {
                var errores = IncorrectLogin();
                return BadRequest(errores);

            }
        }

        private IEnumerable<IdentityError> IncorrectLogin()
        {
            var identityError = new IdentityError() { Description = "Incorrect Login" };
            var errores = new List<IdentityError>();
            errores.Add(identityError);
            return errores;
            
        }

        private async Task<AuthenticationResponseDto> BuildToken(IdentityUser identityUser)
        {
            var claims = new List<Claim>
            {
                new Claim("email", identityUser.Email!),
                new Claim("Whatever i want", "Any value")

            };

            var claimsDB = await userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDB);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var tokenSecurity = new JwtSecurityToken(issuer: null, audience: null, claims: claims, 
                expires: expiration, signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenSecurity);

            return new AuthenticationResponseDto 
            { 
                Token = token,
                Expiration = expiration,
            };
        }



    }
}

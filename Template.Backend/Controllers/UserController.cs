using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Template.Backend.Dtos;
using Template.Backend.NewFolder;
using Template.Backend.Utilities;

namespace Template.Backend.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isadmin")]
    public class UserController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("userlist")]
        public async Task<ActionResult<List<UserDto>>> UserLists([FromQuery] PaginationDto paginationDto)
        {
            var queryable = context.Users.AsQueryable();
            await HttpContext.InsertPaginationParametersInHeader(queryable);
            var user = await queryable.ProjectTo<UserDto>(mapper.ConfigurationProvider)
                .OrderBy(x => x.Email).Paginar(paginationDto).ToListAsync();

            return user;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Register(UserCredentialsDto userCredentialsDto)
        {
            var user = new IdentityUser
            {
                Email = userCredentialsDto.Email,
                UserName = userCredentialsDto.Email,

            };

            var result = await userManager.CreateAsync(user, userCredentialsDto.Password);

            if (result.Succeeded)
            {
                return await BuildToken(user);
            }
            else 
            {
                return BadRequest(result.Errors);
            }

        }

        //[HttpPost("register")]
        //[AllowAnonymous]
        //public async Task<ActionResult<AuthenticationResponseDto>> Register(UserCredentialsDto userCredentialsDto)
        //{
        //    var user = new IdentityUser
        //    {
        //        Email = userCredentialsDto.Email,
        //        UserName = userCredentialsDto.Email,
        //    };

        //    var result = await userManager.CreateAsync(user, userCredentialsDto.Password);

        //    if (result.Succeeded)
        //    {
        //        // Generar el token de confirmación de correo
        //        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        //        // Generar el link de confirmación
        //        var confirmationLink = Url.Action("ConfirmEmail", "Account",
        //            new { userId = user.Id, token = token }, Request.Scheme);

        //        // Enviar el correo electrónico
        //        await emailSender.SendEmailAsync(user.Email, "Confirma tu cuenta",
        //            $"Por favor confirma tu cuenta haciendo clic en este enlace: <a href='{confirmationLink}'>Confirmar Email</a>");

        //        // Opcionalmente puedes devolver una respuesta indicando que el registro fue exitoso pero necesita confirmación
        //        return Ok(new { Message = "Usuario registrado exitosamente. Por favor, revisa tu correo para confirmar tu cuenta." });
        //    }
        //    else
        //    {
        //        return BadRequest(result.Errors);
        //    }
        //}

        [HttpGet("confirmemail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest("El token de confirmación es inválido.");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("El usuario no fue encontrado.");

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Tu correo ha sido confirmado. Ahora puedes iniciar sesión.");
            }
            else
            {
                return BadRequest("Error al confirmar el correo.");
            }
        }



        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Login(UserCredentialsDto userCredentialsDto)
        {
            // Old code 
            //var user = await userManager.FindByEmailAsync(userCredentialsDto.Email);

            // Almacenar el email en una variable
            string email = userCredentialsDto.Email;

            // Utilizar la variable email en lugar de userCredentialsDto.Email
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                var errors = IncorrectLogin();
                return BadRequest(errors);

            }

            var result = await signInManager.CheckPasswordSignInAsync(user, 
                userCredentialsDto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(user);
            }
            else 
            {
                var errors = IncorrectLogin();
                return BadRequest(errors);

            }
        }

        [HttpPost("MakeAdmin")]
        public async Task<IActionResult> MakeAdmin(EditClaimDto editClaimDto)
        {
            var user = await userManager.FindByEmailAsync(editClaimDto.Email);

            if (user is null)
            {
                return NotFound();
            }

            await userManager.AddClaimAsync(user, new Claim("isadmin", "true"));
            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        public async Task<IActionResult> RemoveAdmin(EditClaimDto editClaimDto)
        {
            var user = await userManager.FindByEmailAsync(editClaimDto.Email);

            if (user is null)
            {
                return NotFound();
            }

            await userManager.RemoveClaimAsync(user, new Claim("isadmin", "true"));
            return NoContent();
        }

        private IEnumerable<IdentityError> IncorrectLogin()
        {
            var identityError = new IdentityError() { Description = "Incorrect Login" };
            var errors = new List<IdentityError>();
            errors.Add(identityError);
            return errors;
            
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

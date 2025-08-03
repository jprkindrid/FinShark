using api.DTOs.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AccountController> logger;
        
        private const string DefaultUserRole = "User";
        private const string InvalidCredentialsMessage = "Invalid credentials";

        public AccountController(
            UserManager<AppUser> userManager, 
            ITokenService tokenService, 
            SignInManager<AppUser> signInManager,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null)
            {
                logger.LogWarning("Login attempt with invalid username: {Username}", loginDto.Username);
                return Unauthorized(InvalidCredentialsMessage);
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed login attempt for user: {Username}", user.UserName);
                return Unauthorized(InvalidCredentialsMessage);
            }

            logger.LogInformation("Successful login for user: {Username}", user.UserName);
            return Ok(
                new NewUserDTO
                {
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Token = tokenService.CreateToken(user)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if user already exists
                var existingUser = await userManager.FindByNameAsync(registerDto.Username);
                if (existingUser != null)
                {
                    return BadRequest("Username is already taken");
                }

                var existingEmail = await userManager.FindByEmailAsync(registerDto.Email);
                if (existingEmail != null)
                {
                    return BadRequest("Email is already registered");
                }

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };

                var createdUser = await userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(appUser, DefaultUserRole);
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Successfully registered new user: {Username}", appUser.UserName);
                        return Ok(
                            new NewUserDTO
                            {
                                UserName = appUser.UserName!,
                                Email = appUser.Email!,
                                Token = tokenService.CreateToken(appUser)
                            }
                        );
                    }
                    else
                    {
                        logger.LogError("Failed to assign role to user {Username}: {Errors}", 
                            appUser.UserName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        
                        // Clean up the created user since role assignment failed
                        await userManager.DeleteAsync(appUser);
                        return BadRequest("Failed to create user account");
                    }
                }
                else
                {
                    logger.LogWarning("Failed to create user {Username}: {Errors}", 
                        registerDto.Username, string.Join(", ", createdUser.Errors.Select(e => e.Description)));
                    return BadRequest(createdUser.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during user registration for username: {Username}", registerDto.Username);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}

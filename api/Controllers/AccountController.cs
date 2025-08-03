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

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.signInManager = signInManager;
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
                return Unauthorized("Invalid username");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Username not found and/or password incorrect");

            return Ok(
                new NewUserDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
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

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };

                var createdUser = await userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDTO
                            {
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = tokenService.CreateToken(appUser)
                            }
                            );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}

using Blog.Data;
using Blog.Extensions;
using Blog.Services;
using Blog.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;

// [Authorize]
[ApiController]
[Route("v1/accounts")]
public class AccountController : ControllerBase
{
    // private readonly TokenService _tokenService;
    //
    // public AccountController(TokenService tokenService)
    // {
    //     _tokenService = tokenService;
    // }
    
    // [AllowAnonymous]
    // [HttpPost("log")]
    // public IActionResult Log([FromServices] TokenService tokenService)
    // {
    //     var token = tokenService.GenerateToken(null);
    //
    //     return Ok(token);
    // }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context
            .Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, "ACCX01 - Falha interna no servidor");
        }
    }

    // [Authorize(Roles = "user")]
    // [HttpGet("user")]
    // public IActionResult GetUser() => Ok(User.Identity.Name);
    //
    // [Authorize(Roles = "author")]
    // [HttpGet("author")]
    // public IActionResult GetAuthor() => Ok(User.Identity.Name);
    //
    // [Authorize(Roles = "admin")]
    // [HttpGet("admin")]
    // public IActionResult GetAdmin() => Ok(User.Identity.Name);
}
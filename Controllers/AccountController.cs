using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModel.Accounts;
using Blog.ViewModel.Results;
using Blog.ViewModel.UploadImage;
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

    [Authorize]
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,")
            .Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<string>("ACCX02 - Falha interna no servidor"));
        }

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user is null)
            return NotFound(new ResultViewModel<User>("ACCX03 - Usuário não encontrado"));

        user.Image = $"https://localhost:7152/images/{fileName}";

        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("ACCX04 - Falha Interna no servidor"));
        }

        return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!", null));
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
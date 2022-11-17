using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;

[ApiController]
[Route("v1/users")]
public class UserController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
    {
        try
        {
            var users = await context.Users.ToListAsync();
            return Ok(new ResultViewModel<List<User>>(users));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<User>("USRX11 - Falha interna no servidor"));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var user = await context
                .Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new ResultViewModel<User>("USRX01 - Usuário não encontrado"));

            return Ok(new ResultViewModel<User>(user));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<User>("USRX02 - Falha interna no servidor"));
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> PostAsync(
        [FromBody] CreateUserViewModel model,
        [FromServices] EmailService emailService,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));

        var user = new User
        {
            Id = 0,
            Name = model.Name,
            Email = model.Email.ToLower(),
            Bio = model.Bio,
            Image = model.Image,
            Slug = model.Slug.ToLower(),
            GitHub = model.GitHub
        };

        var password = model.PasswordHash;
        user.PasswordHash = PasswordHasher.Hash(password);
        //var password = PasswordGenerator.Generate(25, true, false);
        
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(user.Name, user.Email, "Bem vindo ao blog!", $"Sua senha é <strong>{password}</strong>");
            
            return Created($"v1/users/{user.Id}", new ResultViewModel<User>(user));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<User>("USRX03 - Não foi possível incluir o usuário"));
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<User>("USRX04 - Falha interna no servidor"));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] EditUserViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));
        
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new ResultViewModel<User>("USRX05 - Usuário não encontrado"));

            user.Name = model.Name;
            user.Email = model.Email;
            user.Bio = model.Bio;
            user.Image = model.Image;
            user.Slug = model.Slug;
            user.GitHub = model.GitHub;

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<User>(user));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<User>("USRX06 - Não foi possível atualizar o usuário"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<User>("USRX07 - Falha interna no servidor"));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new ResultViewModel<User>("USRX08 - Usuário não encontrado"));

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<User>(user));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<User>("USRX09 - Não foi possível deletar o usuário"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<User>("USRX10 - Falha interna no servidor"));
        }
    }
}
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return StatusCode(500, new ResultViewModel<User>("USRX01 - Falaha interna no servidor"));
        }
    }

    [HttpGet("{id:int")]
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
                return NotFound(new ResultViewModel<User>("Conteúdo não encontrado"));

            return Ok(new ResultViewModel<User>(user));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<User>("USRX02 - Falha interna no servidor"));
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> PostAsync(
        [FromBody] EditorUserViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));

        try
        {
            var user = new User
            {
                Id = 0,
                Name = model.Name,
                Email = model.Email.ToLower(),
                PasswordHash = model.PasswordHash,
                Bio = model.Bio,
                Image = model.Image,
                Slug = model.Slug.ToLower(),
                GitHub = model.GitHub
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return Created($"v1/users/{user.Id}", new ResultViewModel<User>(user));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<User>("USRX03 - Não foi possível incluir o usuário"));
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<User>("USRX04 - Falha interna no servidor"));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] EditorUserViewModel model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new ResultViewModel<User>("USRX05 - Conteúdo não encontrado"));

            user.Name = model.Name;
            user.Email = model.Email;
            user.PasswordHash = model.PasswordHash;
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
            return StatusCode(500, new ResultViewModel<User>("USRX06 - Não foi possível atualizsar o usuário"));
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
                return NotFound(new ResultViewModel<User>("USRX08 - Conteúdo não encontrado"));

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
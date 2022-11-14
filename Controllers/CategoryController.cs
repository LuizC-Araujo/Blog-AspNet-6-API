using Blog.Data;
using Blog.Models;
using Blog.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
[Route("v1/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
    {
        try
        {
            var categories = await context.Categories.ToListAsync();
            return Ok(categories);
        }
        catch (Exception e)
        {
            return StatusCode(500, "CATX01 - Falha interna no servidor");
        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();
        
            return Ok(category);
        }
        catch (Exception e)
        {
            return StatusCode(500, "CATX02 - Falha interna no servidor");
        }
    }
    
    [HttpPost("")]
    public async Task<IActionResult> PostAsync(
        [FromBody] CreateCategoryViewModel model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = new Category
            {
                Id = 0,
                Name = model.Name, 
                Slug = model.Slug.ToLower(),
            };
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{category.Id}", category);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "CATX03 - Não foi possível incluir a categoria");
        }
        catch (Exception e)
        {
            return StatusCode(500, "CATX04 - Falha interna no servidor");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] Category model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            category.Name = model.Name;
            category.Slug = model.Slug;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{model.Id}", model);
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "CATX05 - Não foi possível atualizar a categoria");
        }
        catch (Exception e)
        {
            return StatusCode(500, "CATX06 - Falha interna no servidor");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            
            return Ok(category);
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "CATX07 - Não foi possível deletar a categoria");
        }
        catch (Exception e)
        {
            return StatusCode(500, "CATX08 - Falha interna no servidor");
        }
    }
}
using Blog.Data;
using Blog.Models;
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
        var categories = await context.Categories.ToListAsync();
        return Ok(categories);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        var category = await context
            .Categories
            .FirstOrDefaultAsync();

        if (category == null)
            return NotFound();
        
        return Ok(category);
    }
    
    [HttpPost("")]
    public async Task<IActionResult> PostAsync(
        [FromBody] Category category,
        [FromServices] BlogDataContext context)
    {
        try
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{category.Id}", category);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "Não foi possível incluir a categoria");
        }
        catch (Exception e)
        {
            return StatusCode(500, "Falha interna no servidor");
        }
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] Category model,
        [FromServices] BlogDataContext context)
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
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
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
}
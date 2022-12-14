using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModel.Categories;
using Blog.ViewModel.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers;

[ApiController]
[Route("v1/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAsync(
        [FromServices] IMemoryCache cache,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var categories = cache.GetOrCreate("CategoriesCache", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return GetCategories(context);
            });
            return Ok(new ResultViewModel<List<Category>>(categories));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<List<Category>>("CATX01 - Falha interna no servidor"));
        }
    }

    private List<Category> GetCategories(BlogDataContext context)
    {
        return context.Categories.ToList();
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
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new ResultViewModel<Category>("CATX02 - Categoria não encontrada"));
        
            return Ok(new ResultViewModel<Category>(category));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("CATX03 - Falha interna no servidor"));
        }
    }
    
    [HttpPost("")]
    public async Task<IActionResult> PostAsync(
        [FromBody] EditorCategoryViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
        
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

            return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new ResultViewModel<Category>("CATX04 - Não foi possível incluir a categoria"));
        }
        catch 
        {
            return StatusCode(500, new ResultViewModel<Category>("CATX05 - Falha interna no servidor"));
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] EditorCategoryViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));
        
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new ResultViewModel<Category>("CATX06 - Categoria não encontrada"));

            category.Name = model.Name;
            category.Slug = model.Slug;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Category>(category));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Category>("CATX07 - Não foi possível atualizar a categoria"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("CATX08 - Falha interna no servidor"));
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
                return NotFound(new ResultViewModel<Category>("CATX09 - Categoria não encontrada"));

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<Category>(category));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Category>("CATX10 - Não foi possível deletar a categoria"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("CATX11 - Falha interna no servidor"));
        }
    }
}
using System.ComponentModel.DataAnnotations;
using Blog.Models;

namespace Blog.ViewModel;

public class EditorUserViewModel
{
    [Required(ErrorMessage = "O campo nome é obrigatório")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "O campo nome deve conter entre 3 e 80 caracteres")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "O campo email é obrigatório")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "O campo password é obrigatório")]
    public string PasswordHash { get; set; }
    
    [Required(ErrorMessage = "O campo bio é obrigatório")]
    public string Bio { get; set; }
    
    public string Image { get; set; }
    
    [Required(ErrorMessage = "O campo slug é obrigatório")]
    public string Slug { get; set; }
    
    public string GitHub { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel;

public class EditUserViewModel
{
    [Required(ErrorMessage = "O campo nome é obrigatório")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "O campo nome deve conter entre 3 e 80 caracteres")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O campo email é obrigatório")]
    [EmailAddress(ErrorMessage = "O e-mail é inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo bio é obrigatório")]
    public string Bio { get; set; }
    
    public string Image { get; set; }
    
    [Required(ErrorMessage = "O campo slug é obrigatório")]
    public string Slug { get; set; }
    
    public string GitHub { get; set; }
}
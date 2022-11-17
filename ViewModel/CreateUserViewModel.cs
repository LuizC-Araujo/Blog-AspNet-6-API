using System.ComponentModel.DataAnnotations;
using Blog.Models;

namespace Blog.ViewModel;

public class CreateUserViewModel : EditUserViewModel
{
    [Required(ErrorMessage = "O campo password é obrigatório")]
    public string PasswordHash { get; set; }
}
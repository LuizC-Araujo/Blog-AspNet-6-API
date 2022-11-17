using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts;

public class CreateUserViewModel : EditUserViewModel
{
    [Required(ErrorMessage = "O campo password é obrigatório")]
    public string PasswordHash { get; set; }
}
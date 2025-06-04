using System.ComponentModel.DataAnnotations;

namespace ProjetoLoginRegister.Shared
{
    public class UserRegister
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "A senha deve ter pelo menos 4 caracteres")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Senha e Confirmação de Senha não coicidem" )]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; } = "User";
    }
}

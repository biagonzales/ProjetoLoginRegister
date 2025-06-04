namespace ProjetoLoginRegister.Domain
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash {  get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;
    }
}

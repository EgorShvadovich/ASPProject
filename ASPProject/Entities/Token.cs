namespace ASPProject.Entities
{
    public class Token
    {
        public String Id { get; set; } = null!;
        public Guid UserId { get; set; }
        public DateTime Expires { get; set; }
    }
}

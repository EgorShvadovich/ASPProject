using ASPProject.Services.Validations;

namespace ASPProject.Models.Forum.Topic
{
    public class ThemeFormModel
    {
        [ValidationRules(ValidationRule.NotEmpty)]
        public String Title { get; set; } = null!;
        public Guid TopicId { get; set; }
        public Guid UserId { get; set; }
        [ValidationRules(ValidationRule.NotEmpty)]
        public string Content { get; set; } = null!;
    }
}

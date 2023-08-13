using ASPProject.Services.Validations;

namespace ASPProject.Models.Forum.Theme
{
    public class CommentFormModel
    {
        [ValidationRules(ValidationRule.NotEmpty)]
        public String Content { get; set; } = null!;
        public Guid ThemeId { get; set; }
        public Guid? ReplyId { get; set; }
    }
}

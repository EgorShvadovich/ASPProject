using ASPProject.Services.Validations;
using Microsoft.AspNetCore.Mvc;

namespace ASPProject.Models.Forum.Section
{
    public class TopicFormModel
    {

        [FromForm(Name = "topic-title")]
        [ValidationRules(ValidationRule.NotEmpty)]
        public String Title { get; set; } = null!;


        [FromForm(Name = "topic-description")]
        [ValidationRules(ValidationRule.NotEmpty)]
        public String Description { get; set; } = null!;


        [FromForm(Name = "topic-image")]
        [ValidationRules(ValidationRule.NotEmpty)]
        public IFormFile ImageFile { get; set; } = null!;

        [FromForm(Name = "section-id")]
        [ValidationRules(ValidationRule.NotEmpty)]
        public Guid SectionId { get; set; }
    }
}

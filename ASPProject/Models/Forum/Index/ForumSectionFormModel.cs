using ASPProject.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ASPProject.Models.Forum.Index
{
    public class ForumSectionFormModel
    {
        [FromForm(Name = "section-title")]
        [ValidationRules(ValidationRule.NotEmpty)]
        public String Title { get; set; } = null!;

        [FromForm(Name = "section-description")]
        [ValidationRules(ValidationRule.NotEmpty)]
        public String Description { get; set; } = null!;
        [FromForm(Name = "section-image")]
        public IFormFile ImageFile { get; set; } = null!;
    }
}

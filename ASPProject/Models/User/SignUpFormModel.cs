using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ASPProject.Models.User
{
    public class SignUpFormModel
    {
        [FromForm(Name = "user-login")]
        public string Login { get; set; } = null!;
        [FromForm(Name = "user-password")]
        public string Password { get; set; } = null!;
        [FromForm(Name = "user-repeat")]
        public string RepeatPassword { get; set; } = null!;
        [FromForm(Name = "user-name")]
        public string? RealName { get; set; } = null!;
        [FromForm(Name = "user-email")]
        public string Email { get; set; } = null!;

        [FromForm(Name = "user-avatar")]
        [JsonIgnore]
        public IFormFile Avatar { get; set; } = null!;
        [FromForm(Name = "rememeber")]
        public string IsAgree { get; set; } = null!;
    }
}
/* Модели и передача данных
 * В АСП модели - классы,использующиеся для передачи данных.
 * Очень часто каждое представление имеет свою модель,поэтому моделей
 * много и их следует группировать с именами контроллеров
 * 
 * Для приема данных от форм также создают модели. При этом дополнительная
 * задача таких моделей - согласовать имена полей в форме(разметке)
 * и в модели
 */

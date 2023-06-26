using ASPProject.Data;
using ASPProject.Models.User;
using ASPProject.Services.Email;
using ASPProject.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ASPProject.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailService _emailService;


        public UserController(DataContext dataContext, IHashService hashService, ILogger<UserController> logger, IEmailService emailService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
            _logger = logger;
            _emailService = emailService;
        }

        public JsonResult UpdateEmail(String email)
        {
          
            // _logger.LogInformation("UpdateEmail works {email}", email);
            // проверяем что пользователь аутентифицирован
            if (HttpContext.User.Identity?.IsAuthenticated != true)
            {
                return Json(new { success = false, message = "UnAuthenticated" });
            }
            Guid userId;
            try
            {   // извлекаем из Claims ID и ...
                userId = Guid.Parse(
                    HttpContext.User.Claims.First(
                        c => c.Type == ClaimTypes.Sid
                    ).Value);
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateEmail exception {ex}", ex.Message);
                return Json(new { success = false, message = "UnAuthorized" });
            }
            // ... находим по нему пользователя
            var user = _dataContext.Users.Find(userId);

            if (user.Email == email)
            {
                return Json(new { success = false, message = "Email is not changed" });
            }

            if (user == null)
            {
                return Json(new { success = false, message = "Access denied" });
            }
            // генерируем код для подтверждения почты
            String confirmCode = Guid.NewGuid().ToString()[..6].ToUpperInvariant();
            try
            {
                _emailService.Send(
                    email,
                    $"To confirm Email enter code <b>{confirmCode}</b>",
                    "Email changed");
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateEmail exception {ex}", ex.Message);
                return Json(new { success = false, message = "Email invalid" });
            }

            user.Email = email;
            user.ConfirmCode = confirmCode;  // сохраняем в БД код подтверждения почты

            _dataContext.SaveChanges();
            return Json(new { success = true });
        }

        public ViewResult Profile()
        {
            // находим ид пользователя из Claims
            String? userId = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

            ProfileViewModel model = null!;
            if (userId is not null)
            {
                // находим данные о пользователе по ид
                var user = _dataContext.Users.Find(Guid.Parse(userId));
                if (user != null)
                {
                    model = new()
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Avatar = user.Avatar ?? "no-photo.png",
                        CreatedDt = user.CreatedDt,
                        Login = user.Login,
                        IsEmailConfirmed = (user.ConfirmCode == null),
                    };
                }
            }
            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Auth([FromBody] AuthAjaxModel model)
        {
            var user = _dataContext.Users.FirstOrDefault(
                u => u.Login == model.Login &&
                u.PasswordHash == _hashService.GetHash(model.Password)
                );

            if (user != null)
            {
                HttpContext.Session.SetString("userId", user.Id.ToString());
            }
            return Json(new { Success = (user != null) });
        }

        
        public IActionResult Login(SignUpFormModel? formModel)
        {
            SignUpViewModel viewModel;
            if (Request.Method == "POST" && formModel != null)
            {
                // передача формы
                viewModel = ValidateSignUpForm(formModel);
                viewModel.FormModel = formModel;
                HttpContext.Session.SetString("FromData", System.Text.Json.JsonSerializer.Serialize(viewModel));
                return RedirectToAction(nameof(Login));
            }
            else
            {
                if (HttpContext.Session.Keys.Contains("FromData"))
                {
                    String? data = HttpContext.Session.GetString("FromData");
                    if (data != null)
                    {
                        viewModel = System.Text.Json.JsonSerializer.Deserialize<SignUpViewModel>(data)!;
                        HttpContext.Session.Remove("FromData");
                    }
                    else
                    {
                        viewModel = new();
                        viewModel.FormModel = null;  // нечего проверять
                    }

                }
                else
                {
                    // первый заход - начало заполнения формы
                    viewModel = new();
                    viewModel.FormModel = null;  // нечего проверять
                }

            }
            return View(viewModel);  // передаем модель в представление
        }

        private SignUpViewModel ValidateSignUpForm(SignUpFormModel formModel)
        {
            SignUpViewModel viewModel = new();
            if (String.IsNullOrEmpty(formModel.Login))
            {
                viewModel.LoginMessage = "Логин не может быть пустым";
            }
            else if (formModel.Login.Length < 3)
            {
                viewModel.LoginMessage = "Логин слишком короткий (3 символа минимум)";
            }
            else if (_dataContext.Users.Any(u => u.Login == formModel.Login))
            {
                viewModel.LoginMessage = "Данный логин уже занят";
            }
            else
            {
                viewModel.LoginMessage = null;  // все проверки логина пройдены
            }


            if (String.IsNullOrEmpty(formModel.Password))
            {
                viewModel.PasswordMessage = "Пароль не может быть пустым";
            }
            else if (formModel.Password.Length < 3)
            {
                viewModel.PasswordMessage = "Пароль слишком короткий (3 символа минимум)";
            }
            else if (!Regex.IsMatch(formModel.Password, @"\d"))
            {
                viewModel.PasswordMessage = "Пароль должен содержать цифры";
            }
            else
            {
                viewModel.PasswordMessage = null;  // все проверки пароля пройдены
            }


            if (String.IsNullOrEmpty(formModel.RepeatPassword))
            {
                viewModel.RepeatMessage = "Вы должны подтвердить свой пароль";
            }
            else if (formModel.Password != formModel.RepeatPassword)
            {
                viewModel.RepeatMessage = "Пароли не совпадают";
            }
            else
            {
                viewModel.RepeatMessage = null;  // все проверки пароля пройдены
            }

            if (formModel.RealName != null)
            {
                if (Regex.IsMatch(formModel.RealName, @"\d|[\W_]"))
                {
                    viewModel.RealNameMessage = "Имя содержит цифры или символы";
                }
                else
                {
                    viewModel.RealNameMessage = null;  // все проверки пароля пройдены
                }
            }



            if (String.IsNullOrEmpty(formModel.Email))
            {
                viewModel.EmailMessage = "E-mail не может быть пустым";
            }
            else if (_dataContext.Users.Any(u => u.Email == formModel.Email))
            {
                viewModel.EmailMessage = "На данную почту уже есть регистрация";
            }
            else
            {
                viewModel.EmailMessage = null;  // все проверки пароля пройдены
            }

            String nameAvatar = null!;
            // сохранение файла
            if (formModel.Avatar != null)  // файл передан
            {
                /* При приема файла важно:
                 * - проверить допустимые расширения (тип)
                 * - проверить максимальный размер
                 * - заменить имя файла
                 * Создаем папку (вручную) wwwroot/avatars/
                 *  и в нее будем сохранять файлы, расширения - оставляем
                 *  какие есть, а вместо имени - используем GUID
                 */
                if (formModel.Avatar.Length > 1048576)
                {
                    viewModel.AvatarMessage = "Файл слишком большой (макс 1 МБ)";
                }
                // определяем расширение файла
                String ext = Path.GetExtension(formModel.Avatar.FileName);
                // проверить расширение на перечень допустимых

                // формируем имя для файла
                nameAvatar = Guid.NewGuid().ToString() + ext;

                formModel.Avatar.CopyTo(
                    new FileStream("wwwroot/avatars/" + nameAvatar, FileMode.Create));
            }



            if (viewModel.LoginMessage == null &&
                viewModel.PasswordMessage == null &&
                viewModel.AvatarMessage == null)
            {
                _dataContext.Users.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Login = formModel.Login,
                    PasswordHash = _hashService.GetHash(formModel.Password),
                    Email = formModel.Email!,
                    CreatedDt = DateTime.Now,
                    Name = formModel.RealName!,
                    Avatar = nameAvatar
                });
                _dataContext.SaveChanges();
            }
            return viewModel;
        }

        public RedirectToActionResult Logout()
        {
            /* Выход из авторизированного режима всегда должен
          * перенаправить на страницу, которая доступна без авторизации
          * Чаще всего - на домашнюю страницу
          */
            HttpContext.Session.Remove("userId");
            return RedirectToAction("Index", "Home");
        }
    }
}

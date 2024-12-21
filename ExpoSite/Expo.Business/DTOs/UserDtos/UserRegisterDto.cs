using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.UserDtos
{
    public record UserRegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class UserRegisterDtoValidation : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidation()
        {

            RuleFor(x => x.Email).NotEmpty()
            .WithMessage("The email cannot be empty!")
            .EmailAddress()
            .WithMessage("Email format is not correct!").Must(u => !u.Contains(" "))
            .WithMessage("The Email cannot contain spaces!");

            RuleFor(x => x.Password).NotEmpty()
            .WithMessage("The password cannot be empty!")
            .Must(r =>
            {
                Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,50}$");
                Match match = passwordRegex.Match(r);
                return match.Success;
            }).WithMessage("Password format is not correct!")
            .Must(p => !p.Contains(" "))
            .WithMessage("The password cannot contain spaces!");
        }
    }
}

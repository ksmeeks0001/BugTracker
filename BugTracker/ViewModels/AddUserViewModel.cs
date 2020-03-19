using BugTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [UniqueEmailValidator]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email")]
        [Compare("Email", ErrorMessage = "The email and confirmation email do not match.")]
        public string ConfirmEmail { get; set; }

        public List<IdentityRole> RoleOptions { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class UniqueEmailValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var pending = (AddUserViewModel)validationContext.ObjectInstance;
            var email = db.Users.SingleOrDefault(u => u.Email == pending.Email);
            var pendingemail = db.PendingRegistrations.SingleOrDefault(p => p.Email == pending.Email);
            if (email != null)
                return new ValidationResult("A user exists with this email address");
            
            if (pendingemail != null)
                return new ValidationResult("Registration already pending for email");

            return ValidationResult.Success;

        }
    }
}
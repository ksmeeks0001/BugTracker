using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Validation
{
 
    public class IsPending : ValidationAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected override ValidationResult IsValid
                  (object value, ValidationContext validationContext)
        {
            var registering = (RegisterViewModel)validationContext.ObjectInstance;
            var pending = db.PendingRegistrations.SingleOrDefault(p => p.Email == registering.Email);
            if (pending != null)
                return ValidationResult.Success;
            return new ValidationResult("Please contact Admin for registration details.");
        }

    }
  
}
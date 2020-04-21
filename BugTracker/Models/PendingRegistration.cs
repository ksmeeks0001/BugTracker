using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class PendingRegistration
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public int OrganizationId { get; set; }
        
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime DateCreated { get; set; }

        public int IssueId { get; set; }
        public Issue Issue { get; set; }

        public string PostedById { get; set; }
        [ForeignKey("PostedById")]
        public ApplicationUser PostedBy { get; set; }

    }
}
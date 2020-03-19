using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Issue
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public int Priority { get; set; }

        public bool Resolved { get; set; }

        [Display(Name="Date Added")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyy}")]
        public DateTime DateCreated { get; set; }

        [Display(Name="Date Updated")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyy}")]
        public DateTime? DateUpdated { get; set; }

        public List<Note> Notes { get; set; }


        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public ApplicationUser CreatedBy { get; set; }

        public string DeveloperAssignedId { get; set; }
        [ForeignKey("DeveloperAssignedId")]
        public ApplicationUser DeveloperAssigned { get; set; }
    }
}
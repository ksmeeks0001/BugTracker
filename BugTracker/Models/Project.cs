using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Required]
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }

        public string ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public ApplicationUser Manager { get; set; }

        public List<ProjectDeveloper> ProjectDevelopers { get; set; }
        public List<Issue> Issues { get; set; }

        public bool Complete { get; set; }

        [Required]
        public string Details { get; set; }

        [Display(Name = "Date Started")]
        public DateTime DateStarted { get; set; }
    }

    public class ProjectDeveloper
    {
        [Key, Column(Order = 1)]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Key, Column(Order = 2)]
        public string DeveloperId { get; set; }
        public ApplicationUser Developer { get; set; }
    }
}
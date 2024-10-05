using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace TeamTracker.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Display(Name = "Department Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters long", MinimumLength = 2)]
        public required string Name { get; set; }
       

        // Navigation Properties for Many-to-Many Relationships
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
       public virtual ICollection<Location> Locations { get; set; } = new HashSet<Location>();
     

    }
}

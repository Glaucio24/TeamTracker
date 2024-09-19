using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamTracker.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Display(Name = "Location Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters long", MinimumLength = 2)]
        public required string Name { get; set; }

        [Display(Name = "Address")]
        public required string Address { get; set; }

        // Navigation Properties
        public virtual ICollection<Department> Departments { get; set; } = new HashSet<Department>();
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}

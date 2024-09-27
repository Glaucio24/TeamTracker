using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamTracker.Enums;

namespace TeamTracker.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters long", MinimumLength = 2)]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters long", MinimumLength = 2)]
        public required string LastName { get; set; }

        public required string Email { get; set; }

        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }

        public EmploymentStatus Status { get; set; } // For tracking employement status

        [Display(Name = "Hired Date")]
        public DateTime HireDate { get; set; } = DateTime.Now;

        public byte[]? ImageData { get; set; }

        public string? ImageType { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}"; // This property will not be mapped to the database
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Navigation Properties for Many-to-Many Relationships
        public virtual ICollection<Location> Locations { get; set; } = new HashSet<Location>();
        public virtual ICollection<Department> Departments { get; set; } = new HashSet<Department>();
    }
}

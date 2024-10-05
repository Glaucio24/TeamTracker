using System.Collections.Generic;
using Humanizer;

namespace TeamTracker.ModelView
{
    public class Dashboard
    {
        // Properties to include Total 
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalLocations { get; set; }

        // Properties to include charts data      
        public List<string> DepartmentNames { get; set; } = new List<string>();
        public List<int> EmployeesPerDepartment { get; set; } = new List<int>();

        public List<string> LocationNames { get; set; } = new List<string>();
        public List<int> EmployeesPerLocation { get; set; } = new List<int>();

        // Method to add a department and its employee count
        public void AddDepartment(string departmentName, int employeeCount)
        {
            if (!string.IsNullOrWhiteSpace(departmentName) && employeeCount >= 0)
            {
                DepartmentNames.Add(departmentName);
                EmployeesPerDepartment.Add(employeeCount);
                TotalDepartments++; // Increment total departments count
                TotalEmployees += employeeCount; // Update total employees count
            }
        }

        // Method to add a location and its employee count
        public void AddLocation(string locationName, int employeeCount)
        {
            if (!string.IsNullOrWhiteSpace(locationName) && employeeCount >= 0)
            {
                LocationNames.Add(locationName);
                EmployeesPerLocation.Add(employeeCount);
                TotalLocations++; // Increment total locations count
                TotalEmployees += employeeCount; // Update total employees count
            }
        }

        // Optional: Method to get total employees by department name
        public int GetEmployeesByDepartment(string departmentName)
        {
            var index = DepartmentNames.IndexOf(departmentName);
            return index >= 0 ? EmployeesPerDepartment[index] : 0;
        }

        // Optional: Method to get total employees by location name
        public int GetEmployeesByLocation(string locationName)
        {
            var index = LocationNames.IndexOf(locationName);
            return index >= 0 ? EmployeesPerLocation[index] : 0;
        }
    }
}

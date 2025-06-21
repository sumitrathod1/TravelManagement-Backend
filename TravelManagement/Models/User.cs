    using System.ComponentModel.DataAnnotations;

    namespace TravelManagement.Models
    {
        public enum Licecnce 
        {
            LMVC,
            Badge,
            HeavyBadge
        }
        public enum Role 
        {
            Employee,
            Admin
        }
        public class User
        {
            [Key]
            public int userId { get; set; }
            public required string EmployeeName { get; set; }
            public string UserName { get; set; }
            public DateOnly? EmployeeDOB { get; set; }
            public string? Address { get; set; }
            public Role Role { get; set; }
            public Licecnce? Licence { get; set; }

            [EmailAddress]
            public string? Email { get; set; }

            [Required]
            public required string Password { get; set; }
            [Required]
            public int Number { get; set; }
            public decimal Salary { get; set; }
            public string? ResetPasswordtoken { get; set; }
            public DateTime RestPasswordExpiry { get; set; }
            public DateTime? RenewalMailSentDate { get; set; }
            public bool Status { get; set; }
            public int EmployeAge { get; set; } 
        }
    }

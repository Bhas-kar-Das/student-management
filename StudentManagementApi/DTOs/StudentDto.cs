using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.DTOs
{
    // DTO for creating a new student
    public class CreateStudentDto
    {
        // Student name - required, max 100 chars
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        // Email - required, valid email format
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        // Age - required, must be positive
        [Required(ErrorMessage = "Age is required")]
        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150")]
        public int Age { get; set; }

        // Course - required, max 100 chars
        [Required(ErrorMessage = "Course is required")]
        [MaxLength(100, ErrorMessage = "Course cannot exceed 100 characters")]
        public string Course { get; set; } = string.Empty;
    }

    // DTO for updating an existing student
    public class UpdateStudentDto
    {
        // Student ID - required for update
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        // Student name - required, max 100 chars
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        // Email - required, valid email format
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        // Age - required, must be positive
        [Required(ErrorMessage = "Age is required")]
        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150")]
        public int Age { get; set; }

        // Course - required, max 100 chars
        [Required(ErrorMessage = "Course is required")]
        [MaxLength(100, ErrorMessage = "Course cannot exceed 100 characters")]
        public string Course { get; set; } = string.Empty;
    }

    // DTO for student response (includes ID and CreatedDate)
    public class StudentResponseDto
    {
        // Student unique identifier
        public int Id { get; set; }

        // Student name
        public string Name { get; set; } = string.Empty;

        // Email address
        public string Email { get; set; } = string.Empty;

        // Age
        public int Age { get; set; }

        // Course enrolled
        public string Course { get; set; } = string.Empty;

        // Record creation timestamp
        public DateTime CreatedDate { get; set; }
    }
}

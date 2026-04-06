using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementApi.Models
{
    // Student entity representing the Students table in database
    [Table("Students")]
    public class Student
    {
        // Primary key - auto-incremented
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Student name (required, max 100 chars)
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Email address (required, unique)
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Age (required, must be positive)
        [Required]
        [Range(1, 150)]
        public int Age { get; set; }

        // Course name (required, max 100 chars)
        [Required]
        [MaxLength(100)]
        public string Course { get; set; } = string.Empty;

        // Creation timestamp
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

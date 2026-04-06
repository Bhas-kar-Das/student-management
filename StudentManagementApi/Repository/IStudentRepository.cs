using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Models;

namespace StudentManagementApi.Repository
{
    // Repository interface for Student operations
    public interface IStudentRepository
    {
        // Get all students
        Task<IEnumerable<Student>> GetAllStudentsAsync();

        // Get student by ID
        Task<Student?> GetStudentByIdAsync(int id);

        // Get student by email
        Task<Student?> GetStudentByEmailAsync(string email);

        // Add new student
        Task<Student> AddStudentAsync(Student student);

        // Update existing student
        Task<Student> UpdateStudentAsync(Student student);

        // Delete student by ID
        Task<bool> DeleteStudentAsync(int id);

        // Check if student exists by ID
        Task<bool> StudentExistsAsync(int id);

        // Check if email already exists (for create/update validation)
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }

    // Repository implementation for Student entity
    public class StudentRepository : IStudentRepository
    {
        // Database context instance
        private readonly ApplicationDbContext _context;

        // Constructor with dependency injection
        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all students from database
        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        // Get student by ID
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        // Get student by email address
        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
        }

        // Add new student to database
        public async Task<Student> AddStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        // Update existing student
        public async Task<Student> UpdateStudentAsync(Student student)
        {
            var existingStudent = await _context.Students.FindAsync(student.Id);
            if (existingStudent == null)
            {
                throw new InvalidOperationException($"Student with ID {student.Id} not found");
            }

            // Update properties
            existingStudent.Name = student.Name;
            existingStudent.Email = student.Email;
            existingStudent.Age = student.Age;
            existingStudent.Course = student.Course;

            await _context.SaveChangesAsync();
            return existingStudent;
        }

        // Delete student by ID
        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if student exists by ID
        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id);
        }

        // Check if email already exists (excluding current student for updates)
        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Students.Where(s => s.Email.ToLower() == email.ToLower());
            
            // Exclude the current student when checking for updates
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}

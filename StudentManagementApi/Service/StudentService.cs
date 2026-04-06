using Microsoft.Extensions.Logging;
using StudentManagementApi.DTOs;
using StudentManagementApi.Models;
using StudentManagementApi.Repository;

namespace StudentManagementApi.Service
{
    // Service interface for student business logic
    public interface IStudentService
    {
        // Get all students
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync();

        // Get student by ID
        Task<StudentResponseDto?> GetStudentByIdAsync(int id);

        // Create new student
        Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto createDto);

        // Update existing student
        Task<StudentResponseDto> UpdateStudentAsync(UpdateStudentDto updateDto);

        // Delete student by ID
        Task<bool> DeleteStudentAsync(int id);
    }

    // Service implementation with business logic
    public class StudentService : IStudentService
    {
        // Repository instance for data access
        private readonly IStudentRepository _repository;

        // Logger instance for logging
        private readonly ILogger<StudentService> _logger;

        // Constructor with dependency injection
        public StudentService(IStudentRepository repository, ILogger<StudentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Get all students - converts entities to DTOs
        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync()
        {
            _logger.LogInformation("Fetching all students");

            var students = await _repository.GetAllStudentsAsync();
            return students.Select(MapToDto);
        }

        // Get student by ID with error handling
        public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
        {
            _logger.LogInformation("Fetching student with ID: {Id}", id);

            var student = await _repository.GetStudentByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("Student with ID {Id} not found", id);
                return null;
            }

            return MapToDto(student);
        }

        // Create new student with validation
        public async Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto createDto)
        {
            _logger.LogInformation("Creating new student with email: {Email}", createDto.Email);

            // Check if email already exists
            if (await _repository.EmailExistsAsync(createDto.Email))
            {
                _logger.LogWarning("Email {Email} already exists", createDto.Email);
                throw new InvalidOperationException($"Student with email {createDto.Email} already exists");
            }

            // Map DTO to entity
            var student = new Student
            {
                Name = createDto.Name,
                Email = createDto.Email,
                Age = createDto.Age,
                Course = createDto.Course,
                CreatedDate = DateTime.Now
            };

            // Save to database
            var createdStudent = await _repository.AddStudentAsync(student);
            _logger.LogInformation("Student created successfully with ID: {Id}", createdStudent.Id);

            return MapToDto(createdStudent);
        }

        // Update existing student
        public async Task<StudentResponseDto> UpdateStudentAsync(UpdateStudentDto updateDto)
        {
            _logger.LogInformation("Updating student with ID: {Id}", updateDto.Id);

            // Check if student exists
            if (!await _repository.StudentExistsAsync(updateDto.Id))
            {
                _logger.LogWarning("Student with ID {Id} not found", updateDto.Id);
                throw new InvalidOperationException($"Student with ID {updateDto.Id} not found");
            }

            // Check if email already exists (excluding current student)
            if (await _repository.EmailExistsAsync(updateDto.Email, updateDto.Id))
            {
                _logger.LogWarning("Email {Email} already exists", updateDto.Email);
                throw new InvalidOperationException($"Email {updateDto.Email} is already in use");
            }

            // Map DTO to entity
            var student = new Student
            {
                Id = updateDto.Id,
                Name = updateDto.Name,
                Email = updateDto.Email,
                Age = updateDto.Age,
                Course = updateDto.Course
            };

            // Update in database
            var updatedStudent = await _repository.UpdateStudentAsync(student);
            _logger.LogInformation("Student updated successfully with ID: {Id}", updatedStudent.Id);

            return MapToDto(updatedStudent);
        }

        // Delete student by ID
        public async Task<bool> DeleteStudentAsync(int id)
        {
            _logger.LogInformation("Deleting student with ID: {Id}", id);

            var result = await _repository.DeleteStudentAsync(id);
            if (result)
            {
                _logger.LogInformation("Student with ID {Id} deleted successfully", id);
            }
            else
            {
                _logger.LogWarning("Student with ID {Id} not found for deletion", id);
            }

            return result;
        }

        // Helper method to map Student entity to StudentResponseDto
        private static StudentResponseDto MapToDto(Student student)
        {
            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Age = student.Age,
                Course = student.Course,
                CreatedDate = student.CreatedDate
            };
        }
    }
}

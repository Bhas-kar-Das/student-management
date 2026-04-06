using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.DTOs;
using StudentManagementApi.Service;

namespace StudentManagementApi.Controller
{
    // API Controller for Student operations
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StudentsController : ControllerBase
    {
        // Service instance for business logic
        private readonly IStudentService _studentService;

        // Logger instance
        private readonly ILogger<StudentsController> _logger;

        // Constructor with dependency injection
        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        // =====================================================
        // GET: api/students
        // Get all students - requires JWT authentication
        // =====================================================
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<StudentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetStudents()
        {
            _logger.LogInformation("GET request received for all students");

            try
            {
                var students = await _studentService.GetAllStudentsAsync();
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all students");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error retrieving students", error = ex.Message });
            }
        }

        // =====================================================
        // GET: api/students/{id}
        // Get student by ID - requires JWT authentication
        // =====================================================
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentResponseDto>> GetStudent(int id)
        {
            _logger.LogInformation("GET request received for student with ID: {Id}", id);

            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);
                if (student == null)
                {
                    _logger.LogWarning("Student with ID {Id} not found", id);
                    return NotFound(new { message = $"Student with ID {id} not found" });
                }

                return Ok(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error retrieving student", error = ex.Message });
            }
        }

        // =====================================================
        // POST: api/students
        // Create new student - requires JWT authentication
        // =====================================================
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentResponseDto>> CreateStudent([FromBody] CreateStudentDto createDto)
        {
            _logger.LogInformation("POST request received to create new student");

            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for create student request");
                    return BadRequest(ModelState);
                }

                var createdStudent = await _studentService.CreateStudentAsync(createDto);
                
                _logger.LogInformation("Student created successfully with ID: {Id}", createdStudent.Id);
                return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.Id }, createdStudent);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error creating student: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new student");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error creating student", error = ex.Message });
            }
        }

        // =====================================================
        // PUT: api/students
        // Update existing student - requires JWT authentication
        // =====================================================
        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentResponseDto>> UpdateStudent([FromBody] UpdateStudentDto updateDto)
        {
            _logger.LogInformation("PUT request received to update student with ID: {Id}", updateDto.Id);

            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for update student request");
                    return BadRequest(ModelState);
                }

                var updatedStudent = await _studentService.UpdateStudentAsync(updateDto);
                
                _logger.LogInformation("Student updated successfully with ID: {Id}", updatedStudent.Id);
                return Ok(updatedStudent);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating student: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student with ID: {Id}", updateDto.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error updating student", error = ex.Message });
            }
        }

        // =====================================================
        // DELETE: api/students/{id}
        // Delete student by ID - requires JWT authentication
        // =====================================================
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            _logger.LogInformation("DELETE request received for student with ID: {Id}", id);

            try
            {
                var result = await _studentService.DeleteStudentAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Student with ID {Id} not found for deletion", id);
                    return NotFound(new { message = $"Student with ID {id} not found" });
                }

                _logger.LogInformation("Student with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error deleting student", error = ex.Message });
            }
        }
    }
}

import React, { useState, useEffect } from 'react';
import { studentApi, authApi } from '../services/api';
import { Student, CreateStudentDto, UpdateStudentDto } from '../types/student';
import './StudentList.css';

const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

const StudentList: React.FC = () => {
  const [students, setStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingStudent, setEditingStudent] = useState<Student | null>(null);
  const [formData, setFormData] = useState<CreateStudentDto>({
    name: '',
    email: '',
    age: 0,
    course: '',
  });
  const [formErrors, setFormErrors] = useState<{ name?: string; email?: string; age?: string; course?: string }>({});

  // Load students on component mount
  useEffect(() => {
    loadStudents();
  }, []);

  // Fetch all students from API
  const loadStudents = async () => {
    try {
      setLoading(true);
      const data = await studentApi.getAllStudents();
      setStudents(data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load students');
    } finally {
      setLoading(false);
    }
  };

  // Handle logout
  const handleLogout = () => {
    authApi.logout();
    window.location.href = '/login';
  };

  // Open modal for creating new student
  const handleAddNew = () => {
    setEditingStudent(null);
    setFormData({ name: '', email: '', age: 0, course: '' });
    setFormErrors({});
    setShowModal(true);
  };

  // Open modal for editing existing student
  const handleEdit = (student: Student) => {
    setEditingStudent(student);
    setFormData({
      name: student.name,
      email: student.email,
      age: student.age,
      course: student.course,
    });
    setFormErrors({});
    setShowModal(true);
  };

  // Handle form input changes
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'age' ? parseInt(value) || 0 : value,
    }));

    if (name === 'email') {
      if (value && !validateEmail(value)) {
        setFormErrors((prev) => ({ ...prev, email: 'Please enter a valid email address' }));
      } else {
        setFormErrors((prev) => ({ ...prev, email: undefined }));
      }
    }
  };

  // Validate form before submit
  const validateForm = (): boolean => {
    const newErrors: typeof formErrors = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Name is required';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!validateEmail(formData.email)) {
      newErrors.email = 'Please enter a valid email address';
    }

    if (!formData.age || formData.age < 1) {
      newErrors.age = 'Age must be at least 1';
    }

    if (!formData.course.trim()) {
      newErrors.course = 'Course is required';
    }

    setFormErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Submit form - create or update student
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      if (editingStudent) {
        const updateData: UpdateStudentDto = {
          id: editingStudent.id,
          ...formData,
        };
        await studentApi.updateStudent(updateData);
      } else {
        await studentApi.createStudent(formData);
      }
      setShowModal(false);
      setFormErrors({});
      loadStudents();
    } catch (err: any) {
      const message = err.response?.data?.message || '';
      if (message.toLowerCase().includes('email') && message.toLowerCase().includes('already exists')) {
        setFormErrors((prev) => ({ ...prev, email: message }));
      } else {
        setError(message || 'Failed to save student');
      }
    }
  };

  // Delete student
  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this student?')) {
      try {
        await studentApi.deleteStudent(id);
        loadStudents();
      } catch (err: any) {
        setError(err.response?.data?.message || 'Failed to delete student');
      }
    }
  };

  return (
    <div className="student-list-container">
      <header className="header">
        <h1>Student Management System</h1>
        <button className="logout-btn" onClick={handleLogout}>
          Logout
        </button>
      </header>

      <div className="content">
        <div className="toolbar">
          <h2>Students</h2>
          <button className="add-btn" onClick={handleAddNew}>
            + Add Student
          </button>
        </div>

        {error && <div className="error-message">{error}</div>}

        {loading ? (
          <div className="loading">Loading...</div>
        ) : students.length === 0 ? (
          <div className="empty-message">No students found. Add your first student!</div>
        ) : (
          <table className="student-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Email</th>
                <th>Age</th>
                <th>Course</th>
                <th>Created Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id}>
                  <td>{student.id}</td>
                  <td>{student.name}</td>
                  <td>{student.email}</td>
                  <td>{student.age}</td>
                  <td>{student.course}</td>
                  <td>{new Date(student.createdDate).toLocaleDateString()}</td>
                  <td>
                    <button
                      className="edit-btn"
                      onClick={() => handleEdit(student)}
                    >
                      Edit
                    </button>
                    <button
                      className="delete-btn"
                      onClick={() => handleDelete(student.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Modal for Add/Edit Student */}
      {showModal && (
        <div className="modal-overlay">
          <div className="modal">
            <h3>{editingStudent ? 'Edit Student' : 'Add New Student'}</h3>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Name</label>
                <div className="input-with-icon">
                  <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                    <circle cx="12" cy="7" r="4"></circle>
                  </svg>
                  <input
                    type="text"
                    name="name"
                    value={formData.name}
                    onChange={handleInputChange}
                    required
                    placeholder="Enter name"
                  />
                </div>
                {formErrors.name && <span className="field-error">{formErrors.name}</span>}
              </div>
              <div className="form-group">
                <label>Email</label>
                <div className="input-with-icon">
                  <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
                    <polyline points="22,6 12,13 2,6"></polyline>
                  </svg>
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleInputChange}
                    required
                    placeholder="Enter email"
                  />
                </div>
                {formErrors.email && <span className="field-error">{formErrors.email}</span>}
              </div>
              <div className="form-group">
                <label>Age</label>
                <div className="input-with-icon">
                  <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z"></path>
                    <path d="M12 6v6l4 2"></path>
                  </svg>
                  <input
                    type="number"
                    name="age"
                    value={formData.age}
                    onChange={handleInputChange}
                    required
                    min="1"
                    placeholder="Enter age"
                  />
                </div>
                {formErrors.age && <span className="field-error">{formErrors.age}</span>}
              </div>
              <div className="form-group">
                <label>Course</label>
                <div className="input-with-icon">
                  <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                    <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path>
                  </svg>
                  <input
                    type="text"
                    name="course"
                    value={formData.course}
                    onChange={handleInputChange}
                    required
                    placeholder="Enter course"
                  />
                </div>
                {formErrors.course && <span className="field-error">{formErrors.course}</span>}
              </div>
              <div className="modal-actions">
                <button type="submit" className="save-btn">
                  {editingStudent ? 'Update' : 'Create'}
                </button>
                <button
                  type="button"
                  className="cancel-btn"
                  onClick={() => setShowModal(false)}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default StudentList;

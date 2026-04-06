import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { studentApi } from '../services/api';
import { CreateStudentDto } from '../types/student';
import './NewStudent.css';

const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

const NewStudent: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<CreateStudentDto>({
    name: '',
    email: '',
    age: 0,
    course: '',
  });
  const [errors, setErrors] = useState<{ name?: string; email?: string; age?: string; course?: string }>({});
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'age' ? parseInt(value) || 0 : value,
    }));
    
    if (name === 'email') {
      if (value && !validateEmail(value)) {
        setErrors((prev) => ({ ...prev, email: 'Please enter a valid email address' }));
      } else {
        setErrors((prev) => ({ ...prev, email: undefined }));
      }
    }
  };

  const validateForm = (): boolean => {
    const newErrors: typeof errors = {};

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

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      await studentApi.createStudent(formData);
      navigate('/students');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create student');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="new-student-container">
      <header className="header">
        <h1>Student Management System</h1>
      </header>

      <div className="content">
        <div className="form-card">
          <h2>Add New Student</h2>

          {error && <div className="error-message">{error}</div>}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="name">Name</label>
              <div className="input-with-icon">
                <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                  <circle cx="12" cy="7" r="4"></circle>
                </svg>
                <input
                  type="text"
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  placeholder="Enter student name"
                />
              </div>
              {errors.name && <span className="field-error">{errors.name}</span>}
            </div>

            <div className="form-group">
              <label htmlFor="email">Email</label>
              <div className="input-with-icon">
                <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
                  <polyline points="22,6 12,13 2,6"></polyline>
                </svg>
                <input
                  type="email"
                  id="email"
                  name="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  placeholder="Enter student email"
                />
              </div>
              {errors.email && <span className="field-error">{errors.email}</span>}
            </div>

            <div className="form-group">
              <label htmlFor="age">Age</label>
              <div className="input-with-icon">
                <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z"></path>
                  <path d="M12 6v6l4 2"></path>
                </svg>
                <input
                  type="number"
                  id="age"
                  name="age"
                  value={formData.age}
                  onChange={handleInputChange}
                  placeholder="Enter student age"
                  min="1"
                />
              </div>
              {errors.age && <span className="field-error">{errors.age}</span>}
            </div>

            <div className="form-group">
              <label htmlFor="course">Course</label>
              <div className="input-with-icon">
                <svg className="input-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                  <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path>
                </svg>
                <input
                  type="text"
                  id="course"
                  name="course"
                  value={formData.course}
                  onChange={handleInputChange}
                  placeholder="Enter course name"
                />
              </div>
              {errors.course && <span className="field-error">{errors.course}</span>}
            </div>

            <div className="form-actions">
              <button type="submit" disabled={isLoading}>
                {isLoading ? 'Creating...' : 'Create Student'}
              </button>
              <button
                type="button"
                className="cancel-btn"
                onClick={() => navigate('/students')}
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default NewStudent;
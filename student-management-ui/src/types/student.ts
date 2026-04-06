// Student type definition matching the API DTOs
export interface Student {
  id: number;
  name: string;
  email: string;
  age: number;
  course: string;
  createdDate: string;
}

// DTO for creating a new student
export interface CreateStudentDto {
  name: string;
  email: string;
  age: number;
  course: string;
}

// DTO for updating a student
export interface UpdateStudentDto {
  id: number;
  name: string;
  email: string;
  age: number;
  course: string;
}

// Login request type
export interface LoginRequest {
  username: string;
  password: string;
}

// Login response type
export interface LoginResponse {
  token: string;
  username: string;
  expiresIn: number;
}

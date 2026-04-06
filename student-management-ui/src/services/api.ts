import axios, { AxiosInstance, AxiosError } from 'axios';
import { Student, CreateStudentDto, UpdateStudentDto, LoginRequest, LoginResponse } from '../types/student';

// API base URL - change this to match your API URL
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

// Store token in localStorage
const getToken = (): string | null => localStorage.getItem('jwtToken');
const setToken = (token: string): void => localStorage.setItem('jwtToken', token);
const removeToken = (): void => localStorage.removeItem('jwtToken');

// Create axios instance with default config
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests if available
apiClient.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Handle response errors globally
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      removeToken();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// =====================================================
// Authentication API Calls
// =====================================================

export const authApi = {
  // Login user and get JWT token
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await apiClient.post<LoginResponse>('/auth/login', credentials);
    if (response.data.token) {
      setToken(response.data.token);
    }
    return response.data;
  },

  // Logout user and remove token
  logout: (): void => {
    removeToken();
  },

  // Check if user is logged in
  isLoggedIn: (): boolean => {
    return !!getToken();
  },
};

// =====================================================
// Student API Calls
// =====================================================

export const studentApi = {
  // Get all students
  getAllStudents: async (): Promise<Student[]> => {
    const response = await apiClient.get<Student[]>('/students');
    return response.data;
  },

  // Get student by ID
  getStudentById: async (id: number): Promise<Student> => {
    const response = await apiClient.get<Student>(`/students/${id}`);
    return response.data;
  },

  // Create new student
  createStudent: async (student: CreateStudentDto): Promise<Student> => {
    const response = await apiClient.post<Student>('/students', student);
    return response.data;
  },

  // Update existing student
  updateStudent: async (student: UpdateStudentDto): Promise<Student> => {
    const response = await apiClient.put<Student>('/students', student);
    return response.data;
  },

  // Delete student by ID
  deleteStudent: async (id: number): Promise<void> => {
    await apiClient.delete(`/students/${id}`);
  },
};

export default apiClient;

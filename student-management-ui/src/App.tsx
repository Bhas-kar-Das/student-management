import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import StudentList from './components/StudentList';
import NewStudent from './components/NewStudent';
import { authApi } from './services/api';

// Protected Route component - redirects to login if not authenticated
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  if (!authApi.isLoggedIn()) {
    return <Navigate to="/login" replace />;
  }
  return <>{children}</>;
};

function App() {
  return (
    <Router>
      <Routes>
        {/* Public route - Login page */}
        <Route path="/login" element={<Login />} />
        
        {/* Protected route - Student list page */}
        <Route
          path="/students"
          element={
            <ProtectedRoute>
              <StudentList />
            </ProtectedRoute>
          }
        />
        
        {/* Protected route - Add new student page */}
        <Route
          path="/students/new"
          element={
            <ProtectedRoute>
              <NewStudent />
            </ProtectedRoute>
          }
        />
        
        {/* Default redirect to login */}
        <Route path="/" element={<Navigate to="/login" replace />} />
        
        {/* Catch all - redirect to login */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </Router>
  );
}

export default App;

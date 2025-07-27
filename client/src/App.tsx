import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LandingPage from './components/pages/LandingPage';
import LoginPage from './components/pages/LoginPage';
import RegisterPage from './components/pages/RegisterPage';
import Dashboard from './components/pages/Dashboard';
import ProfilePage from './components/pages/ProfilePage';
import AdminPage from './components/pages/AdminPage';
import AuthDebug from './components/AuthDebug';
import ApiTest from './components/ApiTest';
import { ProtectedRoute, PublicRoute, AdminRoute } from './components/RouteGuards';

function App() {
  // Show debug components if ?debug=true is in URL
  const showDebug = new URLSearchParams(window.location.search).get('debug') === 'true';
  
  return (
    <div className="w-full">
      {showDebug && (
        <>
          <AuthDebug />
          <ApiTest />
        </>
      )}
      
      <Router>
        <Routes>
          {/* Public Routes */}
          <Route 
            path="/" 
            element={
              <PublicRoute>
                <LandingPage />
              </PublicRoute>
            } 
          />
          <Route 
            path="/login" 
            element={
              <PublicRoute>
                <LoginPage />
              </PublicRoute>
            } 
          />
          <Route 
            path="/register" 
            element={
              <PublicRoute>
                <RegisterPage />
              </PublicRoute>
            } 
          />
          
          {/* Protected Routes */}
          <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/profile" 
            element={
              <ProtectedRoute>
                <ProfilePage />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/admin" 
            element={
              <AdminRoute>
                <AdminPage />
              </AdminRoute>
            } 
          />
          
          {/* Catch all - redirect to appropriate page */}
          <Route 
            path="*" 
            element={<Navigate to="/" replace />} 
          />
        </Routes>
      </Router>
    </div>
  );
}

export default App;

import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LandingPage from './components/pages/LandingPage';
import LoginPage from './components/pages/LoginPage';
import RegisterPage from './components/pages/RegisterPage';
import Dashboard from './components/pages/Dashboard';
import ProfilePage from './components/pages/ProfilePage';
import ProjectsPage from './components/pages/ProjectsPage';
import AdminPage from './components/pages/AdminPage';
import AuthDebug from './components/AuthDebug';
import ApiTest from './components/ApiTest';
import Layout from './components/Layout';
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
          {/* Public Routes - No Layout */}
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
          
          {/* Protected Routes - With Layout */}
          <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <Layout>
                  <Dashboard />
                </Layout>
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/profile" 
            element={
              <ProtectedRoute>
                <Layout>
                  <ProfilePage />
                </Layout>
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/projects" 
            element={
              <ProtectedRoute>
                <Layout>
                  <ProjectsPage />
                </Layout>
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/admin" 
            element={
              <AdminRoute>
                <Layout>
                  <AdminPage />
                </Layout>
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

import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

// Loading component for auth checks
const AuthLoadingSpinner: React.FC = () => (
  <div className="min-h-screen bg-gray-50 flex items-center justify-center">
    <div className="text-center">
      <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto"></div>
      <p className="mt-4 text-gray-600">Verifying your authentication...</p>
    </div>
  </div>
);

// Protected Route Component - requires authentication
export const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { state } = useAuth();
  const location = useLocation();
  
  // Show loading while checking authentication
  if (state.isLoading) {
    return <AuthLoadingSpinner />;
  }
  
  // If not authenticated, redirect to login with return URL
  if (!state.isAuthenticated) {
    return (
      <Navigate 
        to="/login" 
        state={{ from: location.pathname }} 
        replace 
      />
    );
  }
  
  // User is authenticated, render the protected content
  return <>{children}</>;
};

// Public Route Component - redirects to dashboard if already logged in
export const PublicRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { state } = useAuth();
  const location = useLocation();
  
  // Show loading while checking authentication
  if (state.isLoading) {
    return <AuthLoadingSpinner />;
  }
  
  // If authenticated, redirect to dashboard or intended destination
  if (state.isAuthenticated) {
    // Check if user was redirected here from a protected route
    const from = location.state?.from || '/dashboard';
    return <Navigate to={from} replace />;
  }
  
  // User is not authenticated, render the public content
  return <>{children}</>;
};

// Admin Route Component - for future admin-only pages
export const AdminRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { state } = useAuth();
  const location = useLocation();
  
  // Show loading while checking authentication
  if (state.isLoading) {
    return <AuthLoadingSpinner />;
  }
  
  // If not authenticated, redirect to login
  if (!state.isAuthenticated) {
    return (
      <Navigate 
        to="/login" 
        state={{ from: location.pathname }} 
        replace 
      />
    );
  }
  
  // Check if user has admin privileges (placeholder for future implementation)
  const isAdmin = state.user?.jobTitle?.toLowerCase().includes('admin') || 
                  state.user?.jobTitle?.toLowerCase().includes('partner');
  
  if (!isAdmin) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="max-w-md mx-auto text-center">
          <div className="bg-red-100 border border-red-400 text-red-700 px-6 py-8 rounded-lg shadow-lg">
            <div className="text-6xl mb-4">🚫</div>
            <h2 className="text-2xl font-bold mb-4">Access Denied</h2>
            <p className="mb-6">You don't have permission to access this page.</p>
            <p className="text-sm mb-6">
              Admin access requires a job title containing "admin" or "partner".<br/>
              Your current job title: <span className="font-mono bg-red-200 px-2 py-1 rounded">
                {state.user?.jobTitle || 'None'}
              </span>
            </p>
            <button
              onClick={() => window.history.back()}
              className="bg-red-600 hover:bg-red-700 text-white px-6 py-2 rounded-md font-medium mr-3"
            >
              Go Back
            </button>
            <button
              onClick={() => window.location.href = '/dashboard'}
              className="bg-gray-600 hover:bg-gray-700 text-white px-6 py-2 rounded-md font-medium"
            >
              Dashboard
            </button>
          </div>
        </div>
      </div>
    );
  }
  
  // User is authenticated and has admin privileges
  return <>{children}</>;
}; 
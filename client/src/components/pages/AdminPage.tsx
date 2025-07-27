import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';

const AdminPage: React.FC = () => {
  const { state, logout } = useAuth();

  if (!state.user) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900">Loading...</h2>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <div className="flex items-center space-x-4">
              <Link to="/dashboard" className="text-indigo-600 hover:text-indigo-500">
                ← Back to Dashboard
              </Link>
              <h1 className="text-2xl font-bold text-red-600">🔐 Admin Portal</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">
                Admin: {state.user.firstName} {state.user.lastName}
              </span>
              <button
                onClick={logout}
                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white overflow-hidden shadow rounded-lg">
            <div className="px-4 py-5 sm:p-6">
              <div className="text-center mb-8">
                <h2 className="text-3xl font-bold text-gray-900 mb-4">
                  🛡️ Admin Dashboard
                </h2>
                <p className="text-lg text-gray-600">
                  Welcome to the admin area! Only authorized users can access this page.
                </p>
              </div>

              {/* Admin Features */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
                  <div className="text-3xl mb-2">👥</div>
                  <h3 className="font-medium text-gray-900">User Management</h3>
                  <p className="text-sm text-gray-500 mt-1">
                    Manage user accounts and permissions
                  </p>
                </div>
                <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
                  <div className="text-3xl mb-2">📊</div>
                  <h3 className="font-medium text-gray-900">Analytics</h3>
                  <p className="text-sm text-gray-500 mt-1">
                    View system usage and statistics
                  </p>
                </div>
                <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
                  <div className="text-3xl mb-2">⚙️</div>
                  <h3 className="font-medium text-gray-900">System Settings</h3>
                  <p className="text-sm text-gray-500 mt-1">
                    Configure application settings
                  </p>
                </div>
              </div>

              {/* Current User Info */}
              <div className="mt-8 bg-gray-50 rounded-lg p-6">
                <h3 className="text-lg font-medium text-gray-900 mb-4">
                  Your Admin Privileges
                </h3>
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <span className="font-medium">Name:</span> {state.user.fullName}
                  </div>
                  <div>
                    <span className="font-medium">Email:</span> {state.user.email}
                  </div>
                  <div>
                    <span className="font-medium">Job Title:</span> {state.user.jobTitle || 'Not specified'}
                  </div>
                  <div>
                    <span className="font-medium">Company:</span> {state.user.companyName || 'Not specified'}
                  </div>
                </div>
                <div className="mt-4 p-3 bg-blue-50 border border-blue-200 rounded">
                  <p className="text-sm text-blue-700">
                    <strong>Admin Access:</strong> You have admin privileges because your job title contains "admin" or "partner".
                    Current job title: <span className="font-mono">{state.user.jobTitle || 'None'}</span>
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminPage; 
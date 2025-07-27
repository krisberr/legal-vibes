import React from 'react';
import { useAuth } from '../../contexts/AuthContext';

const AdminPage: React.FC = () => {
  const { state } = useAuth();

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
    <>
      {/* Main Content */}
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          {/* Page Header */}
          <div className="mb-8">
            <h2 className="text-2xl font-bold leading-7 text-red-600 sm:text-3xl sm:truncate">
              🔐 Admin Portal
            </h2>
            <p className="mt-1 text-sm text-gray-500">
              Administrative tools and system management
            </p>
          </div>

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
    </>
  );
};

export default AdminPage; 
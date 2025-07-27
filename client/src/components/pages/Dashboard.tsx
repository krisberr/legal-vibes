import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';

const Dashboard: React.FC = () => {
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
            <div className="flex items-center">
              <h1 className="text-2xl font-bold text-blue-600">Legal Vibes</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">
                Welcome, {state.user.firstName}!
              </span>
              <Link
                to="/profile"
                className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                Profile
              </Link>
              <Link
                to="/admin"
                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                🔐 Admin
              </Link>
              <button
                onClick={logout}
                className="bg-gray-600 hover:bg-gray-700 text-white px-4 py-2 rounded-md text-sm font-medium"
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
          <div className="border-4 border-dashed border-gray-200 rounded-lg">
            <div className="p-8">
              {/* Welcome Message */}
              <div className="text-center mb-8">
                <h2 className="text-3xl font-bold text-gray-900 mb-4">
                  Welcome to Legal Vibes! 🎉
                </h2>
                <p className="text-lg text-gray-600">
                  Your AI-powered legal document assistant is ready to help.
                </p>
              </div>

              {/* User Info Card */}
              <div className="bg-white overflow-hidden shadow rounded-lg mb-8">
                <div className="px-4 py-5 sm:p-6">
                  <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
                    Your Profile
                  </h3>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <dt className="text-sm font-medium text-gray-500">Full Name</dt>
                      <dd className="mt-1 text-sm text-gray-900">{state.user.fullName}</dd>
                    </div>
                    <div>
                      <dt className="text-sm font-medium text-gray-500">Email</dt>
                      <dd className="mt-1 text-sm text-gray-900">{state.user.email}</dd>
                    </div>
                    {state.user.companyName && (
                      <div>
                        <dt className="text-sm font-medium text-gray-500">Company</dt>
                        <dd className="mt-1 text-sm text-gray-900">{state.user.companyName}</dd>
                      </div>
                    )}
                    {state.user.jobTitle && (
                      <div>
                        <dt className="text-sm font-medium text-gray-500">Job Title</dt>
                        <dd className="mt-1 text-sm text-gray-900">{state.user.jobTitle}</dd>
                      </div>
                    )}
                    {state.user.phoneNumber && (
                      <div>
                        <dt className="text-sm font-medium text-gray-500">Phone</dt>
                        <dd className="mt-1 text-sm text-gray-900">{state.user.phoneNumber}</dd>
                      </div>
                    )}
                    <div>
                      <dt className="text-sm font-medium text-gray-500">Member Since</dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {new Date(state.user.createdAt).toLocaleDateString()}
                      </dd>
                    </div>
                  </div>
                </div>
              </div>

              {/* Coming Soon Features */}
              <div className="bg-white overflow-hidden shadow rounded-lg">
                <div className="px-4 py-5 sm:p-6">
                  <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
                    Coming Soon
                  </h3>
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div className="text-center p-4 border-2 border-dashed border-gray-300 rounded-lg">
                      <div className="text-3xl mb-2">📁</div>
                      <h4 className="font-medium text-gray-900">Projects</h4>
                      <p className="text-sm text-gray-500 mt-1">
                        Manage your legal projects and cases
                      </p>
                    </div>
                    <div className="text-center p-4 border-2 border-dashed border-gray-300 rounded-lg">
                      <div className="text-3xl mb-2">📄</div>
                      <h4 className="font-medium text-gray-900">Documents</h4>
                      <p className="text-sm text-gray-500 mt-1">
                        AI-powered document generation
                      </p>
                    </div>
                    <div className="text-center p-4 border-2 border-dashed border-gray-300 rounded-lg">
                      <div className="text-3xl mb-2">🤖</div>
                      <h4 className="font-medium text-gray-900">AI Assistant</h4>
                      <p className="text-sm text-gray-500 mt-1">
                        Smart legal document drafting
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard; 
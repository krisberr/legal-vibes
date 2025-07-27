import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { healthCheck } from '../services/api';

const ApiTest: React.FC = () => {
  const { state, login, clearError } = useAuth();
  const [healthStatus, setHealthStatus] = useState<string>('');
  const [testCredentials, setTestCredentials] = useState({
    email: 'lawyer@example.com',
    password: 'SecurePass123!'
  });

  const testHealthCheck = async () => {
    try {
      const result = await healthCheck();
      setHealthStatus(`✅ API Connected: ${result}`);
    } catch (error) {
      setHealthStatus(`❌ API Error: ${error instanceof Error ? error.message : 'Unknown error'}`);
    }
  };

  const testLogin = async () => {
    try {
      await login(testCredentials.email, testCredentials.password);
    } catch (error) {
      // Error is already handled by the AuthContext
      console.log('Login failed:', error);
    }
  };

  return (
    <div className="bg-blue-50 p-4 rounded-lg border border-blue-200 mb-6">
      <h3 className="text-lg font-bold text-blue-800 mb-3">🧪 API Connection Test</h3>
      
      {/* Health Check */}
      <div className="mb-4">
        <button
          onClick={testHealthCheck}
          className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 mr-3"
        >
          Test API Health
        </button>
        {healthStatus && (
          <span className="text-sm">{healthStatus}</span>
        )}
      </div>

      {/* Login Test */}
      <div className="mb-4">
        <div className="mb-2">
          <label className="block text-sm font-medium text-blue-700">Test Email:</label>
          <input
            type="email"
            value={testCredentials.email}
            onChange={(e) => setTestCredentials({...testCredentials, email: e.target.value})}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md text-sm"
          />
        </div>
        <div className="mb-2">
          <label className="block text-sm font-medium text-blue-700">Test Password:</label>
          <input
            type="password"
            value={testCredentials.password}
            onChange={(e) => setTestCredentials({...testCredentials, password: e.target.value})}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md text-sm"
          />
        </div>
        <button
          onClick={testLogin}
          disabled={state.isLoading}
          className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600 disabled:bg-gray-400"
        >
          {state.isLoading ? 'Testing Login...' : 'Test Login'}
        </button>
        {state.error && (
          <div className="mt-2">
            <span className="text-red-600 text-sm">{state.error}</span>
            <button
              onClick={clearError}
              className="ml-2 px-2 py-1 bg-red-500 text-white rounded text-xs hover:bg-red-600"
            >
              Clear
            </button>
          </div>
        )}
      </div>

      <div className="text-xs text-blue-600">
        💡 Make sure your .NET API is running on localhost:7032 (HTTPS)
      </div>
    </div>
  );
};

export default ApiTest; 
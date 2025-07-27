import React from 'react';
import { useAuth } from '../contexts/AuthContext';

const AuthDebug: React.FC = () => {
  const { state, logout, clearError, refreshTokenManually } = useAuth();

  const handleRefreshToken = async () => {
    try {
      await refreshTokenManually();
      console.log('✅ Token refreshed successfully!');
    } catch (error) {
      console.error('❌ Token refresh failed:', error);
    }
  };

  return (
    <div className="bg-gray-100 p-4 rounded-lg border-2 border-dashed border-gray-300 mb-6">
      <h3 className="text-lg font-bold text-gray-800 mb-3">🔧 Auth State Debug</h3>
      <div className="text-sm space-y-2">
        <div>
          <strong>Is Authenticated:</strong> 
          <span className={state.isAuthenticated ? 'text-green-600' : 'text-red-600'}>
            {' '}{state.isAuthenticated ? 'Yes' : 'No'}
          </span>
        </div>
        <div>
          <strong>Is Loading:</strong> 
          <span className={state.isLoading ? 'text-yellow-600' : 'text-gray-600'}>
            {' '}{state.isLoading ? 'Yes' : 'No'}
          </span>
        </div>
        <div>
          <strong>User:</strong> 
          <span className="text-gray-700">
            {' '}{state.user ? `${state.user.fullName} (${state.user.email})` : 'None'}
          </span>
        </div>
        <div>
          <strong>Token:</strong> 
          <span className="text-gray-700 font-mono">
            {' '}{state.token ? `${state.token.substring(0, 20)}...` : 'None'}
          </span>
        </div>
        {state.error && (
          <div>
            <strong>Error:</strong> 
            <span className="text-red-600">{' '}{state.error}</span>
          </div>
        )}
      </div>
      <div className="mt-4 space-x-2">
        {state.isAuthenticated && (
          <button
            onClick={logout}
            className="px-3 py-1 bg-red-500 text-white rounded text-sm hover:bg-red-600"
          >
            Logout
          </button>
        )}
        {state.error && (
          <button
            onClick={clearError}
            className="px-3 py-1 bg-blue-500 text-white rounded text-sm hover:bg-blue-600"
          >
            Clear Error
          </button>
        )}
        <button
          onClick={handleRefreshToken}
          className="px-3 py-1 bg-purple-500 text-white rounded text-sm hover:bg-purple-600"
        >
          Refresh Token
        </button>
      </div>
    </div>
  );
};

export default AuthDebug; 
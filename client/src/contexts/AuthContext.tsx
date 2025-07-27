import React, { createContext, useContext, useReducer, useEffect } from 'react';
import type { ReactNode } from 'react';

// Types for our authentication state
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phoneNumber?: string;
  companyName?: string;
  jobTitle?: string;
  isActive: boolean;
  createdAt: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

// Action types for our reducer
type AuthAction =
  | { type: 'LOGIN_START' }
  | { type: 'LOGIN_SUCCESS'; payload: { user: User; token: string } }
  | { type: 'LOGIN_FAILURE'; payload: string }
  | { type: 'LOGOUT' }
  | { type: 'CLEAR_ERROR' }
  | { type: 'SET_LOADING'; payload: boolean }
  | { type: 'UPDATE_USER'; payload: User };

// Initial state
const initialState: AuthState = {
  user: null,
  token: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
};

// Auth reducer to manage state transitions
const authReducer = (state: AuthState, action: AuthAction): AuthState => {
  console.log('🎯 Auth action dispatched:', action.type, action);
  
  switch (action.type) {
    case 'LOGIN_START':
      return {
        ...state,
        isLoading: true,
        error: null,
      };
    case 'LOGIN_SUCCESS':
      return {
        ...state,
        user: action.payload.user,
        token: action.payload.token,
        isAuthenticated: true,
        isLoading: false,
        error: null,
      };
    case 'LOGIN_FAILURE':
      return {
        ...state,
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
        error: action.payload,
      };
    case 'LOGOUT':
      return {
        ...state,
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
        error: null,
      };
    case 'CLEAR_ERROR':
      return {
        ...state,
        error: null,
      };
    case 'SET_LOADING':
      return {
        ...state,
        isLoading: action.payload,
      };
    case 'UPDATE_USER':
      return {
        ...state,
        user: action.payload,
      };
    default:
      return state;
  }
};

// Context interface
interface AuthContextType {
  state: AuthState;
  login: (email: string, password: string) => Promise<void>;
  register: (userData: RegisterData) => Promise<void>;
  logout: () => void;
  clearError: () => void;
  updateUser: (userData: Partial<User>) => void;
  refreshTokenManually: () => Promise<void>;
}

export interface RegisterData {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  companyName?: string;
  jobTitle?: string;
}

// Create the context
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Auth provider component
export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [state, dispatch] = useReducer(authReducer, initialState);

  // Load token from localStorage on app start
  useEffect(() => {
    const loadStoredAuth = () => {
      try {
        const storedToken = localStorage.getItem('legalvibes_token');
        const storedUser = localStorage.getItem('legalvibes_user');
        
        if (storedToken && storedUser) {
          const user = JSON.parse(storedUser);
          dispatch({
            type: 'LOGIN_SUCCESS',
            payload: { user, token: storedToken }
          });
        }
      } catch (error) {
        console.error('Error loading stored auth:', error);
        // Clear invalid stored data
        localStorage.removeItem('legalvibes_token');
        localStorage.removeItem('legalvibes_user');
      }
    };

    loadStoredAuth();
  }, []);

  // Save to localStorage whenever auth state changes
  useEffect(() => {
    console.log('🔄 Auth state changed:', {
      isAuthenticated: state.isAuthenticated,
      hasToken: !!state.token,
      hasUser: !!state.user,
      userEmail: state.user?.email
    });
    
    if (state.isAuthenticated && state.token && state.user) {
      console.log('💾 Saving auth to localStorage');
      localStorage.setItem('legalvibes_token', state.token);
      localStorage.setItem('legalvibes_user', JSON.stringify(state.user));
    } else {
      console.log('🗑️ Clearing auth from localStorage');
      localStorage.removeItem('legalvibes_token');
      localStorage.removeItem('legalvibes_user');
    }
  }, [state.isAuthenticated, state.token, state.user]);

  // Login function
  const login = async (email: string, password: string): Promise<void> => {
    dispatch({ type: 'LOGIN_START' });
    
    try {
      const { login: apiLogin } = await import('../services/api');
      const result = await apiLogin({ email, password });
      
      dispatch({
        type: 'LOGIN_SUCCESS',
        payload: {
          user: result.user,
          token: result.token
        }
      });
    } catch (error) {
      dispatch({
        type: 'LOGIN_FAILURE',
        payload: error instanceof Error ? error.message : 'Login failed'
      });
      throw error;
    }
  };

  // Register function
  const register = async (userData: RegisterData): Promise<void> => {
    dispatch({ type: 'LOGIN_START' });
    
    try {
      const { register: apiRegister } = await import('../services/api');
      const result = await apiRegister(userData);
      
      dispatch({
        type: 'LOGIN_SUCCESS',
        payload: {
          user: result.user,
          token: result.token
        }
      });
    } catch (error) {
      dispatch({
        type: 'LOGIN_FAILURE',
        payload: error instanceof Error ? error.message : 'Registration failed'
      });
      throw error;
    }
  };

  // Logout function
  const logout = (): void => {
    dispatch({ type: 'LOGOUT' });
  };

  // Clear error function
  const clearError = (): void => {
    dispatch({ type: 'CLEAR_ERROR' });
  };

  // Update user function
  const updateUser = (userData: Partial<User>): void => {
    if (state.user) {
      const updatedUser = { ...state.user, ...userData };
      dispatch({ type: 'UPDATE_USER', payload: updatedUser });
    }
  };

  // Manual token refresh function
  const refreshTokenManually = async (): Promise<void> => {
    if (!state.token) {
      throw new Error('No token available for refresh');
    }

    dispatch({ type: 'SET_LOADING', payload: true });
    
    try {
      const { refreshToken } = await import('../services/api');
      const result = await refreshToken();
      
      dispatch({
        type: 'LOGIN_SUCCESS',
        payload: {
          user: result.user,
          token: result.token
        }
      });
    } catch (error) {
      dispatch({
        type: 'LOGIN_FAILURE',
        payload: error instanceof Error ? error.message : 'Token refresh failed'
      });
      throw error;
    } finally {
      dispatch({ type: 'SET_LOADING', payload: false });
    }
  };

  const contextValue: AuthContextType = {
    state,
    login,
    register,
    logout,
    clearError,
    updateUser,
    refreshTokenManually,
  };

  return <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>;
};

// Custom hook to use the auth context
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}; 
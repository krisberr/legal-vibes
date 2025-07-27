import axios, { AxiosError } from 'axios';
import type { AxiosInstance, AxiosResponse } from 'axios';
import type { User, RegisterData } from '../contexts/AuthContext';

// API Response types matching our .NET backend
export interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  error?: string;
  data?: T;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: {
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
  };
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  companyName?: string;
  jobTitle?: string;
}

// Project Management Types
export interface Client {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  companyName?: string;
  address?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateClientRequest {
  name: string;
  email: string;
  phoneNumber: string;
  companyName?: string;
  address?: string;
}

export interface UpdateClientRequest {
  name?: string;
  email?: string;
  phoneNumber?: string;
  companyName?: string;
  address?: string;
  isActive?: boolean;
}

export enum ProjectStatus {
  Draft = 0,
  InProgress = 1,
  UnderReview = 2,
  Submitted = 3,
  Approved = 4,
  Rejected = 5,
  Completed = 6,
  Archived = 7
}

export enum ProjectType {
  TrademarkApplication = 0,
  PatentApplication = 1,
  CopyrightRegistration = 2,
  IPConsultation = 3,
  Other = 4
}

export interface Project {
  id: string;
  name: string;
  description: string;
  status: ProjectStatus;
  type: ProjectType;
  dueDate?: string;
  referenceNumber?: string;
  createdAt: string;
  createdBy?: string;
  clientId: string;
  client: Client;
  trademarkName?: string;
  trademarkDescription?: string;
  goodsAndServices?: string;
  specialConsiderations?: string;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  type: ProjectType;
  clientId: string;
  dueDate?: string;
  referenceNumber?: string;
  trademarkName?: string;
  trademarkDescription?: string;
  goodsAndServices?: string;
  specialConsiderations?: string;
}

export interface UpdateProjectRequest {
  name?: string;
  description?: string;
  status?: ProjectStatus;
  clientId?: string;
  dueDate?: string;
  referenceNumber?: string;
  trademarkName?: string;
  trademarkDescription?: string;
  goodsAndServices?: string;
  specialConsiderations?: string;
}

export interface UpdateProjectStatusRequest {
  status: ProjectStatus;
}

// API Configuration
const API_BASE_URL = 'https://localhost:7032/api'; // Our .NET API URL (HTTPS)
const API_TIMEOUT = 10000; // 10 seconds

class ApiService {
  private axiosInstance: AxiosInstance;
  private isRefreshing = false;
  private failedQueue: Array<{
    resolve: (value?: any) => void;
    reject: (error?: any) => void;
  }> = [];

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: API_BASE_URL,
      timeout: API_TIMEOUT,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor to add JWT token
    this.axiosInstance.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('legalvibes_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor with automatic token refresh
    this.axiosInstance.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        const originalRequest = error.config as any;

        if (error.response?.status === 401 && !originalRequest._retry) {
          if (this.isRefreshing) {
            // If already refreshing, queue this request
            return new Promise((resolve, reject) => {
              this.failedQueue.push({ resolve, reject });
            }).then(token => {
              if (originalRequest.headers) {
                originalRequest.headers.Authorization = `Bearer ${token}`;
              }
              return this.axiosInstance(originalRequest);
            }).catch(err => {
              return Promise.reject(err);
            });
          }

          originalRequest._retry = true;
          this.isRefreshing = true;

          try {
            const newToken = await this.refreshToken();
            
            // Update stored token
            localStorage.setItem('legalvibes_token', newToken.token);
            
            // Update user data if provided
            if (newToken.user) {
              localStorage.setItem('legalvibes_user', JSON.stringify(newToken.user));
            }

            // Process queued requests
            this.processQueue(newToken.token, null);
            
            // Retry original request with new token
            if (originalRequest.headers) {
              originalRequest.headers.Authorization = `Bearer ${newToken.token}`;
            }
            return this.axiosInstance(originalRequest);

          } catch (refreshError) {
            // Refresh failed - logout user
            this.processQueue(null, refreshError);
            this.clearAuthData();
            
            // Redirect to login (you might want to dispatch an event instead)
            window.location.href = '/login';
            
            return Promise.reject(refreshError);
          } finally {
            this.isRefreshing = false;
          }
        }

        return Promise.reject(this.handleApiError(error));
      }
    );
  }

  private processQueue(token: string | null, error: any) {
    this.failedQueue.forEach(({ resolve, reject }) => {
      if (error) {
        reject(error);
      } else {
        resolve(token);
      }
    });
    
    this.failedQueue = [];
  }

  private clearAuthData() {
    localStorage.removeItem('legalvibes_token');
    localStorage.removeItem('legalvibes_user');
  }

  // Error handling helper
  private handleApiError(error: AxiosError): Error {
    if (error.response) {
      // Server responded with error status
      const responseData = error.response.data as ApiResponse;
      const message = responseData.error || responseData.message || `HTTP ${error.response.status}: ${error.response.statusText}`;
      return new Error(message);
    } else if (error.request) {
      // Network error
      return new Error('Network error: Unable to connect to server');
    } else {
      // Other error
      return new Error(error.message || 'An unexpected error occurred');
    }
  }

  // Helper method to extract data from API response
  private extractData<T>(response: AxiosResponse<ApiResponse<T>>): T {
    if (response.data.success && response.data.data) {
      return response.data.data;
    }
    throw new Error(response.data.error || response.data.message || 'API request failed');
  }

  // Authentication endpoints
  async login(credentials: LoginRequest): Promise<{ user: User; token: string }> {
    try {
      const response = await this.axiosInstance.post<ApiResponse<AuthResponse>>('/auth/login', credentials);
      const authData = this.extractData(response);
      
      // Transform the response to match our User interface
      const user: User = {
        id: authData.user.id,
        email: authData.user.email,
        firstName: authData.user.firstName,
        lastName: authData.user.lastName,
        fullName: authData.user.fullName,
        phoneNumber: authData.user.phoneNumber,
        companyName: authData.user.companyName,
        jobTitle: authData.user.jobTitle,
        isActive: authData.user.isActive,
        createdAt: authData.user.createdAt,
      };

      return {
        user,
        token: authData.token,
      };
    } catch (error) {
      console.error('Login API error:', error);
      throw error;
    }
  }

  async register(userData: RegisterRequest): Promise<{ user: User; token: string }> {
    try {
      const response = await this.axiosInstance.post<ApiResponse<AuthResponse>>('/auth/register', userData);
      const authData = this.extractData(response);
      
      // Transform the response to match our User interface
      const user: User = {
        id: authData.user.id,
        email: authData.user.email,
        firstName: authData.user.firstName,
        lastName: authData.user.lastName,
        fullName: authData.user.fullName,
        phoneNumber: authData.user.phoneNumber,
        companyName: authData.user.companyName,
        jobTitle: authData.user.jobTitle,
        isActive: authData.user.isActive,
        createdAt: authData.user.createdAt,
      };

      return {
        user,
        token: authData.token,
      };
    } catch (error) {
      console.error('Register API error:', error);
      throw error;
    }
  }

  async getProfile(): Promise<User> {
    try {
      const response = await this.axiosInstance.get<ApiResponse<User>>('/auth/profile');
      return this.extractData(response);
    } catch (error) {
      console.error('Get profile API error:', error);
      throw error;
    }
  }

  async updateProfile(userData: Partial<User>): Promise<User> {
    try {
      const response = await this.axiosInstance.put<ApiResponse<User>>('/auth/profile', userData);
      return this.extractData(response);
    } catch (error) {
      console.error('Update profile API error:', error);
      throw error;
    }
  }

  async validateToken(): Promise<boolean> {
    try {
      await this.axiosInstance.get('/auth/validate-token');
      return true;
    } catch (error) {
      return false;
    }
  }

  // Client Management endpoints
  async getClients(): Promise<Client[]> {
    try {
      const response = await this.axiosInstance.get<Client[]>('/client');
      return response.data;
    } catch (error) {
      console.error('Get clients API error:', error);
      throw error;
    }
  }

  async getClient(id: string): Promise<Client> {
    try {
      const response = await this.axiosInstance.get<Client>(`/client/${id}`);
      return response.data;
    } catch (error) {
      console.error('Get client API error:', error);
      throw error;
    }
  }

  async createClient(clientData: CreateClientRequest): Promise<Client> {
    try {
      const response = await this.axiosInstance.post<Client>('/client', clientData);
      return response.data;
    } catch (error) {
      console.error('Create client API error:', error);
      throw error;
    }
  }

  async updateClient(id: string, clientData: UpdateClientRequest): Promise<Client> {
    try {
      const response = await this.axiosInstance.put<Client>(`/client/${id}`, clientData);
      return response.data;
    } catch (error) {
      console.error('Update client API error:', error);
      throw error;
    }
  }

  async deleteClient(id: string): Promise<void> {
    try {
      await this.axiosInstance.delete(`/client/${id}`);
    } catch (error) {
      console.error('Delete client API error:', error);
      throw error;
    }
  }

  // Project Management endpoints
  async getProjects(search?: string): Promise<Project[]> {
    try {
      const url = search ? `/project?search=${encodeURIComponent(search)}` : '/project';
      const response = await this.axiosInstance.get<Project[]>(url);
      return response.data;
    } catch (error) {
      console.error('Get projects API error:', error);
      throw error;
    }
  }

  async getProject(id: string): Promise<Project> {
    try {
      const response = await this.axiosInstance.get<Project>(`/project/${id}`);
      return response.data;
    } catch (error) {
      console.error('Get project API error:', error);
      throw error;
    }
  }

  async createProject(projectData: CreateProjectRequest): Promise<Project> {
    try {
      const response = await this.axiosInstance.post<Project>('/project', projectData);
      return response.data;
    } catch (error) {
      console.error('Create project API error:', error);
      throw error;
    }
  }

  async updateProject(id: string, projectData: UpdateProjectRequest): Promise<Project> {
    try {
      const response = await this.axiosInstance.put<Project>(`/project/${id}`, projectData);
      return response.data;
    } catch (error) {
      console.error('Update project API error:', error);
      throw error;
    }
  }

  async updateProjectStatus(id: string, status: ProjectStatus): Promise<Project> {
    try {
      const response = await this.axiosInstance.put<Project>(`/project/${id}/status`, { status });
      return response.data;
    } catch (error) {
      console.error('Update project status API error:', error);
      throw error;
    }
  }

  async deleteProject(id: string): Promise<void> {
    try {
      await this.axiosInstance.delete(`/project/${id}`);
    } catch (error) {
      console.error('Delete project API error:', error);
      throw error;
    }
  }

  // Health check endpoint (not under /api prefix)
  async healthCheck(): Promise<string> {
    try {
      const response = await axios.get<string>('https://localhost:7032/health');
      return response.data;
    } catch (error) {
      console.error('Health check API error:', error);
      throw error;
    }
  }

  // Refresh token endpoint
  async refreshToken(): Promise<{ user: User; token: string }> {
    try {
      const token = localStorage.getItem('legalvibes_token');
      if (!token) {
        throw new Error('No token available for refresh');
      }

      const response = await axios.post<ApiResponse<AuthResponse>>(
        `${API_BASE_URL}/auth/refresh-token`,
        {},
        {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          },
          timeout: 10000
        }
      );

      const authData = this.extractData(response);
      
      return {
        user: {
          id: authData.user.id,
          email: authData.user.email,
          firstName: authData.user.firstName,
          lastName: authData.user.lastName,
          phoneNumber: authData.user.phoneNumber,
          isActive: authData.user.isActive,
          companyName: authData.user.companyName,
          jobTitle: authData.user.jobTitle,
          createdAt: authData.user.createdAt,
          fullName: authData.user.fullName
        },
        token: authData.token
      };
    } catch (error) {
      console.error('Token refresh API error:', error);
      throw this.handleApiError(error as AxiosError);
    }
  }
}

// Create and export a singleton instance
export const apiService = new ApiService();

// Export individual methods for easier importing
export const login = apiService.login.bind(apiService);
export const register = apiService.register.bind(apiService);
export const getProfile = apiService.getProfile.bind(apiService);
export const updateProfile = apiService.updateProfile.bind(apiService);
export const validateToken = apiService.validateToken.bind(apiService);
export const refreshToken = apiService.refreshToken.bind(apiService);
export const healthCheck = apiService.healthCheck.bind(apiService);

// Client Management exports
export const getClients = apiService.getClients.bind(apiService);
export const getClient = apiService.getClient.bind(apiService);
export const createClient = apiService.createClient.bind(apiService);
export const updateClient = apiService.updateClient.bind(apiService);
export const deleteClient = apiService.deleteClient.bind(apiService);

// Project Management exports
export const getProjects = apiService.getProjects.bind(apiService);
export const getProject = apiService.getProject.bind(apiService);
export const createProject = apiService.createProject.bind(apiService);
export const updateProject = apiService.updateProject.bind(apiService);
export const updateProjectStatus = apiService.updateProjectStatus.bind(apiService);
export const deleteProject = apiService.deleteProject.bind(apiService); 
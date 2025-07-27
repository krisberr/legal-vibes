import React, { useState, useEffect } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { 
  getProjects, 
  getClients,
  createProject,
  updateProjectStatus,
  deleteProject,
  type Project, 
  type Client,
  type CreateProjectRequest,
  ProjectStatus,
  ProjectType 
} from '../../services/api';

// Status badge component
const StatusBadge: React.FC<{ status: ProjectStatus }> = ({ status }) => {
  const getStatusConfig = (status: ProjectStatus) => {
    switch (status) {
      case ProjectStatus.Draft:
        return { text: 'Draft', bg: 'bg-gray-100', color: 'text-gray-800' };
      case ProjectStatus.InProgress:
        return { text: 'In Progress', bg: 'bg-blue-100', color: 'text-blue-800' };
      case ProjectStatus.UnderReview:
        return { text: 'Under Review', bg: 'bg-yellow-100', color: 'text-yellow-800' };
      case ProjectStatus.Submitted:
        return { text: 'Submitted', bg: 'bg-purple-100', color: 'text-purple-800' };
      case ProjectStatus.Approved:
        return { text: 'Approved', bg: 'bg-green-100', color: 'text-green-800' };
      case ProjectStatus.Rejected:
        return { text: 'Rejected', bg: 'bg-red-100', color: 'text-red-800' };
      case ProjectStatus.Completed:
        return { text: 'Completed', bg: 'bg-emerald-100', color: 'text-emerald-800' };
      case ProjectStatus.Archived:
        return { text: 'Archived', bg: 'bg-slate-100', color: 'text-slate-800' };
      default:
        return { text: 'Unknown', bg: 'bg-gray-100', color: 'text-gray-800' };
    }
  };

  const config = getStatusConfig(status);
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${config.bg} ${config.color}`}>
      {config.text}
    </span>
  );
};

// Project type helper
const getProjectTypeText = (type: ProjectType): string => {
  switch (type) {
    case ProjectType.TrademarkApplication:
      return 'Trademark Application';
    case ProjectType.PatentApplication:
      return 'Patent Application';
    case ProjectType.CopyrightRegistration:
      return 'Copyright Registration';
    case ProjectType.IPConsultation:
      return 'IP Consultation';
    case ProjectType.Other:
      return 'Other';
    default:
      return 'Unknown';
  }
};

const ProjectsPage: React.FC = () => {
  const { state } = useAuth();
  const [projects, setProjects] = useState<Project[]>([]);
  const [clients, setClients] = useState<Client[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  // Create project form state
  const [createForm, setCreateForm] = useState<CreateProjectRequest>({
    name: '',
    description: '',
    type: ProjectType.TrademarkApplication,
    clientId: '',
    referenceNumber: '',
    trademarkName: '',
    trademarkDescription: '',
    goodsAndServices: '',
    specialConsiderations: ''
  });

  // Load projects and clients
  const loadData = async () => {
    try {
      setIsLoading(true);
      setError(null);
      
      const [projectsData, clientsData] = await Promise.all([
        getProjects(searchTerm || undefined),
        getClients()
      ]);
      
      setProjects(projectsData);
      setClients(clientsData);
    } catch (err: any) {
      setError(err.message || 'Failed to load data');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [searchTerm]);

  // Handle search with debounce
  const handleSearch = (term: string) => {
    setSearchTerm(term);
  };

  // Handle create project
  const handleCreateProject = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!createForm.name || !createForm.description || !createForm.clientId) {
      setError('Please fill in all required fields');
      return;
    }

    try {
      setError(null);
      const newProject = await createProject(createForm);
      setProjects([newProject, ...projects]);
      setShowCreateForm(false);
      setCreateForm({
        name: '',
        description: '',
        type: ProjectType.TrademarkApplication,
        clientId: '',
        referenceNumber: '',
        trademarkName: '',
        trademarkDescription: '',
        goodsAndServices: '',
        specialConsiderations: ''
      });
      setSuccessMessage('Project created successfully!');
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err: any) {
      setError(err.message || 'Failed to create project');
    }
  };

  // Handle status update
  const handleStatusUpdate = async (projectId: string, newStatus: ProjectStatus) => {
    try {
      setError(null);
      const updatedProject = await updateProjectStatus(projectId, newStatus);
      setProjects(projects.map(p => p.id === projectId ? updatedProject : p));
      setSuccessMessage('Project status updated successfully!');
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err: any) {
      setError(err.message || 'Failed to update project status');
    }
  };

  // Handle delete project
  const handleDeleteProject = async (projectId: string) => {
    if (!window.confirm('Are you sure you want to delete this project?')) {
      return;
    }

    try {
      setError(null);
      await deleteProject(projectId);
      setProjects(projects.filter(p => p.id !== projectId));
      setSuccessMessage('Project deleted successfully!');
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err: any) {
      setError(err.message || 'Failed to delete project');
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Page Header */}
        <div className="mb-8">
          <div className="md:flex md:items-center md:justify-between">
            <div className="flex-1 min-w-0">
              <h2 className="text-2xl font-bold leading-7 text-gray-900 sm:text-3xl sm:truncate">
                Projects
              </h2>
              <p className="mt-1 text-sm text-gray-500">
                Manage your intellectual property projects
              </p>
            </div>
            <div className="mt-4 flex md:mt-0 md:ml-4">
              <button
                onClick={() => setShowCreateForm(true)}
                className="ml-3 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                <svg className="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
                New Project
              </button>
            </div>
          </div>
        </div>

        {/* Messages */}
        {error && (
          <div className="mb-4 bg-red-50 border border-red-200 rounded-md p-4">
            <div className="flex">
              <svg className="h-5 w-5 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <div className="ml-3">
                <p className="text-sm text-red-800">{error}</p>
              </div>
            </div>
          </div>
        )}

        {successMessage && (
          <div className="mb-4 bg-green-50 border border-green-200 rounded-md p-4">
            <div className="flex">
              <svg className="h-5 w-5 text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <div className="ml-3">
                <p className="text-sm text-green-800">{successMessage}</p>
              </div>
            </div>
          </div>
        )}

        {/* Search */}
        <div className="mb-6">
          <div className="max-w-lg">
            <label htmlFor="search" className="sr-only">Search projects</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <svg className="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>
              <input
                type="text"
                id="search"
                className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
                placeholder="Search projects..."
                value={searchTerm}
                onChange={(e) => handleSearch(e.target.value)}
              />
            </div>
          </div>
        </div>

        {/* Projects Grid */}
        {projects.length === 0 ? (
          <div className="text-center py-12">
            <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            <h3 className="mt-2 text-sm font-medium text-gray-900">No projects</h3>
            <p className="mt-1 text-sm text-gray-500">Get started by creating your first project.</p>
            <div className="mt-6">
              <button
                onClick={() => setShowCreateForm(true)}
                className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                <svg className="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
                New Project
              </button>
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {projects.map((project) => (
              <div key={project.id} className="bg-white overflow-hidden shadow rounded-lg">
                <div className="p-6">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="h-8 w-8 bg-blue-500 rounded-lg flex items-center justify-center">
                          <svg className="h-4 w-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                          </svg>
                        </div>
                      </div>
                      <div className="ml-4">
                        <h3 className="text-lg font-medium text-gray-900 truncate">{project.name}</h3>
                        <p className="text-sm text-gray-500">{getProjectTypeText(project.type)}</p>
                      </div>
                    </div>
                    <StatusBadge status={project.status} />
                  </div>
                  
                  <div className="mt-4">
                    <p className="text-sm text-gray-600 line-clamp-2">{project.description}</p>
                  </div>

                  <div className="mt-4">
                    <div className="flex items-center text-sm text-gray-500">
                      <svg className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                      </svg>
                      <span className="truncate">{project.client.name}</span>
                    </div>
                    {project.referenceNumber && (
                      <div className="flex items-center text-sm text-gray-500 mt-1">
                        <svg className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 20l4-16m2 16l4-16M6 9h14M4 15h14" />
                        </svg>
                        <span>{project.referenceNumber}</span>
                      </div>
                    )}
                    <div className="flex items-center text-sm text-gray-500 mt-1">
                      <svg className="flex-shrink-0 mr-1.5 h-4 w-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3a1 1 0 011-1h6a1 1 0 011 1v4h3a1 1 0 011 1v9a2 2 0 01-2 2H5a2 2 0 01-2-2V8a1 1 0 011-1h3z" />
                      </svg>
                      <span>{new Date(project.createdAt).toLocaleDateString()}</span>
                    </div>
                  </div>

                  <div className="mt-6 flex items-center justify-between">
                    <div className="flex space-x-2">
                      <select
                        value={project.status}
                        onChange={(e) => handleStatusUpdate(project.id, parseInt(e.target.value) as ProjectStatus)}
                        className="text-xs border border-gray-300 rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
                      >
                        <option value={ProjectStatus.Draft}>Draft</option>
                        <option value={ProjectStatus.InProgress}>In Progress</option>
                        <option value={ProjectStatus.UnderReview}>Under Review</option>
                        <option value={ProjectStatus.Submitted}>Submitted</option>
                        <option value={ProjectStatus.Approved}>Approved</option>
                        <option value={ProjectStatus.Rejected}>Rejected</option>
                        <option value={ProjectStatus.Completed}>Completed</option>
                        <option value={ProjectStatus.Archived}>Archived</option>
                      </select>
                    </div>
                    <button
                      onClick={() => handleDeleteProject(project.id)}
                      className="text-red-600 hover:text-red-900 text-sm"
                    >
                      Delete
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Create Project Modal */}
      {showCreateForm && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white">
            <div className="mt-3">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-lg font-medium text-gray-900">Create New Project</h3>
                <button
                  onClick={() => setShowCreateForm(false)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <svg className="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>

              <form onSubmit={handleCreateProject} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Project Name *</label>
                  <input
                    type="text"
                    required
                    className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                    value={createForm.name}
                    onChange={(e) => setCreateForm({...createForm, name: e.target.value})}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Description *</label>
                  <textarea
                    required
                    rows={3}
                    className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                    value={createForm.description}
                    onChange={(e) => setCreateForm({...createForm, description: e.target.value})}
                  />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Client *</label>
                    {clients.length === 0 ? (
                      <div className="mt-1 p-3 bg-yellow-50 border border-yellow-200 rounded-md">
                        <p className="text-sm text-yellow-800">No clients found. Please create a client first in Swagger or contact your administrator.</p>
                        <p className="text-xs text-yellow-600 mt-1">
                          Tip: Go to <code>https://localhost:7032/swagger</code> → <code>/api/Client POST</code> → Create a client with your current user login.
                        </p>
                      </div>
                    ) : (
                      <select
                        required
                        className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={createForm.clientId}
                        onChange={(e) => setCreateForm({...createForm, clientId: e.target.value})}
                      >
                        <option value="">Select a client</option>
                        {clients.map((client) => (
                          <option key={client.id} value={client.id}>{client.name}</option>
                        ))}
                      </select>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">Project Type</label>
                    <select
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                      value={createForm.type}
                      onChange={(e) => setCreateForm({...createForm, type: parseInt(e.target.value) as ProjectType})}
                    >
                      <option value={ProjectType.TrademarkApplication}>Trademark Application</option>
                      <option value={ProjectType.PatentApplication}>Patent Application</option>
                      <option value={ProjectType.CopyrightRegistration}>Copyright Registration</option>
                      <option value={ProjectType.IPConsultation}>IP Consultation</option>
                      <option value={ProjectType.Other}>Other</option>
                    </select>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Reference Number</label>
                  <input
                    type="text"
                    className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                    value={createForm.referenceNumber}
                    onChange={(e) => setCreateForm({...createForm, referenceNumber: e.target.value})}
                  />
                </div>

                {createForm.type === ProjectType.TrademarkApplication && (
                  <>
                    <div>
                      <label className="block text-sm font-medium text-gray-700">Trademark Name</label>
                      <input
                        type="text"
                        className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={createForm.trademarkName}
                        onChange={(e) => setCreateForm({...createForm, trademarkName: e.target.value})}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700">Trademark Description</label>
                      <textarea
                        rows={2}
                        className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={createForm.trademarkDescription}
                        onChange={(e) => setCreateForm({...createForm, trademarkDescription: e.target.value})}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700">Goods and Services</label>
                      <textarea
                        rows={2}
                        className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={createForm.goodsAndServices}
                        onChange={(e) => setCreateForm({...createForm, goodsAndServices: e.target.value})}
                      />
                    </div>
                  </>
                )}

                <div className="flex justify-end space-x-3 pt-4">
                  <button
                    type="button"
                    onClick={() => setShowCreateForm(false)}
                    className="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                  >
                    Create Project
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ProjectsPage; 
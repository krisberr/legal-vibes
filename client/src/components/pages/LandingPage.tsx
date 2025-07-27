import React from 'react';
import { Link } from 'react-router-dom';

const LandingPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-50">
      {/* Navigation */}
      <nav className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16 items-center">
            <div className="flex-shrink-0">
              <span className="text-2xl font-bold text-blue-600">Legal Vibes</span>
            </div>
            <div className="hidden sm:flex sm:space-x-8">
              <a href="#features" className="text-slate-600 hover:text-slate-900 px-3 py-2 rounded-md text-sm font-medium">Features</a>
              <a href="#about" className="text-slate-600 hover:text-slate-900 px-3 py-2 rounded-md text-sm font-medium">About</a>
              <a href="#contact" className="text-slate-600 hover:text-slate-900 px-3 py-2 rounded-md text-sm font-medium">Contact</a>
            </div>
            <div className="flex space-x-4">
              <Link to="/login" className="bg-white text-blue-600 px-4 py-2 rounded-md text-sm font-medium border border-blue-600 hover:bg-blue-50">
                Sign In
              </Link>
              <Link to="/register" className="bg-blue-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-blue-700">
                Get Started
              </Link>
            </div>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <div className="relative overflow-hidden">
        <div className="max-w-7xl mx-auto pt-16 pb-24 px-4 sm:pt-24 sm:px-6 lg:px-8">
          <div className="text-center">
            <h1 className="text-4xl tracking-tight font-extrabold text-slate-900 sm:text-5xl md:text-6xl">
              <span className="block">Revolutionize Your</span>
              <span className="block text-blue-600">IP Law Practice</span>
            </h1>
            <p className="mt-3 max-w-md mx-auto text-base text-slate-500 sm:text-lg md:mt-5 md:text-xl md:max-w-3xl">
              Harness the power of AI to streamline your trademark applications, automate document generation, and enhance your legal practice.
            </p>
            <div className="mt-10 flex justify-center space-x-4">
              <Link to="/register" className="bg-blue-600 text-white px-8 py-3 rounded-md text-lg font-medium hover:bg-blue-700 transform transition hover:scale-105">
                Start Free Trial
              </Link>
              <Link to="/login" className="bg-white text-blue-600 px-8 py-3 rounded-md text-lg font-medium border-2 border-blue-600 hover:bg-blue-50 transform transition hover:scale-105">
                Sign In
              </Link>
            </div>
          </div>
        </div>
      </div>

      {/* Features Section */}
      <div className="bg-white py-24" id="features">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h2 className="text-3xl font-extrabold text-slate-900 sm:text-4xl">
              Powerful Features for Modern IP Lawyers
            </h2>
            <p className="mt-4 text-lg text-slate-500">
              Everything you need to manage your IP practice efficiently
            </p>
          </div>

          <div className="mt-20 grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-3">
            {/* AI Document Generation */}
            <div className="flex flex-col items-center p-6 bg-slate-50 rounded-lg shadow hover:shadow-md transition">
              <div className="h-12 w-12 text-blue-600 mb-4">
                <svg className="h-full w-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
              <h3 className="text-xl font-medium text-slate-900">AI Document Generation</h3>
              <p className="mt-2 text-slate-500 text-center">
                Generate trademark applications and legal documents with AI assistance
              </p>
            </div>

            {/* Smart Analytics */}
            <div className="flex flex-col items-center p-6 bg-slate-50 rounded-lg shadow hover:shadow-md transition">
              <div className="h-12 w-12 text-blue-600 mb-4">
                <svg className="h-full w-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <h3 className="text-xl font-medium text-slate-900">Smart Analytics</h3>
              <p className="mt-2 text-slate-500 text-center">
                Track your success rates and analyze patterns in trademark applications
              </p>
            </div>

            {/* Automated Workflows */}
            <div className="flex flex-col items-center p-6 bg-slate-50 rounded-lg shadow hover:shadow-md transition">
              <div className="h-12 w-12 text-blue-600 mb-4">
                <svg className="h-full w-full" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                </svg>
              </div>
              <h3 className="text-xl font-medium text-slate-900">Automated Workflows</h3>
              <p className="mt-2 text-slate-500 text-center">
                Streamline your practice with automated document workflows
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* CTA Section */}
      <div className="bg-blue-700">
        <div className="max-w-7xl mx-auto py-12 px-4 sm:px-6 lg:py-16 lg:px-8 lg:flex lg:items-center lg:justify-between">
          <h2 className="text-3xl font-extrabold tracking-tight text-white sm:text-4xl">
            <span className="block">Ready to get started?</span>
            <span className="block text-blue-200">Start your free trial today.</span>
          </h2>
          <div className="mt-8 flex lg:mt-0 lg:flex-shrink-0">
            <div className="inline-flex rounded-md shadow">
              <button className="bg-white text-blue-600 px-8 py-3 rounded-md text-lg font-medium hover:bg-blue-50">
                Get Started
              </button>
            </div>
            <div className="ml-3 inline-flex rounded-md shadow">
              <button className="bg-blue-800 text-white px-8 py-3 rounded-md text-lg font-medium hover:bg-blue-900">
                Learn More
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Footer */}
      <footer className="bg-slate-800">
        <div className="max-w-7xl mx-auto py-12 px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <p className="text-base text-slate-400">
              © 2024 Legal Vibes. All rights reserved.
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default LandingPage; 
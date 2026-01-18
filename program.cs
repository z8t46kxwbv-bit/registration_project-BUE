import React, { useState, useEffect } from 'react';
import { AlertCircle, CheckCircle, Edit2, Save, X, Plus, Search, ChevronLeft, ChevronRight, Users, UserPlus, Sparkles } from 'lucide-react';

// Simple toggle - flip this to connect to your real backend!
const USE_MOCK_DATA = true;
const API_URL = 'https://localhost:5001/api';

// Our friendly data manager - handles both practice mode and real server
const dataManager = {
  async fetchUsers(page = 1, limit = 10, searchText = '') {
    if (USE_MOCK_DATA) {
      // Practice mode - using browser memory
      const allUsers = JSON.parse(localStorage.getItem('users') || '[]');
      let results = allUsers;
      
      // Filter if someone's searching
      if (searchText.trim()) {
        const search = searchText.toLowerCase();
        results = allUsers.filter(person => 
          person.name.toLowerCase().includes(search) ||
          person.email.toLowerCase().includes(search)
        );
      }
      
      // Slice up the results for pagination
      const startIdx = (page - 1) * limit;
      const endIdx = startIdx + limit;
      
      return {
        data: results.slice(startIdx, endIdx),
        totalCount: results.length,
        page,
        pageSize: limit
      };
    } else {
      // Real mode - talk to the server
      const response = await fetch(
        `${API_URL}/users?page=${page}&pageSize=${limit}&search=${encodeURIComponent(searchText)}`
      );
      const result = await response.json();
      return result.data;
    }
  },
  
  async addUser(userData) {
    if (USE_MOCK_DATA) {
      const allUsers = JSON.parse(localStorage.getItem('users') || '[]');
      const newPerson = { 
        ...userData, 
        id: Date.now(), // Simple unique ID
        createdAt: new Date().toISOString()
      };
      allUsers.push(newPerson);
      localStorage.setItem('users', JSON.stringify(allUsers));
      return newPerson;
    } else {
      const response = await fetch(`${API_URL}/users`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData)
      });
      const result = await response.json();
      return result.data;
    }
  },
  
  async modifyUser(userId, userData) {
    if (USE_MOCK_DATA) {
      const allUsers = JSON.parse(localStorage.getItem('users') || '[]');
      const userIndex = allUsers.findIndex(person => person.id === userId);
      if (userIndex !== -1) {
        allUsers[userIndex] = { 
          ...allUsers[userIndex], 
          ...userData,
          updatedAt: new Date().toISOString()
        };
        localStorage.setItem('users', JSON.stringify(allUsers));
        return allUsers[userIndex];
      }
      throw new Error('Oops! User not found');
    } else {
      const response = await fetch(`${API_URL}/users/${userId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData)
      });
      const result = await response.json();
      return result.data;
    }
  }
};

const RegistrationApp = () => {
  // Which screen are we showing?
  const [activeScreen, setActiveScreen] = useState('form');
  
  // All our users
  const [userList, setUserList] = useState([]);
  
  // Are we editing someone?
  const [personBeingEdited, setPersonBeingEdited] = useState(null);
  
  // Feedback messages
  const [notification, setNotification] = useState(null);
  
  // Search stuff
  const [searchQuery, setSearchQuery] = useState('');
  
  // Pagination
  const [activePage, setActivePage] = useState(1);
  const [pageCount, setPageCount] = useState(1);
  const usersPerPage = 5;

  // Form fields
  const [formInfo, setFormInfo] = useState({
    name: '',
    email: '',
    phone: '',
    age: ''
  });

  // Validation errors
  const [fieldErrors, setFieldErrors] = useState({});

  // Load some sample users when app starts
  useEffect(() => {
    const savedUsers = localStorage.getItem('users');
    if (!savedUsers) {
      const demoUsers = [
        { id: 1, name: 'Sarah', email: 'sarah.j@email.com', phone: '+1 (555) 234-5678', age: 28, createdAt: '2024-01-15T10:30:00Z' },
        { id: 2, name: 'Mik', email: 'mik.chen@email.com', phone: '+1 (555) 345-6789', age: 35, createdAt: '2024-01-16T14:20:00Z' },
        { id: 3, name: 'Emman', email: 'emman.d@email.com', phone: '+1 (555) 456-7890', age: 24, createdAt: '2024-01-17T09:15:00Z' },
        { id: 4, name: 'James ', email: 'james.w@email.com', phone: '+1 (555) 567-8901', age: 42, createdAt: '2024-01-18T11:45:00Z' },
        { id: 5, name: 'mina', email: 'mina.m@email.com', phone: '+1 (555) 678-9012', age: 31, createdAt: '2024-01-19T16:00:00Z' },
        { id: 6, name: 'David', email: 'david.b@email.com', phone: '+1 (555) 789-0123', age: 29, createdAt: '2024-01-20T13:30:00Z' },
        { id: 7, name: 'Abdelrahman ', email: 'abdel.g@email.com', phone: '+1 (555) 890-1234', age: 38, createdAt: '2024-01-21T10:00:00Z' }
      ];
      localStorage.setItem('users', JSON.stringify(demoUsers));
    }

    if (activeScreen === 'list') {
      refreshUserList();
    }
  }, [activeScreen, activePage, searchQuery]);

  const refreshUserList = async () => {
    try {
      const response = await dataManager.fetchUsers(activePage, usersPerPage, searchQuery);
      setUserList(response.data);
      setPageCount(Math.ceil(response.totalCount / usersPerPage));
    } catch (error) {
      showNotification('Hmm, had trouble loading the users. Try again?', 'error');
    }
  };

  const checkFormValidity = () => {
    const issues = {};

    // Check the name
    if (!formInfo.name.trim()) {
      issues.name = "Hey, Please Enter your name!";
    } else if (formInfo.name.trim().length < 2) {
      issues.name = "Name's a bit too short - need at least 2 characters";
    }

    // Email validation
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!formInfo.email.trim()) {
      issues.email = "Don't forget the email!";
    } else if (!emailPattern.test(formInfo.email)) {
      issues.email = "That doesn't look like a valid email address";
    }

    // Phone check
    const phonePattern = /^[\d\s\-\+\(\)]{11,}$/;
    if (!formInfo.phone.trim()) {
      issues.phone = "Phone number is missing";
    } else if (!phonePattern.test(formInfo.phone.replace(/\s/g, ''))) {
      issues.phone = "Phone needs at least 11 digits";
    }

    // Age validation
    const ageNum = parseInt(formInfo.age);
    if (!formInfo.age) {
      issues.age = "Age is required";
    } else if (isNaN(ageNum) || ageNum < 1 || ageNum > 150) {
      issues.age = "Age should be between 1 and 150";
    }

    setFieldErrors(issues);
    return Object.keys(issues).length === 0;
  };

  const handleFieldChange = (e) => {
    const { name, value } = e.target;
    setFormInfo(prev => ({ ...prev, [name]: value }));
    
    // Clear error as user types
    if (fieldErrors[name]) {
      setFieldErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const showNotification = (text, type) => {
    setNotification({ text, type });
    setTimeout(() => setNotification(null), 5000);
  };

  const handleFormSubmit = async (e) => {
    e.preventDefault();
    
    if (!checkFormValidity()) {
      showNotification('Please fix the issues above', 'error');
      return;
    }

    try {
      if (personBeingEdited) {
        await dataManager.modifyUser(personBeingEdited.id, formInfo);
        showNotification('User updated successfully!', 'success');
      } else {
        await dataManager.addUser(formInfo);
        showNotification('🎉 New user registered!', 'success');
      }
      
      // Clear everything
      setFormInfo({ name: '', email: '', phone: '', age: '' });
      setPersonBeingEdited(null);
      
      // Refresh if we're on the list view
      if (activeScreen === 'list') {
        refreshUserList();
      }
    } catch (error) {
      showNotification('Oops! Something went wrong. Please trying again?', 'error');
    }
  };

  const startEditing = (user) => {
    setPersonBeingEdited(user);
    setFormInfo({
      name: user.name,
      email: user.email,
      phone: user.phone,
      age: user.age.toString()
    });
    setActiveScreen('form');
  };

  const cancelEditing = () => {
    setPersonBeingEdited(null);
    setFormInfo({ name: '', email: '', phone: '', age: '' });
    setFieldErrors({});
  };

  const handleSearchChange = (e) => {
    setSearchQuery(e.target.value);
    setActivePage(1); // Reset to first page when searching
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 py-8 px-4">
      <div className="max-w-6xl mx-auto">
        {/* Welcome Header */}
        <div className="bg-white rounded-xl shadow-lg p-8 mb-6 border-t-4 border-indigo-500">
          <div className="flex items-center gap-3 mb-3">
            <Sparkles className="text-indigo-600" size={32} />
            <h1 className="text-4xl font-bold text-gray-800">User Registration Portal</h1>
          </div>
          <p className="text-gray-600 text-lg">Welcome! Let's manage some awesome people 👋</p>
        </div>

        {/* Navigation Tabs */}
        <div className="bg-white rounded-xl shadow-lg p-6 mb-6">
          <div className="flex gap-4 flex-wrap">
            <button
              onClick={() => setActiveScreen('form')}
              className={`flex items-center gap-2 px-6 py-3 rounded-lg font-semibold transition-all transform hover:scale-105 ${
                activeScreen === 'form'
                  ? 'bg-gradient-to-r from-indigo-600 to-purple-600 text-white shadow-lg'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              <UserPlus size={20} />
              {personBeingEdited ? 'Edit User' : 'Add New User'}
            </button>
            <button
              onClick={() => setActiveScreen('list')}
              className={`flex items-center gap-2 px-6 py-3 rounded-lg font-semibold transition-all transform hover:scale-105 ${
                activeScreen === 'list'
                  ? 'bg-gradient-to-r from-indigo-600 to-purple-600 text-white shadow-lg'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              <Users size={20} />
              View Everyone
            </button>
          </div>
        </div>

        {/* Friendly Messages */}
        {notification && (
          <div className={`mb-6 p-5 rounded-xl flex items-center gap-3 shadow-md animate-in slide-in-from-top ${
            notification.type === 'success' 
              ? 'bg-green-50 border-l-4 border-green-500' 
              : 'bg-red-50 border-l-4 border-red-500'
          }`}>
            {notification.type === 'success' ? (
              <CheckCircle className="text-green-600 flex-shrink-0" size={24} />
            ) : (
              <AlertCircle className="text-red-600 flex-shrink-0" size={24} />
            )}
            <span className={`text-lg ${notification.type === 'success' ? 'text-green-800' : 'text-red-800'}`}>
              {notification.text}
            </span>
          </div>
        )}

        {/* Registration Form */}
        {activeScreen === 'form' && (
          <div className="bg-white rounded-xl shadow-lg p-8">
            <h2 className="text-3xl font-bold text-gray-800 mb-2">
              {personBeingEdited ? '✏️ Update Their Info' : '🌟 Register Someone New'}
            </h2>
            <p className="text-gray-600 mb-6">
              {personBeingEdited 
                ? 'Make any changes you need and hit update!' 
                : 'Fill in the details below to add a new person'}
            </p>
            
            <div className="space-y-6">
              {/* Name Field */}
              <div>
                <label className="block text-sm font-bold text-gray-700 mb-2">
                  Full Name <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="name"
                  value={formInfo.name}
                  onChange={handleFieldChange}
                  className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none focus:ring-2 transition-all ${
                    fieldErrors.name 
                      ? 'border-red-400 focus:ring-red-400 bg-red-50' 
                      : 'border-gray-300 focus:ring-indigo-400 focus:border-indigo-400'
                  }`}
                  placeholder="e.g., John Smith"
                />
                {fieldErrors.name && (
                  <p className="mt-2 text-sm text-red-600 flex items-center gap-1">
                    <AlertCircle size={16} />
                    {fieldErrors.name}
                  </p>
                )}
              </div>

              {/* Email Field */}
              <div>
                <label className="block text-sm font-bold text-gray-700 mb-2">
                  Email Address <span className="text-red-500">*</span>
                </label>
                <input
                  type="email"
                  name="email"
                  value={formInfo.email}
                  onChange={handleFieldChange}
                  className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none focus:ring-2 transition-all ${
                    fieldErrors.email 
                      ? 'border-red-400 focus:ring-red-400 bg-red-50' 
                      : 'border-gray-300 focus:ring-indigo-400 focus:border-indigo-400'
                  }`}
                  placeholder="e.g., john.smith@email.com"
                />
                {fieldErrors.email && (
                  <p className="mt-2 text-sm text-red-600 flex items-center gap-1">
                    <AlertCircle size={16} />
                    {fieldErrors.email}
                  </p>
                )}
              </div>

              {/* Phone Field */}
              <div>
                <label className="block text-sm font-bold text-gray-700 mb-2">
                  Phone Number <span className="text-red-500">*</span>
                </label>
                <input
                  type="tel"
                  name="phone"
                  value={formInfo.phone}
                  onChange={handleFieldChange}
                  className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none focus:ring-2 transition-all ${
                    fieldErrors.phone 
                      ? 'border-red-400 focus:ring-red-400 bg-red-50' 
                      : 'border-gray-300 focus:ring-indigo-400 focus:border-indigo-400'
                  }`}
                  placeholder="e.g., +1 (555) 123-4567"
                />
                {fieldErrors.phone && (
                  <p className="mt-2 text-sm text-red-600 flex items-center gap-1">
                    <AlertCircle size={16} />
                    {fieldErrors.phone}
                  </p>
                )}
              </div>

              {/* Age Field */}
              <div>
                <label className="block text-sm font-bold text-gray-700 mb-2">
                  Age <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  name="age"
                  value={formInfo.age}
                  onChange={handleFieldChange}
                  className={`w-full px-4 py-3 border-2 rounded-lg focus:outline-none focus:ring-2 transition-all ${
                    fieldErrors.age 
                      ? 'border-red-400 focus:ring-red-400 bg-red-50' 
                      : 'border-gray-300 focus:ring-indigo-400 focus:border-indigo-400'
                  }`}
                  placeholder="e.g., 25"
                  min="1"
                  max="150"
                />
                {fieldErrors.age && (
                  <p className="mt-2 text-sm text-red-600 flex items-center gap-1">
                    <AlertCircle size={16} />
                    {fieldErrors.age}
                  </p>
                )}
              </div>

              {/* Action Buttons */}
              <div className="flex gap-4 pt-4">
                <button
                  onClick={handleFormSubmit}
                  className="flex items-center gap-2 bg-gradient-to-r from-indigo-600 to-purple-600 text-white px-8 py-3 rounded-lg font-bold hover:from-indigo-700 hover:to-purple-700 transition-all shadow-lg transform hover:scale-105"
                >
                  <Save size={20} />
                  {personBeingEdited ? 'Update User' : 'Register User'}
                </button>
                
                {personBeingEdited && (
                  <button
                    onClick={cancelEditing}
                    className="flex items-center gap-2 bg-gray-500 text-white px-8 py-3 rounded-lg font-bold hover:bg-gray-600 transition-all shadow-md"
                  >
                    <X size={20} />
                    Cancel
                  </button>
                )}
              </div>
            </div>
          </div>
        )}

        {/* User List */}
        {activeScreen === 'list' && (
          <div className="bg-white rounded-xl shadow-lg p-8">
            <div className="flex justify-between items-center mb-6 flex-wrap gap-4">
              <div>
                <h2 className="text-3xl font-bold text-gray-800">Our Amazing People</h2>
                <p className="text-gray-600 mt-1">Everyone who's registered with us</p>
              </div>
              
              {/* Search Box */}
              <div className="relative w-full md:w-80">
                <Search className="absolute left-3 top-3.5 text-gray-400" size={20} />
                <input
                  type="text"
                  value={searchQuery}
                  onChange={handleSearchChange}
                  placeholder="Search by name or email..."
                  className="w-full pl-10 pr-4 py-3 border-2 border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:border-indigo-400"
                />
              </div>
            </div>

            {userList.length === 0 ? (
              <div className="text-center py-16">
                <Users className="mx-auto text-gray-300 mb-4" size={64} />
                <p className="text-gray-500 text-xl mb-2">No users found</p>
                <p className="text-gray-400">
                  {searchQuery ? 'Try a different search term' : 'Add your first user to get started!'}
                </p>
              </div>
            ) : (
              <>
                {/* Desktop Table */}
                <div className="hidden md:block overflow-x-auto rounded-lg border border-gray-200">
                  <table className="w-full">
                    <thead className="bg-gradient-to-r from-indigo-50 to-purple-50">
                      <tr>
                        <th className="text-left py-4 px-6 font-bold text-gray-700">Name</th>
                        <th className="text-left py-4 px-6 font-bold text-gray-700">Email</th>
                        <th className="text-left py-4 px-6 font-bold text-gray-700">Phone</th>
                        <th className="text-left py-4 px-6 font-bold text-gray-700">Age</th>
                        <th className="text-left py-4 px-6 font-bold text-gray-700">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {userList.map((person) => (
                        <tr key={person.id} className="border-t border-gray-100 hover:bg-indigo-50 transition-colors">
                          <td className="py-4 px-6 text-gray-800 font-medium">{person.name}</td>
                          <td className="py-4 px-6 text-gray-600">{person.email}</td>
                          <td className="py-4 px-6 text-gray-600">{person.phone}</td>
                          <td className="py-4 px-6 text-gray-600">{person.age}</td>
                          <td className="py-4 px-6">
                            <button
                              onClick={() => startEditing(person)}
                              className="flex items-center gap-2 bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-all transform hover:scale-105 shadow-sm"
                            >
                              <Edit2 size={16} />
                              Edit
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {/* Mobile Cards */}
                <div className="md:hidden space-y-4">
                  {userList.map((person) => (
                    <div key={person.id} className="border-2 border-gray-200 rounded-lg p-5 hover:border-indigo-300 transition-all bg-gradient-to-br from-white to-indigo-50">
                      <div className="space-y-3">
                        <div>
                          <span className="text-xs font-bold text-gray-500 uppercase">Name</span>
                          <p className="text-gray-800 font-semibold text-lg">{person.name}</p>
                        </div>
                        <div>
                          <span className="text-xs font-bold text-gray-500 uppercase">Email</span>
                          <p className="text-gray-600">{person.email}</p>
                        </div>
                        <div className="flex gap-4">
                          <div className="flex-1">
                            <span className="text-xs font-bold text-gray-500 uppercase">Phone</span>
                            <p className="text-gray-600">{person.phone}</p>
                          </div>
                          <div>
                            <span className="text-xs font-bold text-gray-500 uppercase">Age</span>
                            <p className="text-gray-600">{person.age}</p>
                          </div>
                        </div>
                        <button
                          onClick={() => startEditing(person)}
                          className="w-full flex items-center justify-center gap-2 bg-indigo-600 text-white px-4 py-3 rounded-lg hover:bg-indigo-700 transition-all transform hover:scale-105 shadow-md font-semibold mt-4"
                        >
                          <Edit2 size={18} />
                          Edit This Person
                        </button>
                      </div>
                    </div>
                  ))}
                </div>

                {/* Pagination Controls */}
                {pageCount > 1 && (
                  <div className="flex items-center justify-center gap-6 mt-8 pt-6 border-t-2 border-gray-200">
                    <button
                      onClick={() => setActivePage(prev => Math.max(1, prev - 1))}
                      disabled={activePage === 1}
                      className="flex items-center gap-2 px-6 py-3 rounded-lg bg-gray-100 text-gray-700 font-semibold disabled:opacity-40 disabled:cursor-not-allowed hover:bg-gray-200 transition-all transform hover:scale-105 disabled:hover:scale-100 shadow-sm"
                    >
                      <ChevronLeft size={20} />
                      Previous
                    </button>
                    
                    <div className="text-center">
                      <span className="text-gray-700 font-bold text-lg">
                        Page {activePage} of {pageCount}
                      </span>
                      <p className="text-gray-500 text-sm mt-1">
                        Showing {userList.length} users
                      </p>
                    </div>
                    
                    <button
                      onClick={() => setActivePage(prev => Math.min(pageCount, prev + 1))}
                      disabled={activePage === pageCount}
                      className="flex items-center gap-2 px-6 py-3 rounded-lg bg-gray-100 text-gray-700 font-semibold disabled:opacity-40 disabled:cursor-not-allowed hover:bg-gray-200 transition-all transform hover:scale-105 disabled:hover:scale-100 shadow-sm"
                    >
                      Next
                      <ChevronRight size={20} />
                    </button>
                  </div>
                )}
              </>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default RegistrationApp;
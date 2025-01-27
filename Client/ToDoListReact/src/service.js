import axios from 'axios';

axios.defaults.baseURL=process.env.REACT_APP_API_URL;

axios.interceptors.response.use(
  (response) => {
    // אם התגובה תקינה, פשוט מחזירים אותה
    return response;
  },
  (error) => {
    // אם התגובה מכילה שגיאה, נרשום אותה ללוג
    console.error('Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`/tasks`)    
    console.log('get all tasks');
    return result.data;
  },

  addTask: async(name)=>{
    const result = await axios.post(`/tasks`,{ name:name , isComplete: false })
    console.log('added task ');
    return result.data;
  },

  setCompleted: async(id, isComplete)=>{
    const result = await axios.put(`/tasks/${id}`,{isComplete:isComplete})
    console.log('setCompleted', {id, isComplete})
    return result.data;
  },

  deleteTask:async(id)=>{
    const result = await axios.delete(`/tasks/${id}`)
    console.log('deleteTask')
  }
};

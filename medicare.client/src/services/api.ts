import axios from "axios"

const api = axios.create({
    baseURL: "https://localhost:7014/api",
})

api.interceptors.response.use(
    response => response,
    error => {
        if (error.response?.status === 401 || error.response?.status === 403) {
            localStorage.removeItem("token");
            window.location.href = "/";
        }
        return Promise.reject(error);
    }
);
export default api;
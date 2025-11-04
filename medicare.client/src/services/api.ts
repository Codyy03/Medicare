import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:7014/api",
});

api.interceptors.request.use(
    config => {
        const token = localStorage.getItem("token");
        if (token) {
            config.headers["Authorization"] = "Bearer " + token;
        }
        return config;
    },
    error => {
        return Promise.reject(error);
    }
);

let isRefreshing = false;
let refreshSubscribers: ((token: string) => void)[] = [];

function onRefreshed(token: string) {
    refreshSubscribers.forEach(cb => cb(token));
    refreshSubscribers = [];
}

api.interceptors.response.use(
    response => response,
    async error => {
        const originalRequest = error.config;

        if ((error.response?.status === 401 || error.response?.status === 403) &&
            !originalRequest._retry &&
            !originalRequest.url.includes("/Auth/refresh")) {

            if (isRefreshing) {
                return new Promise(resolve => {
                    refreshSubscribers.push((token: string) => {
                        originalRequest.headers["Authorization"] = "Bearer " + token;
                        resolve(api(originalRequest));
                    });
                });
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                const refreshToken = localStorage.getItem("refreshToken");
                const res = await api.post("/Auth/refresh", { refreshToken });
                const { accessToken, refreshToken: newRefreshToken } = res.data;

                localStorage.setItem("token", accessToken);
                localStorage.setItem("refreshToken", newRefreshToken);

                api.defaults.headers.common["Authorization"] = "Bearer " + accessToken;
                onRefreshed(accessToken);

                return api(originalRequest);
            } catch (err) {
                localStorage.removeItem("token");
                localStorage.removeItem("refreshToken");
                window.location.href = "/";
                return Promise.reject(err);
            } finally {
                isRefreshing = false;
            }
        }

        if (error.response?.status === 500) {
            window.location.href = "/server-error";
        }
        if (error.response?.status === 404) {
            window.location.href = "/not-found";
        }

        return Promise.reject(error);
    }
);


export default api;
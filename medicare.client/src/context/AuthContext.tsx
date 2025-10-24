import { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

interface JwtPayload {
    name?: string;
    unique_name?: string;
    email?: string;
    exp?: number;
    role?: string;
    [key: string]: any;
}

interface AuthContextType {
    userName: string | null;
    userRole: string | null;
    loading: boolean;
    logout: () => void;
    login: (accessToken: string, refreshToken: string) => void;
    refreshAccessToken: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [userName, setUserName] = useState<string | null>(null);
    const [userRole, setUserRole] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const loadUser = () => {
            const token = localStorage.getItem("token");
            if (token) {
                try {
                    const decoded = jwtDecode<JwtPayload>(token);

                    if (decoded.exp && decoded.exp * 1000 < Date.now()) {
                        refreshAccessToken().finally(() => setLoading(false));
                        return;
                    }

                    setDecodedUser(decoded);

                    if (decoded.exp) {
                        const timeout = decoded.exp * 1000 - Date.now() - 5000;
                        setTimeout(() => {
                            refreshAccessToken();
                        }, timeout);
                    }
                } catch {
                    logout();
                }
            } else {
                logout();
            }
            setLoading(false);
        };

        loadUser();
        window.addEventListener("storage", loadUser);
        return () => window.removeEventListener("storage", loadUser);
    }, []);

    const setDecodedUser = (decoded: JwtPayload) => {
        const name =
            decoded.name ??
            decoded.unique_name ??
            decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ??
            null;

        const role =
            decoded.role ??
            decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
            null;

        setUserName(name);
        setUserRole(role);
    };

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        setUserName(null);
        setUserRole(null);
    };

    const login = (accessToken: string, refreshToken: string) => {
        localStorage.setItem("token", accessToken);
        localStorage.setItem("refreshToken", refreshToken);
        try {
            const decoded = jwtDecode<JwtPayload>(accessToken);
            setDecodedUser(decoded);
        } catch {
            logout();
        }
    };

    const refreshAccessToken = async () => {
        const refreshToken = localStorage.getItem("refreshToken");
        if (!refreshToken) {
            logout();
            return;
        }

        try {
            const res = await fetch("/api/Auth/refresh", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ refreshToken }),
            });

            if (!res.ok) {
                logout();
                return;
            }

            const data = await res.json();
            const newAccessToken = data.accessToken;

            localStorage.setItem("token", newAccessToken);

            const decoded = jwtDecode<JwtPayload>(newAccessToken);
            setDecodedUser(decoded);

            if (decoded.exp) {
                const timeout = decoded.exp * 1000 - Date.now() - 5000;
                setTimeout(() => {
                    refreshAccessToken();
                }, timeout);
            }
        } catch {
            logout();
        }
    };

    return (
        <AuthContext.Provider value={{ userName, userRole, loading, logout, login, refreshAccessToken }}>
            {children}
        </AuthContext.Provider>
    );
};

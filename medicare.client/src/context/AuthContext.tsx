import { createContext, useContext, useState, useEffect } from "react";
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
    login: (token: string) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

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
                        logout();
                        setLoading(false);
                        return;
                    }

                    // support for different claim names
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

                    // set a timer to automatically log out
                    if (decoded.exp) {
                        const timeout = decoded.exp * 1000 - Date.now();
                        setTimeout(() => {
                            logout();
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

    const logout = () => {
        localStorage.removeItem("token");
        setUserName(null);
        setUserRole(null);
    };

    const login = (token: string) => {
        localStorage.setItem("token", token);
        try {
            const decoded = jwtDecode<JwtPayload>(token);

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
        } catch {
            logout();
        }
    };

    return (
        <AuthContext.Provider value={{ userName, userRole, loading, logout, login }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
    return ctx;
};

import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import type { JSX } from "react";

interface ProtectedRouteProps {
    children: JSX.Element;
    role?: string;
}

export default function ProtectedRoute({ children, role }: ProtectedRouteProps) {
    const { userName, userRole, loading } = useAuth();

    if (loading) {
        return <p>Loading...</p>;
    }

    if (!userName) {
        return <Navigate to="/login/patient" replace />;
    }

    if (role && userRole !== role) {
        return <Navigate to="/" replace />;
    }

    return children;
}

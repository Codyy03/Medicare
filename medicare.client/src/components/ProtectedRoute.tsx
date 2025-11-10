import { Navigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";
import type { JSX } from "react";

interface ProtectedRouteProps {
    children: JSX.Element;
    role?: string | string[]; // mo¿na podaæ jedn¹ rolê albo listê ról
}

export default function ProtectedRoute({ children, role }: ProtectedRouteProps) {
    const { userName, userRole, loading } = useAuth();

    if (loading) {
        return <p>Loading...</p>;
    }

    if (!userName) {
        return <Navigate to="/" replace />;
    }

    if (role) {
        const allowedRoles = Array.isArray(role) ? role : [role];
        if (!allowedRoles.includes(userRole || "")) {
            return <Navigate to="/unauthorized" replace />;
        }
    }

    return children;
}

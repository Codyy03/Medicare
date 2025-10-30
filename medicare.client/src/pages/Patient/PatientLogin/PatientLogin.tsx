import { useState } from "react"
import { NavLink, useNavigate } from "react-router-dom";
import axios from "axios";
import { useAuth } from "../../../context/useAuth";
import "./PatientLogin.css";

export default function PatientLogin() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const { login } = useAuth();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const res = await axios.post("https://localhost:7014/api/patients/login", { email, password });
            const { accessToken, refreshToken } = res.data;

            if (accessToken && refreshToken) {
                login(accessToken, refreshToken);
                navigate("/");
            } else {
                setError("Invalid email or password");
            }
        } catch (err) {
            if (axios.isAxiosError(err)) {
                const message = err.response?.data;
                if (typeof message === "string") {
                    setError(message);
                }
            } else {
                setError("Unexpected error occurred.");
            }
        }
    };

    return (
        <div className="login-container">
            <div className="login-box">
                <h2 className="login-title">Patient Login</h2>
                <p className="login-subtitle">Log in to access your dashboard</p>

                <form className="login-form" onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="email">Email address</label>
                        <input
                            type="email"
                            id="email"
                            className="form-control"
                            placeholder="e.g. john@gmail.com"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            type="password"
                            id="password"
                            className="form-control"
                            placeholder="&bull;&bull;&bull;&bull;&bull;"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    {error && <p className="error-text">{error}</p>}

                    <button type="submit" className="btn btn-primary w-100 mt-3">
                        Login
                    </button>
                </form>

                <div className="login-footer">
                    <p>Don't have an account yet?</p>
                    <NavLink to="/register/patient" className="btn btn-outline-primary w-100">
                        Register
                    </NavLink>
                </div>
            </div>
        </div>
    );
}

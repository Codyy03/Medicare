import { useState } from "react"
import { NavLink, useNavigate } from "react-router-dom";
import { loginDoctor } from "../../services/authSevice"
import "./DoctorLogin.css";

export default function DoctorLogin() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const token = await loginDoctor({ email, password });

        if (token) {
            localStorage.setItem("token", token);
            navigate("/");
        } else {
            setError("Invalid email or password");
        }
    };

    return (
        <div className="login-container doctor-login">
            <div className="login-box">
                <h2 className="login-title">
                    <i className="bi bi-hospital me-2"></i> Doctor Login
                </h2>
                <p className="login-subtitle">
                    Log in to access your doctor dashboard
                </p>

                <form className="login-form" onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="email">Work email address</label>
                        <input
                            type="email"
                            id="email"
                            className="form-control"
                            placeholder="e.g. joe@medicare.pl"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
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
                        />
                    </div>

                    {error && <p className="error-text">{error}</p>}

                    <button type="submit" className="btn btn-dark w-100 mt-3">
                        Login
                    </button>
                </form>

                <div className="login-footer">
                    <p>Don't have an account yet?</p>
                    <NavLink to="/register/doctor" className="btn btn-outline-dark w-100">
                        Register as a doctor
                    </NavLink>
                </div>
            </div>
        </div>
    );
}

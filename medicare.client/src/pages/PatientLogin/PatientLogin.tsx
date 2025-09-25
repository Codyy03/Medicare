import { NavLink } from "react-router-dom";
import "./PatientLogin.css";

export default function PatientLogin() {
    return (
        <div className="login-container">
            <div className="login-box">
                <h2 className="login-title">Patient Login</h2>
                <p className="login-subtitle">Log in to access your dashboard</p>

                <form className="login-form">
                    <div className="form-group">
                        <label htmlFor="email">Email address</label>
                        <input type="email" id="email" className="form-control" placeholder="e.g. john@gmail.com" />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input type="password" id="password" className="form-control" placeholder="&bull;&bull;&bull;&bull;&bull;" />
                    </div>

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

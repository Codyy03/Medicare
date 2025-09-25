import { NavLink } from "react-router-dom";
import "./DoctorLogin.css";

export default function DoctorLogin() {
    return (
        <div className="login-container doctor-login">
            <div className="login-box">
                <h2 className="login-title">
                    <i className="bi bi-hospital me-2"></i> Doctor Login
                </h2>
                <p className="login-subtitle">
                    Log in to access your doctor dashboard
                </p>

                <form className="login-form">
                    <div className="form-group">
                        <label htmlFor="email">Work email address</label>
                        <input
                            type="email"
                            id="email"
                            className="form-control"
                            placeholder="e.g. joe@medicare.pl"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            type="password"
                            id="password"
                            className="form-control"
                            placeholder="&bull;&bull;&bull;&bull;&bull;"
                        />
                    </div>

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

import { NavLink, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { useAuth } from "../../context/useAuth";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import './Header.css';

export default function Header() {
    const { userName, userRole, logout } = useAuth();
    const [showLoginOptions, setShowLoginOptions] = useState(false);
    const rolePaths: Record<string, string> = {
        Doctor: "/personalDataDoctor",
        Patient: "/personalData"
    };
    const navigate = useNavigate();

    useEffect(() => {
        const updatePadding = () => {
            const header = document.querySelector("header");
            if (header) {
                document.body.style.paddingTop = header.offsetHeight + "px";
            }
        };
        updatePadding();
        window.addEventListener("resize", updatePadding);
        return () => window.removeEventListener("resize", updatePadding);
    }, []);

    const socials = ["facebook", "twitter", "instagram"];

    return (
        <header>
            {/* Topbar */}
            <div className="topbar text-white py-3">
                <div className="container d-flex justify-content-between align-items-center">
                    {/* Logo + slogan */}
                    <div className="d-flex align-items-center gap-3">
                        <NavLink className="navbar-brand d-flex align-items-center" to="/">
                            <img src="/logo.png" alt="Logo" width="60" height="60" className="me-2" />
                            <span className="fw-bold text-white fs-5">MediCare</span>
                        </NavLink>
                        <div className="slogan">
                            <div className="fw-bold">MediCare Medical Center</div>
                            <div className="tagline">With your health in mind</div>
                        </div>
                    </div>

                    {/* Social media + search */}
                    <div className="d-flex flex-column align-items-center">
                        <div className="d-flex mb-2 gap-3">
                            {socials.map(icon => (
                                <a key={icon} href="#" className="text-white">
                                    <i className={`bi bi-${icon} fs-5`}></i>
                                </a>
                            ))}
                        </div>
                        <form className="d-flex search-form">
                            <input className="form-control form-control-sm me-2" type="search" placeholder="Find..." />
                            <button className="btn btn-light btn-sm search-btn" type="submit">
                                <i className="bi bi-search"></i>
                            </button>
                        </form>
                    </div>
                </div>
            </div>

            {/* Navigation */}
            <nav className="navbar navbar-expand-lg navbar-light bg-light px-3 border-bottom">
                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#mainNavbar"
                    aria-controls="mainNavbar"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div className="collapse navbar-collapse" id="mainNavbar">
                    <ul className="navbar-nav mx-auto">
                        <li className="nav-item dropdown">
                            <div className="nav-link dropdown-toggle" id="infoDropdown">Info</div>
                            <ul className="dropdown-menu show-on-hover" aria-labelledby="infoDropdown">
                                <li><NavLink className="dropdown-item hover-slide" to="/aboutUs">About us</NavLink></li>
                                <li><NavLink className="dropdown-item hover-slide" to="/contact">Contact</NavLink></li>
                                <li><NavLink className="dropdown-item hover-slide" to="/allNews">News</NavLink></li>
                            </ul>
                        </li>

                        <li className="nav-item"><NavLink className="nav-link" to="/doctors">Doctors</NavLink></li>
                        <li className="nav-item"><NavLink className="nav-link" to={userRole === "Doctor" ? "/doctorAppointments" : "/appointments"}>Appointment</NavLink></li>

                        <li className="nav-item">
                            {userName ? (
                                <div className="nav-item dropdown">
                                    <div className="nav-link dropdown-toggle" id="userDropdown" role="button" data-bs-toggle="dropdown">
                                        {userRole === "Doctor" ? `Dr. ${userName}` : userName}
                                    </div>
                                    <ul className="dropdown-menu show-on-hover" aria-labelledby="userDropdown">
                                        {userRole !== "Admin" ? (
                                            <>
                                                <li>
                                                    <NavLink className="dropdown-item hover-slide" to={rolePaths[userRole ?? "Patient"]}>
                                                        My personal data
                                                    </NavLink>
                                                </li>
                                                <li>
                                                    <NavLink className="dropdown-item hover-slide" to={userRole === "Doctor" ? "/doctor/visits" : "/patient/visits"}>
                                                        My visits
                                                    </NavLink>
                                                </li>
                                            </>
                                        ) : (
                                            <li>
                                                <NavLink className="dropdown-item hover-slide" to="/admin">
                                                    Admin page
                                                </NavLink>
                                            </li>
                                        )}

                                        <li>
                                            <button
                                                className="dropdown-item hover-slide"
                                                onClick={() => {
                                                    logout();
                                                    navigate("/");
                                                    setShowLoginOptions(false);
                                                }}
                                            >
                                                Logout
                                            </button>
                                        </li>
                                    </ul>
                                </div>
                            ) : (
                                <button className="nav-link login-btn" onClick={() => setShowLoginOptions(true)}>
                                    Login
                                </button>
                            )}
                        </li>
                    </ul>
                </div>
            </nav>

            {/* Login overlay */}
            {showLoginOptions && (
                <div className="login-overlay">
                    <div className="login-box">
                        <button className="close-btn" onClick={() => setShowLoginOptions(false)}>
                            <i className="bi bi-x-lg"></i>
                        </button>
                        <h4 className="mb-4">Who are you:</h4>
                        <div className="login-choices">
                            <NavLink to="/login/patient" className="login-choice" onClick={() => setShowLoginOptions(false)}>
                                <i className="bi bi-person-heart fs-1 mb-2"></i>
                                Patient
                            </NavLink>
                            <NavLink to="/login/doctor" className="login-choice" onClick={() => setShowLoginOptions(false)}>
                                <i className="bi bi-hospital fs-1 mb-2"></i>
                                Doctor
                            </NavLink>
                        </div>
                    </div>
                </div>
            )}
        </header>
    );
}

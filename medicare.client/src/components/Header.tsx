import { NavLink } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import './Header.css';

export default function Header() {
    const socials = ["facebook", "twitter", "instagram"];

    return (
        <header>
            {/* Topbar */}
            <div className="topbar text-white py-3">
                <div className="container d-flex justify-content-between align-items-center">

                    {/* Logo + slogan */}
                    <div className="d-flex align-items-center gap-3">
                        <NavLink className="navbar-brand d-flex align-items-center" to="/">
                            <img
                                src="/logo.png"
                                alt="Logo"
                                width="60"
                                height="60"
                                className="me-2"
                            />
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
                            <input
                                className="form-control form-control-sm me-2"
                                type="search"
                                placeholder="Find..."
                            />
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
                        <li className="nav-item">
                            <NavLink className="nav-link" to="/about">About us</NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink className="nav-link" to="/doctors">Doctors</NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink className="nav-link" to="/appointments">Appointment</NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink className="nav-link" to="/registration">Registration</NavLink>
                        </li>
                    </ul>
                </div>
            </nav>
        </header>
    );
}

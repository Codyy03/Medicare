import { Link } from "react-router-dom";
import "./Footer.css";

function Footer() {
    return (
        <footer className="footer bg-light pt-5">
            <div className="container">
                <div className="row">
                    {/* Column 1 – contact */}
                    <div className="col-md-4 mb-4">
                        <h5>MediCare Medical Center</h5>
                        <p>Superb Street 1<br />35-505 Rzeszow</p>
                        <p>6532 542 5123<br />medicare@email.com</p>
                    </div>

                    {/* Column 2 –links */}
                    <div className="col-md-2 mb-4">
                        <h5>MediCare</h5>
                        <ul className="list-unstyled">
                            <li><Link to="/aboutUs" className="info">About us</Link></li>
                            <li><Link to="/contact" className="info">Contact</Link></li>
                        </ul>
                    </div>

                    {/* Column 3 – links for patients */}
                    <div className="col-md-3 mb-4">
                        <h5>For patients</h5>
                        <ul className="list-unstyled">
                            <li><Link to="/testResylt" className="info">Test results</Link></li>
                            <li><Link to="/register/patient" className="info">Registration</Link></li>
                        </ul>
                    </div>

                    {/* Column 4 – logo NFZ + social media */}
                    <div className="col-md-3 mb-4 text-center">
                        <img src="/nfz-logo.png" alt="NFZ" className="img-fluid mb-3"/>
                        <div className="social-icons">
                            <a href="#"><i className="bi bi-facebook"></i></a>
                            <a href="#"><i className="bi bi-instagram"></i></a>
                            <a href="#"><i className="bi bi-youtube"></i></a>
                        </div>
                    </div>
                </div>

                <div className="footer-bottom text-center py-3 mt-4 border-top">
                    <small>{new Date().getFullYear()} MediCare Medical Center. All rights reserved.</small>
                </div>
            </div>
        </footer>
    );
}

export default Footer;

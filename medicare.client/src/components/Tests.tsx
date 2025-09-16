import { Link } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import './Tests.css';
function Tests() {

    const services = [
        {
            title: "Cardiologist",
            text: "Protect your heart with expert cardiovascular care and diagnostics.",
            link: "#"
        },
        {
            title: "Dermatologist",
            text: "Healthy skin starts here comprehensive dermatological treatments for all ages.",
            link: "#"
        },
        {
            title: "Orthopedic Surgeon",
            text: "Restore mobility and strength with advanced orthopedic solutions.",
            link: "#"
        }
    ];

    return (
        <div className="container my-5">
            <div className="row g-4">
                {services.map((service, index) => (
                    <div className="col-md-4" key={index}>
                        <Link to={service.link} className="text-decoration-none text-dark">
                            <div className="card h-100 text-center service-card">
                                <div className="card-body">
                                    <h5 className="card-title">{service.title}</h5>
                                    <p className="card-text">{service.text}</p>
                                </div>
                            </div>
                        </Link>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Tests;
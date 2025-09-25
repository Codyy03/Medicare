import { Link } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import './Tests.css';
import { useEffect, useState } from 'react';
import { getSpecializations } from "../services/specializationsService";

interface Specializations {
    specializationName: string;
    specializationHighlight: string;
    link: string;
}
function Tests() {
    const [specializations, setSpecializations] = useState<Specializations[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getSpecializations()
            .then(data => setSpecializations(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));

    }, []);

    if (loading) return <p>Loading...</p>;
    return (
        <div className="container my-5">
            <div className="row g-4">
                {specializations.map((service, index) => (
                    <div className="col-md-4" key={index}>
                        <Link to={service.link} className="text-decoration-none text-dark">
                            <div className="card h-100 text-center service-card">
                                <div className="card-body">
                                    <h5 className="card-title">{service.specializationName}</h5>
                                    <p className="card-text">{service.specializationHighlight}</p>
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
import React, { useEffect, useState } from "react";
import { Link, useLocation, useParams } from "react-router-dom";
import { getDoctorById } from "../../../services/doctorsService";
import type { DoctorDto } from "../../../interfaces/doctors.types";
import "./DoctorInfo.css";

const DoctorInfo: React.FC = () => {
    const { id } = useParams();
    const location = useLocation();
    const [doctor, setDoctor] = useState<DoctorDto | null>(
        location.state?.doctor || null
    );

    useEffect(() => {
      if (id) {
        getDoctorById(parseInt(id)).then(data => {
          setDoctor(prev => ({
            ...prev,
            ...data
          }));
        });
      }
    }, [id]);

    if (!doctor) return <p>Doctor not found</p>;

    return (
        <main className="doctor-profile container my-5">
            <div className="profile-grid">
                {/* Row 1: Contact + About */}
                <div className="profile-card shadow-sm">
                    <h5 className="section-title">Contact Information</h5>
                    <ul className="list-unstyled">
                        <li><i className="bi bi-envelope-fill me-2 text-primary"></i> {doctor.email}</li>
                        <li><i className="bi bi-telephone-fill me-2 text-primary"></i> {doctor.phoneNumber}</li>
                        <li><i className="bi bi-geo-alt-fill me-2 text-primary"></i>{doctor.facility}</li>
                    </ul>
                </div>

                <div className="profile-card shadow-sm">
                    <h5 className="section-title">About the Doctor</h5>
                    <p>{doctor.doctorDescription} </p>
                </div>

                {/* Row 2: Hours + Specializations */}
                <div className="profile-card shadow-sm">
                    <h5 className="section-title">Working Hours</h5>
                    <p className="mb-1">Monday &ndash; Friday</p>
                    <p className="fw-bold"> {doctor.startHour.slice(0, 5)} - {" "} {doctor.endHour.slice(0, 5)}</p>
                </div>

                <div className="profile-card shadow-sm">
                    <h5 className="section-title">Specializations</h5>
                    <div className="d-flex flex-wrap gap-2">
                        {doctor.specializations.map((spec, i) => (
                            <span key={i} className="badge rounded-pill bg-info text-dark px-3 py-2">
                                {spec}
                            </span>
                        ))}
                    </div>
                </div>
            </div>

            <div className="text-center mt-5">
                <Link to="/appointments" className="btn btn-lg btn-success px-4">
                    Book Appointment
                </Link>
            </div>
        </main>

    );
};

export default DoctorInfo;

import { Link } from "react-router-dom"
import { useEffect, useState } from "react";
import { getPatientMe } from "../../services/patientsService"
export default function PatientProfile() {
    interface Patient {
        id: number;
        name: string;
        surname: string;
        pesel: string;
        birthday: string;
        email: string;
        phoneNumber: string;
    }

    const [patient, setPatient] = useState<Patient>();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getPatientMe().
            then(data => setPatient(data)).
            catch(err => console.error(err)).
            finally(() => setLoading(false));
    },[]);

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-5">
            <h2 className="mb-4 text-center">
                <i className="bi bi-person-heart me-2"></i> My Personal Data
            </h2>

            <div className="card shadow-sm">
                <div className="card-body">
                    {/* Basic Information */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-person-badge me-2"></i> Basic Information
                    </h5>
                    <dl className="row mb-4">
                        <dt className="col-sm-3">Name</dt>
                        <dd className="col-sm-9">{patient?.name} {patient?.surname}</dd>

                        <dt className="col-sm-3">PESEL</dt>
                        <dd className="col-sm-9">{patient?.pesel}</dd>

                        <dt className="col-sm-3">Birthday</dt>
                        <dd className="col-sm-9">
                            {patient ? new Date(patient.birthday).toLocaleDateString() : ""}
                        </dd>
                    </dl>

                    {/* Contact Details */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-envelope me-2"></i> Contact Details
                    </h5>
                    <dl className="row mb-4">
                        <dt className="col-sm-3">Email</dt>
                        <dd className="col-sm-9">{patient?.email}</dd>

                        <dt className="col-sm-3">Phone</dt>
                        <dd className="col-sm-9">{patient?.phoneNumber}</dd>
                    </dl>

                    {/* Security */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-shield-lock me-2"></i> Security
                    </h5>

                    <div className="row align-items-center mb-3">
                        <div className="col-sm-3 fw-bold">Password</div>
                        <div className="col-sm-9">&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;</div>
                    </div>

                    <div className="row align-items-center">
                        <div className="col-sm-3 fw-bold">Reset Password</div>
                        <div className="col-sm-9">
                            <Link to="/reset-password" className="btn btn-outline-primary btn-sm">
                                <i className="bi bi-arrow-repeat me-1"></i> Reset Password
                            </Link>
                        </div>
                    </div>
                </div>

                <div className="card-footer d-flex justify-content-center bg-light">
                    <a href="/doctor/edit" className="btn btn-outline-primary px-4 py-2 rounded-pill shadow-sm">
                        <i className="bi bi-pencil-square me-2"></i> Edit Data
                    </a>
                </div>

            </div>
        </div>
    );
}
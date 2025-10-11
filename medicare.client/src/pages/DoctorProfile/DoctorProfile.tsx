import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getDoctorMe } from "../../services/doctorsService"
import 'bootstrap/dist/css/bootstrap.min.css';
export default function DoctorProfile() {
    interface DoctorDto {
        name: string;
        surname: string;
        startHour: string;
        endHour: string;
        email: string;
        phoneNumber: string;
        specializations: string[];
    } 

    const [doctor, setDoctor] = useState<DoctorDto>();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getDoctorMe().
            then(data => setDoctor(data)).
            catch(err => console.error(err)).
            finally(() => setLoading(false));
    }, []);

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-5">
            <h2 className="mb-4 text-center">
                <i className="bi bi-person-badge me-2"></i> Doctor Profile
            </h2>

            <div className="card shadow-sm">
                <div className="card-body">
                     {/* Basic Information */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-person me-2"></i> Basic Information
                    </h5>
                    <div className="row mb-2">
                        <div className="col-sm-3 fw-bold">Name</div>
                        <div className="col-sm-9">{doctor?.name} {doctor?.surname}</div>
                    </div>
                    <div className="row mb-2">
                        <div className="col-sm-3 fw-bold">Specializations</div>
                        <div className="col-sm-9"> {doctor?.specializations.join(", ")}</div>
                    </div>
                    <div className="row mb-4">
                        <div className="col-sm-3 fw-bold">Working Hours</div>
                        <div className="col-sm-9">{doctor?.startHour.slice(0, 5)} - {doctor?.endHour.slice(0, 5)}</div>
                    </div>

                    {/*Contact Details */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-envelope me-2"></i> Contact Details
                    </h5>
                    <div className="row mb-2">
                        <div className="col-sm-3 fw-bold">Email</div>
                        <div className="col-sm-9">{doctor?.email}</div>
                    </div>
                    <div className="row mb-4">
                        <div className="col-sm-3 fw-bold">Phone</div>
                        <div className="col-sm-9">{doctor?.phoneNumber}</div>
                    </div>

                    {/* Security */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-shield-lock me-2"></i> Security
                    </h5>
                    <div className="row mb-2">
                        <div className="col-sm-3 fw-bold">Password</div>
                        <div className="col-sm-9">&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;</div>
                    </div>
                    <div className="row">
                        <div className="col-sm-3 fw-bold">Reset Password</div>
                        <div className="col-sm-9">
                            <Link to="/resetPasswordDoctor" className="btn btn-outline-primary btn-sm">
                                <i className="bi bi-arrow-repeat me-1"></i> Reset Password
                            </Link>
                        </div>
                    </div>
                </div>
                <div className="card-footer d-flex justify-content-center bg-light">
                    <Link to="/doctor/edit" className="btn btn-outline-primary px-4 py-2 rounded-pill shadow-sm">
                        <i className="bi bi-pencil-square me-2"></i> Edit Data
                    </Link>
                </div>
            </div>
        </div>
    );
}
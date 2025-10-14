import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getDoctorMe } from "../../services/doctorsService";
import axios from "axios";
import "bootstrap/dist/css/bootstrap.min.css";

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
    const [formData, setFormData] = useState<DoctorDto | undefined>(doctor);
    const [loading, setLoading] = useState(true);
    const [edit, setEdit] = useState(false);

    useEffect(() => {
        getDoctorMe()
            .then((data) => {
                setDoctor(data);
                setFormData(data);
            })
            .catch((err) => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev!, [name]: value }));
    };

    async function change() {
        if (edit) {
            try {
                await axios.put(
                    `https://localhost:7014/api/doctors/update`,
                    {
                        name: formData?.name,
                        surname: formData?.surname,
                        startHour: formData?.startHour,
                        endHour: formData?.endHour,
                        phoneNumber: formData?.phoneNumber,
                    },
                    {
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${localStorage.getItem("token")}`,
                        },
                    }
                );
                setDoctor(formData);
            } catch (err) {
                console.error("Error updating doctor data:", err);
            }
            setEdit(false);
        } else {
            setEdit(true);
        }
    }

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

                    <div className="row mb-3">
                        <div className="col-sm-3 fw-bold">Name</div>
                        <div className="col-sm-9">
                            {!edit && (
                                <div>
                                    {formData?.name} {formData?.surname}
                                </div>
                            )}
                            {edit && (
                                <div style={{ display: "flex", gap: "10px" }}>
                                    <input
                                        type="text"
                                        name="name"
                                        className="form-control"
                                        value={formData?.name || ""}
                                        onChange={handleChange}
                                        style={{ marginBottom: "10px" }}
                                    />
                                    <input
                                        type="text"
                                        name="surname"
                                        className="form-control"
                                        value={formData?.surname || ""}
                                        onChange={handleChange}
                                        style={{ marginBottom: "10px" }}
                                    />
                                </div>
                            )}
                        </div>
                    </div>

                    <div className="row mb-3">
                        <div className="col-sm-3 fw-bold">Specializations</div>
                        <div className="col-sm-9">
                            {formData?.specializations.join(", ")}
                        </div>
                    </div>

                    <div className="row mb-4">
                        <div className="col-sm-3 fw-bold">Working Hours</div>
                        <div className="col-sm-9">
                            {!edit && (
                                <div>
                                    {formData?.startHour.slice(0, 5)} - {formData?.endHour.slice(0, 5)}
                                </div>
                            )}
                            {edit && (
                                <div style={{ display: "flex", gap: "10px" }}>
                                    <input
                                        type="time"
                                        name="startHour"
                                        className="form-control"
                                        value={formData?.startHour.slice(0, 5) || ""}
                                        onChange={handleChange}
                                    />
                                    <input
                                        type="time"
                                        name="endHour"
                                        className="form-control"
                                        value={formData?.endHour.slice(0, 5) || ""}
                                        onChange={handleChange}
                                    />
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Contact Details */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-envelope me-2"></i> Contact Details
                    </h5>

                    <div className="row mb-3">
                        <div className="col-sm-3 fw-bold">Email</div>
                        <div className="col-sm-9">{formData?.email}</div>
                    </div>

                    <div className="row mb-4">
                        <div className="col-sm-3 fw-bold">Phone</div>
                        <div className="col-sm-9">
                            {!edit && <div>{formData?.phoneNumber}</div>}
                            {edit && (
                                <input
                                    type="tel"
                                    name="phoneNumber"
                                    className="form-control"
                                    value={formData?.phoneNumber || ""}
                                    onChange={handleChange}
                                />
                            )}
                        </div>
                    </div>

                    {/* Security */}
                    <h5 className="card-title border-bottom pb-2 mb-3">
                        <i className="bi bi-shield-lock me-2"></i> Security
                    </h5>

                    <div className="row mb-3">
                        <div className="col-sm-3 fw-bold">Password</div>
                        <div className="col-sm-9">
                            &bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;&bull;
                        </div>
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
                    <button className="btn btn-outline-primary px-4 py-2 rounded-pill shadow-sm" onClick={change}>
                        <i className="bi bi-pencil-square me-2"></i>{" "}
                        {edit ? "Save Changes" : "Edit Data"}
                    </button>
                </div>
            </div>
        </div>
    );
}

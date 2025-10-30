import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getDoctorMe } from "../../../services/doctorsService";
import type { DoctorPrifileDto } from "../../../interfaces/doctors.types";
import axios from "axios";
import "bootstrap/dist/css/bootstrap.min.css";

export default function DoctorProfile() {
    const [doctor, setDoctor] = useState<DoctorPrifileDto>();
    const [formData, setFormData] = useState<DoctorPrifileDto | undefined>(doctor);
    const [loading, setLoading] = useState(true);
    const [edit, setEdit] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
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

        setFormData((prev) => ({ ...prev!, [name]: value}));
    };


    async function change() {
        setError("");
        setSuccess("");
        if (edit) {
            if (!formData?.name || "" || !/^[A-Za-z]+$/.test(formData!.name) ) {
                setError("Name must contain only letters and cannot be empty.");
                return;
            }
            if (!formData?.surname || "" || !/^[A-Za-z]+$/.test(formData!.surname)) {
                setError("Surname must contain only letters and cannot be empty.");
                return;
            }
            if (validatePhoneNumber(formData?.phoneNumber || "") !== "") {
                setError(validatePhoneNumber(formData?.phoneNumber || ""));
                return;
            }
            if (validateHours(formData?.startHour || "", formData?.endHour || "") !== "") {
                setError(validateHours(formData?.startHour || "", formData?.endHour || ""));
                return;
            }
            let colonCount = (formData!.startHour.match(/:/g) || []).length;

            if (colonCount === 1) {
                formData!.startHour+=":00";
            }

            colonCount = (formData!.endHour.match(/:/g) || []).length;

            if (colonCount === 1) {
                formData!.endHour += ":00";
            }

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
                setSuccess("Profile updated successfully.");
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
                {error && <div className="reset-error">{error}</div>}
                {success && <div className="reset-success">{success}</div>}
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
function validatePhoneNumber(phoneNumber: string): string {
    if (!phoneNumber) {
        return "Phone number is required.";
    }

    if (!/^\d+$/.test(phoneNumber)) {
       return "Phone number must contain only digits.";
    }

    if (phoneNumber.length != 9) {
        return "Phone number must be 9 digits long.";
    }

    if (phoneNumber.startsWith("0")) {
        return "Phone number cannot start with 0.";
    }
    return "";
}
function validateHours(startHour: string, endHour: string): string {
    if (startHour == endHour) {
        return "Stat hour and end hour cannot be the same.";
    }
    if (!startHour || !endHour) {
        return "Start hour and end hour are required.";
    }
    if (startHour > endHour) {
        return "Start hour must be before end hour.";
    }
    if (startHour < "08:00" || startHour > "20:00" || endHour < "08:00" || endHour > "20:00") {
        return "Working hours must be between 08:00 and 20:00.";
    }

    return "";
}

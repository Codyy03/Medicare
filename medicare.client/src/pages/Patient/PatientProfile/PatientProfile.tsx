import { Link } from "react-router-dom"
import { useEffect, useState } from "react";
import { getPatientMe } from "../../../services/patientsService"
import type { Patient } from "../../../interfaces/patients.types";
import axios from "axios";
export default function PatientProfile() {
    const [patient, setPatient] = useState<Patient>();
    const [loading, setLoading] = useState(true);
    const [edit, setEdit] = useState(false);
    const [formData, setFormData] = useState<Patient | undefined>(patient);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    async function change() {
        setError("");
        setSuccess("");
        if (edit) {
            const name = formData?.name;
            const surname = formData?.surname;
            const pesel = formData?.pesel;
            const phonenumber = formData?.phoneNumber;
            const birthday = formData?.birthday;

            if (!formData?.name || "" || !/^[A-Za-z]+$/.test(formData!.name)) {
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
            if (validatePesel(formData?.pesel || "") !== "") {
                setError(validatePesel(formData?.pesel || ""));
                return;
            }
            if (isValidDate(formData?.birthday || "") !== "") {
                setError(isValidDate(formData?.birthday || ""));
                return;
            }
            try {
                await axios.put(
                    `https://localhost:7014/api/patients/update`,
                    { name, surname, pesel, phonenumber, birthday },
                    {
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${localStorage.getItem("token")}`,
                        },
                    }
                );
                setSuccess("Profile updated successfully.");

            } catch (err) {
                if (axios.isAxiosError(err)) {
                    const message =
                       err.response?.data?.message ||
                     "An unexpected error occurred while changing your password.";
                }
            }
            setEdit(false);
        }
        else {
            //kod jesli zaczynamy edytowaæ
            setEdit(true);
        };
    }
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev!, [name]: value }));
    };
    useEffect(() => {
        getPatientMe().
            then(data => {
                setPatient(data);
                setFormData(data);
            }).
            catch(err => console.error(err)).
            finally(() => setLoading(false));
    }, []);
    const formatDateForInput = (dateString: string) => {
        const date = new Date(dateString);
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, "0");
        const day = date.getDate().toString().padStart(2, "0");
        return `${year}-${month}-${day}`;
    };

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
                        <dd className="col-sm-9">
                            {!edit && (
                                <div>{formData?.name} {formData?.surname}</div>
                            )}
                            {edit && (
                                <div style={{ display: "flex", gap: "10px" }}>
                                    <input
                                        type="text"
                                        name="name"
                                        className="form-control"
                                        value={formData?.name}
                                        onChange={handleChange}
                                        style={{ width: "100%", marginBottom: "10px" }}
                                    />
                                    <input
                                        type="text"
                                        name="surname"
                                        className="form-control"
                                        value={formData?.surname}
                                        onChange={handleChange}
                                        style={{ width: "100%", marginBottom: "10px" }}
                                    />
                                </div>
                            )}
                        </dd>
                        <dt className="col-sm-3">PESEL</dt>
                        <dd className="col-sm-9">
                            {!edit && (
                                <div>{formData?.pesel}</div>
                            )}
                            {edit && (
                                <input
                                    type="text"
                                    name="pesel"
                                    className="form-control"
                                    value={formData?.pesel}
                                    onChange={handleChange}
                                    style={{ marginBottom: "10px" }}
                                />
                            )}
                        </dd>

                        <dt className="col-sm-3">Birthday</dt>
                        <dd className="col-sm-9">
                            {!edit && (
                                <div>{formData ? new Date(formData.birthday).toLocaleDateString() : ""}</div>
                            )}
                            {edit && (
                                <input
                                    type="date"
                                    name="birthday"
                                    className="form-control"
                                    value={formData?.birthday ? formatDateForInput(formData.birthday) : ""}
                                    onChange={handleChange}
                                />
                            )}
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
                        <dd className="col-sm-9">
                            {!edit && (
                                <div>{formData?.phoneNumber}</div>
                            )}
                            {edit && (
                                <input
                                    type="tel"
                                    name="phoneNumber"
                                    className="form-control"
                                    value={formData?.phoneNumber}
                                    onChange={handleChange}
                                />
                            )}
                        </dd>
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
                            <Link to="/resetPasswordPatient" className="btn btn-outline-primary btn-sm">
                                <i className="bi bi-arrow-repeat me-1"></i> Reset Password
                            </Link>
                        </div>
                    </div>
                </div>
                {error && <div className="reset-error">{error}</div>}
                {success && <div className="reset-success">{success}</div>}
                <div className="card-footer d-flex justify-content-center bg-light">
                    <button className="btn btn-outline-primary px-4 py-2 rounded-pill shadow-sm" onClick={change}>
                        <i className="bi bi-pencil-square me-2"></i>
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


function validatePesel(pesel: string): string {

    if (!/^\d{11}$/.test(pesel)) {
        return "PESEL must be 11 digits.";
    }
    if (!pesel) {
        return "PESEL is required.";
    }
    if (!/^\d+$/.test(pesel)) {
        return "Phone number must contain only digits.";
    }
    return "";
}

function isValidDate(birthday: string): string {
    const today = new Date();
    const birthdate = new Date(birthday);
    today.setHours(0, 0, 0, 0);
    birthdate.setHours(0, 0, 0, 0);
    if (isNaN(birthdate.getTime())) {
        return "Invalid date format.";
    }
    if (birthdate > today) {
        return "Date cannot be in the future.";
    }
    if (birthdate < new Date("1900-01-01")) {
        return "Date is too far in the past.";
    }
    return "";
}
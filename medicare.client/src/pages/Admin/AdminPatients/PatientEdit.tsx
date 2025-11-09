import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { FaUser, FaEnvelope, FaPhone, FaCalendarAlt, FaIdCard, FaSave, FaArrowLeft, FaUserCheck } from "react-icons/fa";
import type { Patient } from "../../../interfaces/patients.types";
import { getAdminPatientById, updateAdminPatient } from "../../../services/patientsService";

export default function PatientEdit() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [patient, setPatient] = useState<Patient | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getAdminPatientById(Number(id))
            .then(setPatient)
            .catch((err) => console.error(err))
            .finally(() => setLoading(false));
    }, [id]);


    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        if (!patient) return;
        setPatient({ ...patient, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!patient) return;

        const dto = {
            id: patient.id,
            name: patient.name,
            surname: patient.surname,
            pesel: patient.pesel,
            birthday: patient.birthday,
            email: patient.email,
            phoneNumber: patient.phoneNumber,
            status: patient.status
        };

        try {
            await updateAdminPatient(patient.id, dto);
            alert("Patient updated successfully");
            navigate("/admin/patients");
        } catch (err: any) {
            alert("Error updating patient: " + err.message);
        }

    };

    if (loading) return <p>Loading...</p>;
    if (!patient) return <p>Patient not found</p>;

    return (
        <div className="container py-4">
            <div className="card shadow-lg p-4">
                <h2 className="mb-4 text-center text-primary">Edit Patient</h2>
                <form onSubmit={handleSubmit} className="patient-edit-form">
                    <div className="row g-3">
                        {/* Name */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUser className="me-2" />Name</label>
                            <input type="text" name="name" value={patient.name} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Surname */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUser className="me-2" />Surname</label>
                            <input type="text" name="surname" value={patient.surname} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* PESEL */}
                        <div className="col-md-6">
                            <label className="form-label"><FaIdCard className="me-2" />PESEL</label>
                            <input type="text" name="pesel" value={patient.pesel} onChange={handleChange} className="form-control" required maxLength={11} />
                        </div>

                        {/* Birthday */}
                        <div className="col-md-6">
                            <label className="form-label"><FaCalendarAlt className="me-2" />Birthday</label>
                            <input type="date" name="birthday" value={patient.birthday.slice(0, 10)} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Email */}
                        <div className="col-md-6">
                            <label className="form-label"><FaEnvelope className="me-2" />Email</label>
                            <input type="email" name="email" value={patient.email} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Phone */}
                        <div className="col-md-6">
                            <label className="form-label"><FaPhone className="me-2" />Phone</label>
                            <input type="tel" name="phoneNumber" value={patient.phoneNumber} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Status */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUserCheck className="me-2" />Status</label>
                            <select
                                name="status"
                                value={patient.status}
                                onChange={(e) => setPatient({ ...patient, status: parseInt(e.target.value) })}
                                className="form-select"
                            >
                                <option value={0}>Active</option>
                                <option value={1}>Inactive</option>
                            </select>

                        </div>
                    </div>

                    <div className="d-flex justify-content-between mt-4">
                        <button type="button" className="btn btn-outline-secondary" onClick={() => navigate("/admin/patients")}>
                            <FaArrowLeft className="me-2" /> Back
                        </button>
                        <button type="submit" className="btn btn-primary">
                            <FaSave className="me-2" /> Save Changes
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

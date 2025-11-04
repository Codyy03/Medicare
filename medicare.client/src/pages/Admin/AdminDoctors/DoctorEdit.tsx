import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import type { DoctorAdminDto } from "../../../interfaces/doctors.types";
import { getSpecializationNames } from "../../../services/specializationsService";
import type { SpecializationsNamesID } from "../../../interfaces/visits.types";
import Select from "react-select";
import { FaUserMd, FaEnvelope, FaPhone, FaHospital, FaClock, FaAlignLeft, FaSave, FaArrowLeft, FaUserShield } from "react-icons/fa";
import "./DoctorEdit.css";

interface OptionType {
    value: number | string;
    label: string;
}

export default function DoctorEdit() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [doctor, setDoctor] = useState<DoctorAdminDto | null>(null);
    const [specializations, setSpecializations] = useState<SpecializationsNamesID[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch(`https://localhost:7014/api/doctors/${id}`)
            .then((res) => {
                if (!res.ok) throw new Error("Doctor not found");
                return res.json();
            })
            .then((data) => setDoctor(data))
            .catch((err) => console.error(err))
            .finally(() => setLoading(false));
    }, [id]);

    useEffect(() => {
        Promise.all([
            fetch(`https://localhost:7014/api/doctors/${id}`).then(res => res.json()),
            getSpecializationNames()
        ])
            .then(([doctorData, specs]) => {
                const specializationIds = specs
                    .filter((spec: SpecializationsNamesID) =>
                        doctorData.specializations.includes(spec.specializationName)
                    )
                    .map((spec: SpecializationsNamesID) => spec.id);


                setSpecializations(specs);
                setDoctor({ ...doctorData, specializations: specializationIds });
            })
            .catch(console.error)
            .finally(() => setLoading(false));
    }, [id]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        if (!doctor) return;
        setDoctor({ ...doctor, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!doctor) return;

        let startHour = doctor.startHour;
        let endHour = doctor.endHour;

        if ((startHour.match(/:/g) || []).length === 1) {
            startHour += ":00";
        }
        if ((endHour.match(/:/g) || []).length === 1) {
            endHour += ":00";
        }

        const dto = {
            name: doctor.name,
            surname: doctor.surname,
            email: doctor.email,
            phoneNumber: doctor.phoneNumber,
            startHour: startHour,
            endHour: endHour,
            facility: doctor.facility,
            doctorDescription: doctor.doctorDescription,
            role: doctor.role,
            specializationsIds: doctor.specializations
        };

        const token = localStorage.getItem("token");

        const response = await fetch(`https://localhost:7014/api/AdminDoctors/${doctor.id}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(dto),
        });

        if (response.ok) {
            alert("Doctor updated successfully");
            navigate("/admin/doctors");
        } else {
            const errorMsg = await response.text();
            alert("Error updating doctor: " + errorMsg);
        }
    };


    if (loading) return <p>Loading...</p>;
    if (!doctor) return <p>Doctor not found</p>;

    return (
        <div className="container py-4">
            <div className="card shadow-lg p-4">
                <h2 className="mb-4 text-center text-primary">Edit Doctor</h2>
                <form onSubmit={handleSubmit} className="doctor-edit-form">
                    <div className="row g-3">
                        {/* Name */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUserMd className="me-2" />Name</label>
                            <input type="text" name="name" value={doctor.name} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Surname */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUserMd className="me-2" />Surname</label>
                            <input type="text" name="surname" value={doctor.surname} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Email */}
                        <div className="col-md-6">
                            <label className="form-label"><FaEnvelope className="me-2" />Email</label>
                            <input type="email" name="email" value={doctor.email} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Phone */}
                        <div className="col-md-6">
                            <label className="form-label"><FaPhone className="me-2" />Phone</label>
                            <input type="tel" name="phoneNumber" value={doctor.phoneNumber} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Facility */}
                        <div className="col-md-12">
                            <label className="form-label"><FaHospital className="me-2" />Facility</label>
                            <input type="text" name="facility" value={doctor.facility} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Start Hour */}
                        <div className="col-md-6">
                            <label className="form-label"><FaClock className="me-2" />Start Hour</label>
                            <input type="time" name="startHour" value={doctor.startHour} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* End Hour */}
                        <div className="col-md-6">
                            <label className="form-label"><FaClock className="me-2" />End Hour</label>
                            <input type="time" name="endHour" value={doctor.endHour} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Description */}
                        <div className="col-md-12">
                            <label className="form-label"><FaAlignLeft className="me-2" />Description</label>
                            <textarea name="doctorDescription" value={doctor.doctorDescription} onChange={handleChange} className="form-control" rows={3} required />
                        </div>

                        {/* Role */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUserShield className="me-2" />Role</label>
                            <select
                                name="role"
                                value={doctor.role}
                                onChange={(e) => setDoctor({ ...doctor, role: Number(e.target.value) })}
                                className="form-select"
                            >
                                <option value={2}>Doctor</option>
                                <option value={3}>Admin</option>
                            </select>
                        </div>

                        {/* Specializations */}
                        <div className="col-md-12">
                            <label className="form-label"><FaUserMd className="me-2" />Specializations</label>
                            <Select<OptionType, true>
                                isMulti
                                options={specializations.map(spec => ({
                                    value: spec.id,
                                    label: spec.specializationName
                                }))}
                                value={specializations
                                    .filter(spec => doctor.specializations.includes(spec.id))
                                    .map(spec => ({ value: spec.id, label: spec.specializationName }))}
                                onChange={(selected) => {
                                    const ids = (selected ?? []).map(s => Number(s.value));
                                    setDoctor({ ...doctor, specializations: ids });
                                }}
                            />
                        </div>
                    </div>

                    <div className="d-flex justify-content-between mt-4">
                        <button type="button" className="btn btn-outline-secondary" onClick={() => navigate("/admin/doctors")}>
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

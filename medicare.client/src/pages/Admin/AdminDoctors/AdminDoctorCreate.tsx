import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Select from "react-select";
import { getSpecializationNames } from "../../../services/specializationsService";
import type { SpecializationsNamesID } from "../../../interfaces/visits.types";
import { FaUserMd, FaEnvelope, FaPhone, FaHospital, FaClock, FaAlignLeft, FaSave, FaUserShield, FaKey } from "react-icons/fa";

interface OptionType {
    value: number;
    label: string;
}

export default function AdminDoctorCreate() {
    const navigate = useNavigate();

    const [form, setForm] = useState({
        name: "",
        surname: "",
        email: "",
        phoneNumber: "",
        password: "",
        confirmPassword: "",
        startHour: "",
        endHour: "",
        facility: "",
        doctorDescription: "",
        role: 2, 
        specializationsID: [] as string[],
    });

    const [specializations, setSpecializations] = useState<SpecializationsNamesID[]>([]);
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getSpecializationNames()
            .then(data => setSpecializations(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateForm()) return;

        // dopisz sekundy do godzin
        let startHour = form.startHour;
        let endHour = form.endHour;
        if ((startHour.match(/:/g) || []).length === 1) startHour += ":00";
        if ((endHour.match(/:/g) || []).length === 1) endHour += ":00";

        const { confirmPassword, ...dto } = form;

        const dtoToSend = {
            ...dto,
            startHour,
            endHour,
            specializationIds: form.specializationsID.map(id => Number(id)),
        };

        const token = localStorage.getItem("token");

        const response = await fetch("https://localhost:7014/api/AdminDoctors/adminDoctorRegister", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(dtoToSend),
        });

        if (response.ok) {
            alert("Doctor created successfully");
            navigate("/admin/doctors");
        } else {
            const errorMsg = await response.text();
            alert("Error: " + errorMsg);
        }
    };

    const validateForm = () => {
        const newErrors: { [key: string]: string } = {};

        if (!form.name.trim()) newErrors.name = "Name is required";
        if (!form.surname.trim()) newErrors.surname = "Surname is required";

        if (!form.email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)) {
            newErrors.email = "Invalid email format";
        }

        if (!/^\d{9}$/.test(form.phoneNumber)) {
            newErrors.phoneNumber = "Phone number must be 9 digits long";
        }

        if (form.password.length < 8) {
            newErrors.password = "Password must be at least 8 characters long.";
        }
        if (form.password !== form.confirmPassword) {
            newErrors.confirmPassword = "Passwords do not match";
        }

        if (!form.startHour || !form.endHour) {
            newErrors.hours = "Start and end hours are required";
        }

        if (!form.facility.trim()) newErrors.facility = "Facility is required";
        if (!form.doctorDescription.trim()) newErrors.doctorDescription = "Description is required";

        if (form.specializationsID.length === 0) {
            newErrors.specializations = "At least one specialization must be selected";
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div className="register-container">
            <div className="register-box">
                <h2>Create Doctor (Admin)</h2>
                <form onSubmit={handleSubmit} className="register-form">
                    <input type="text" name="name" placeholder="Name" value={form.name} onChange={handleChange} required />
                    {errors.name && <span className="error-text">{errors.name}</span>}

                    <input type="text" name="surname" placeholder="Surname" value={form.surname} onChange={handleChange} required />
                    {errors.surname && <span className="error-text">{errors.surname}</span>}

                    <input type="email" name="email" placeholder="Email" value={form.email} onChange={handleChange} required />
                    {errors.email && <span className="error-text">{errors.email}</span>}

                    <input type="tel" name="phoneNumber" placeholder="Phone number" value={form.phoneNumber} onChange={handleChange} required />
                    {errors.phoneNumber && <span className="error-text">{errors.phoneNumber}</span>}

                    <input type="password" name="password" placeholder="Password" value={form.password} onChange={handleChange} required />
                    {errors.password && <span className="error-text">{errors.password}</span>}

                    <input type="password" name="confirmPassword" placeholder="Confirm Password" value={form.confirmPassword} onChange={handleChange} required />
                    {errors.confirmPassword && <span className="error-text">{errors.confirmPassword}</span>}

                    <input type="time" name="startHour" value={form.startHour} onChange={handleChange} required />
                    <input type="time" name="endHour" value={form.endHour} onChange={handleChange} required />
                    {errors.hours && <span className="error-text">{errors.hours}</span>}

                    <input type="text" name="facility" placeholder="Facility" value={form.facility} onChange={handleChange} required />
                    {errors.facility && <span className="error-text">{errors.facility}</span>}

                    <textarea name="doctorDescription" placeholder="Description" value={form.doctorDescription} onChange={handleChange} required />
                    {errors.doctorDescription && <span className="error-text">{errors.doctorDescription}</span>}

                    <Select<OptionType, true>
                        isMulti
                        className="register-select"
                        classNamePrefix="register-select"
                        name="specializationsID"
                        options={specializations.map(spec => ({
                            value: spec.id,
                            label: spec.specializationName
                        }))}
                        value={specializations
                            .filter(spec => form.specializationsID.includes(spec.id.toString()))
                            .map(spec => ({ value: spec.id, label: spec.specializationName }))}
                        onChange={(selected) => {
                            const ids = (selected ?? []).map(s => s.value.toString());
                            setForm({ ...form, specializationsID: ids });
                        }}
                    />
                    {errors.specializations && <span className="error-text">{errors.specializations}</span>}

                    <select
                        name="role"
                        value={form.role}
                        onChange={(e) => setForm({ ...form, role: Number(e.target.value) })}
                        className="form-select mt-3"
                    >
                        <option value={2}>Doctor</option>
                        <option value={3}>Admin</option>
                    </select>


                    <button type="submit" className="btn btn-primary w-100 mt-3">
                        <FaSave className="me-2" /> Create Doctor
                    </button>
                </form>
            </div>
        </div>
    );
}

import React, { useState } from "react";
import "./DoctorRegister.css";

export default function DoctorRegister() {
    const [form, setForm] = useState({
        name: "",
        surname: "",
        specialization: "",
        pwz: "",
        email: "",
        phoneNumber: "",
        password: "",
        confirmPassword: ""
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        alert("Only desing");
    };

    return (
        <div className="register-container">
            <div className="register-box">
                <h2>Doctor registration</h2>
                <form onSubmit={handleSubmit} className="register-form">
                    <input type="text" name="name" placeholder="name" value={form.name} onChange={handleChange} required />
                    <input type="text" name="surname" placeholder="last name" value={form.surname} onChange={handleChange} required />

                    <select name="specialization" value={form.specialization} onChange={handleChange} required>
                        <option value="">-- Choose your specialization --</option>
                        <option value="kardiolog">Cardiologist</option>
                        <option value="pediatra">Pediatrician</option>
                        <option value="ortopeda">Orthopaedist</option>
                        <option value="dermatolog">Dermatologist</option>
                        <option value="neurolog">Neurologist</option>
                    </select>

                    <input type="email" name="email" placeholder="email" value={form.email} onChange={handleChange} required />
                    <input type="tel" name="phoneNumber" placeholder="phone number" value={form.phoneNumber} onChange={handleChange} required />
                    <input type="password" name="password" placeholder="password" value={form.password} onChange={handleChange} required />
                    <input type="password" name="confirmPassword" placeholder="repeat password" value={form.confirmPassword} onChange={handleChange} required />

                    <button type="submit" className="btn btn-primary w-100 mt-3">Register</button>
                </form>
            </div>
        </div>
    );
}

import React, { useState, useEffect } from "react";
import { getSpecializationNames } from "../../services/specializationsService";
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

    const [specializations, setSpecializations] = useState<string[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getSpecializationNames().
            then(data => setSpecializations(data)).
            catch(err => console.error(err)).
            finally(() => setLoading(false));
    }, [])

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        alert("Only desing");
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div className="register-container">
            <div className="register-box">
                <h2>Doctor registration</h2>
                <form onSubmit={handleSubmit} className="register-form">
                    <input type="text" name="name" placeholder="name" value={form.name} onChange={handleChange} required />
                    <input type="text" name="surname" placeholder="last name" value={form.surname} onChange={handleChange} required />

                    <select name="specialization" value={form.specialization} onChange={handleChange} required>
                        <option value="">-- Choose your specialization --</option>
                        {specializations.map((spec, index) => (
                            <option key={index} value={spec}>{spec}</option>
                        ))}
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

import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createAdminPatient } from "../../../services/patientsService";

export default function AdminPatientsCreate() {
    const [form, setForm] = useState({
        pesel: "",
        name: "",
        surname: "",
        birthday: "",
        email: "",
        phoneNumber: "",
        password: "",
        confirmPassword: ""
    });

    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;

        if (name === "pesel" && value.length > 11) return;

        setForm({ ...form, [name]: value });
    };

    const validateForm = () => {
        const newErrors: { [key: string]: string } = {};

        if (!/^\d{11}$/.test(form.pesel)) {
            newErrors.pesel = "PESEL must have 11 digits";
        }
        if (!form.name.trim()) {
            newErrors.name = "Name is required";
        }
        if (!form.surname.trim()) {
            newErrors.surname = "Last name is required";
        }
        if (!form.email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)) {
            newErrors.email = "Incorrect email";
        }
        if (!/^\d{9}$/.test(form.phoneNumber)) {
            newErrors.phoneNumber = "Phone number must be 9 digits long";
        }
        if (form.password.length < 8) {
            newErrors.password = "The password must be at least 8 characters long.";
        }
        if (form.password !== form.confirmPassword) {
            newErrors.confirmPassword = "The passwords are not identical";
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateForm()) return;
        const dto = {
            PESEL: form.pesel,
            Name: form.name,
            Surname: form.surname,
            Birthday: form.birthday,
            Email: form.email,
            PhoneNumber: form.phoneNumber,
            Password: form.password
        };


        try {
            await createAdminPatient(dto);
            navigate("/admin/patients");
        } catch (err: any) {
            const msg = await err.response?.text?.();
            alert("Error: " + msg);
        }
    };

    return (
        <div className="register-container">
            <div className="register-box">
                <h2>Create Patient (Admin)</h2>
                <form onSubmit={handleSubmit} className="register-form">
                    <input
                        type="number"
                        name="pesel"
                        placeholder="PESEL"
                        value={form.pesel}
                        onChange={handleChange}
                        className={errors.pesel ? "invalid" : ""}
                        required
                    />
                    {errors.pesel && <span className="error-text">{errors.pesel}</span>}

                    <input
                        type="text"
                        name="name"
                        placeholder="Name"
                        value={form.name}
                        onChange={handleChange}
                        className={errors.name ? "invalid" : ""}
                        required
                    />
                    {errors.name && <span className="error-text">{errors.name}</span>}

                    <input
                        type="text"
                        name="surname"
                        placeholder="Surname"
                        value={form.surname}
                        onChange={handleChange}
                        className={errors.surname ? "invalid" : ""}
                        required
                    />
                    {errors.surname && <span className="error-text">{errors.surname}</span>}

                    <input
                        type="date"
                        name="birthday"
                        value={form.birthday}
                        onChange={handleChange}
                        className={errors.birthday ? "invalid" : ""}
                        required
                    />
                    {errors.birthday && <span className="error-text">{errors.birthday}</span>}

                    <input
                        type="email"
                        name="email"
                        placeholder="Email"
                        value={form.email}
                        onChange={handleChange}
                        className={errors.email ? "invalid" : ""}
                        required
                    />
                    {errors.email && <span className="error-text">{errors.email}</span>}

                    <input
                        type="tel"
                        name="phoneNumber"
                        placeholder="Phone Number"
                        value={form.phoneNumber}
                        onChange={handleChange}
                        className={errors.phoneNumber ? "invalid" : ""}
                        required
                    />
                    {errors.phoneNumber && <span className="error-text">{errors.phoneNumber}</span>}

                    <input
                        type="password"
                        name="password"
                        placeholder="Password"
                        value={form.password}
                        onChange={handleChange}
                        className={errors.password ? "invalid" : ""}
                        required
                    />
                    {errors.password && <span className="error-text">{errors.password}</span>}

                    <input
                        type="password"
                        name="confirmPassword"
                        placeholder="Confirm Password"
                        value={form.confirmPassword}
                        onChange={handleChange}
                        className={errors.confirmPassword ? "invalid" : ""}
                        required
                    />
                    {errors.confirmPassword && <span className="error-text">{errors.confirmPassword}</span>}

                    <button type="submit" className="btn btn-primary w-100 mt-3">
                        Create new Patient
                    </button>
                </form>
            </div>
        </div>
    );
}

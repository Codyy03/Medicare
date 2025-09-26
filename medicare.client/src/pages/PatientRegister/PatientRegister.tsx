import React, { useState } from "react";
import "./PatientRegister.css";
export default function PatientRegister() {
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

        const { confirmPassword, ...dto } = form;

        const response = await fetch("https://localhost:7014/api/patients/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(form),
        });

        if (response.ok) {
            alert("success");
        } else {
            const errorMsg = await response.text();
            alert("Error" + errorMsg);
        }
    };

return (
    <div className="register-container">
        <div className="register-box">
            <h2>Patient registration</h2>
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
                    placeholder="name"
                    value={form.name}
                    onChange={handleChange}
                    className={errors.name ? "invalid" : ""}
                    required
                />
                {errors.name && <span className="error-text">{errors.name}</span>}

                <input
                    type="text"
                    name="surname"
                    placeholder="surname"
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
                    required
                />

                <input
                    type="email"
                    name="email"
                    placeholder="email"
                    value={form.email}
                    onChange={handleChange}
                    className={errors.email ? "invalid" : ""}
                    required
                />
                {errors.email && <span className="error-text">{errors.email}</span>}

                <input
                    type="tel"
                    name="phoneNumber"
                    placeholder="phone number"
                    value={form.phoneNumber}
                    onChange={handleChange}
                    className={errors.phoneNumber ? "invalid" : ""}
                    required
                />
                {errors.phoneNumber && <span className="error-text">{errors.phoneNumber}</span>}

                <input
                    type="password"
                    name="password"
                    placeholder="password"
                    value={form.password}
                    onChange={handleChange}
                    className={errors.password ? "invalid" : ""}
                    required
                />
                {errors.password && <span className="error-text">{errors.password}</span>}

                <input
                    type="password"
                    name="confirmPassword"
                    placeholder="confirm password"
                    value={form.confirmPassword}
                    onChange={handleChange}
                    className={errors.confirmPassword ? "invalid" : ""}
                    required
                />
                {errors.confirmPassword && <span className="error-text">{errors.confirmPassword}</span>}

                <button type="submit" className="btn btn-primary w-100 mt-3">Register</button>
            </form>
        </div>
    </div>
);
}
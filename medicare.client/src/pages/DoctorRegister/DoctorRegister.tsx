import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Select from "react-select";
import { getSpecializationNames } from "../../services/specializationsService";
import "./DoctorRegister.css";

export default function DoctorRegister() {
    const [form, setForm] = useState({
        name: "",
        surname: "",
        specializationsID: [] as string[], // trzymamy jako string[], konwersja przy wysy³ce
        pwz: "",
        email: "",
        phoneNumber: "",
        password: "",
        confirmPassword: ""
    });

    interface SpecializationsNamesID {
        id: number;
        specializationName: string;
    }

    interface OptionType {
        value: number;
        label: string;
    }

    const [specializations, setSpecializations] = useState<SpecializationsNamesID[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getSpecializationNames()
            .then(data => setSpecializations(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!validateForm()) return;

        const { confirmPassword, ...dto } = form;

        //converting ID to number[] and matching field name to DTO
        const dtoToSend = {
            ...dto,
            specializationIds: form.specializationsID.map(id => Number(id))
        };

        const response = await fetch("https://localhost:7014/api/doctors/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(dtoToSend),
        });

        if (response.ok) {
            alert("success");
            navigate("/");
        } else {
            const errorMsg = await response.text();
            alert("Error: " + errorMsg);
        }
    };

    const validateForm = () => {
        const newErrors: { [key: string]: string } = {};

        if (!form.name.trim()) {
            newErrors.name = "Name is required";
        }
        if (!form.surname.trim()) {
            newErrors.surname = "Last name is required";
        }

        if (!form.email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)) {
            newErrors.email = "Incorrect email format";
        } else if (!form.email.endsWith("@medicare.com")) {
            newErrors.email = "Email must use the medicare.com domain";
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
                <h2>Doctor registration</h2>
                <form onSubmit={handleSubmit} className="register-form">
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
                        placeholder="last name"
                        value={form.surname}
                        onChange={handleChange}
                        className={errors.surname ? "invalid" : ""}
                        required
                    />
                    {errors.surname && <span className="error-text">{errors.surname}</span>}

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
                        placeholder="repeat password"
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

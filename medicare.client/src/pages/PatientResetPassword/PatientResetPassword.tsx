import React, { useState } from "react";
import axios from "axios";
import "./PatientResetPassword.css";

const PatientResetPassword: React.FC = () => {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmNewPassword, setConfirmNewPassword] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setSuccess("");

        if (!oldPassword || !newPassword || !confirmNewPassword) {
            setError("All fields are required.");
            return;
        }

        if (newPassword.length < 8) {
            setError("Password must be at least 8 characters long.");
            return;
        } else if (!/[A-Z]/.test(newPassword)) {
            setError("Password must contain at least one uppercase letter.");
            return;
        } else if (!/[a-z]/.test(newPassword)) {
            setError("Password must contain at least one lowercase letter.");
            return;
        } else if (!/[0-9]/.test(newPassword)) {
            setError("Password must contain at least one digit.");
            return;
        } else if (!/[!@#$%^&*()_\-+=<>?/{}~|]/.test(newPassword)) {
            setError("Password must contain at least one special character.");
            return;
        }

        if (newPassword !== confirmNewPassword) {
            setError("New password and confirmation must match.");
            return;
        }

        try {
            await axios.put(
                "https://localhost:7014/api/patients/password-reset",
                { oldPassword, newPassword },
                {
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                }
            );

            setSuccess("Password has been changed successfully.");
            setOldPassword("");
            setNewPassword("");
            setConfirmNewPassword("");
        } catch (err) {
            if (axios.isAxiosError(err)) {
                const message =
                    err.response?.data?.message ||
                    "An unexpected error occurred while changing your password.";
                setError(message);
            } else {
                setError("Something went wrong. Please try again.");
            }
        }
    };

    return (
        <div className="reset-container">
            <h2 className="reset-title">Change Password</h2>
            <form onSubmit={handleSubmit}>
                <div className="reset-group">
                    <label className="reset-label">Old Password</label>
                    <input
                        type="password"
                        value={oldPassword}
                        onChange={(e) => setOldPassword(e.target.value)}
                        required
                        className="reset-input"
                        placeholder="Enter your old password"
                    />
                </div>
                <div className="reset-group">
                    <label className="reset-label">New Password</label>
                    <input
                        type="password"
                        value={newPassword}
                        onChange={(e) => setNewPassword(e.target.value)}
                        required
                        className="reset-input"
                        placeholder="Enter your new password"
                    />
                </div>
                <div className="reset-group">
                    <label className="reset-label">Confirm New Password</label>
                    <input
                        type="password"
                        value={confirmNewPassword}
                        onChange={(e) => setConfirmNewPassword(e.target.value)}
                        required
                        className="reset-input"
                        placeholder="Confirm your new password"
                    />
                </div>

                {error && <div className="reset-error">{error}</div>}
                {success && <div className="reset-success">{success}</div>}

                <button type="submit" className="reset-button">
                    Change Password
                </button>
            </form>
        </div>
    );
};

export default PatientResetPassword;

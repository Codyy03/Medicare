import React, { useState } from "react";
import axios from "axios";
import "./DoctorResetPassword.css";

const DoctorResetPassword: React.FC = () => {
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
        if (newPassword !== confirmNewPassword) {
            setError("New password and confirmation must match.");
            return;
        }

        try {
            await axios.put(
                "https://localhost:7014/api/doctors/password-reset",
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

export default DoctorResetPassword;

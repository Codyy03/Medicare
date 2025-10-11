import React, { useState } from "react";
import axios from "axios";

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

        await axios.put("/api/doctors/password-reset", newPassword, {
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${localStorage.getItem("token")}`
            }
        });
        setSuccess("Password has been changed successfully.");
        setOldPassword("");
        setNewPassword("");
        setConfirmNewPassword("");

    };

    return (
        <div className="container" style={{ maxWidth: 400, margin: "40px auto", background: "#fff", borderRadius: 8, boxShadow: "0 2px 8px rgba(0,0,0,0.08)", padding: 32 }}>
            <h2 style={{ textAlign: "center", marginBottom: 24, color: "#1976d2" }}>Change Password</h2>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: 18 }}>
                    <label style={{ display: "block", marginBottom: 6, fontWeight: 500 }}>Old Password</label>
                    <input
                        type="password"
                        value={oldPassword}
                        onChange={e => setOldPassword(e.target.value)}
                        required
                        style={{
                            width: "100%",
                            padding: "10px",
                            borderRadius: 4,
                            border: "1px solid #ccc",
                            fontSize: 16
                        }}
                        placeholder="Enter your old password"
                    />
                </div>
                <div style={{ marginBottom: 18 }}>
                    <label style={{ display: "block", marginBottom: 6, fontWeight: 500 }}>New Password</label>
                    <input
                        type="password"
                        value={newPassword}
                        onChange={e => setNewPassword(e.target.value)}
                        required
                        style={{
                            width: "100%",
                            padding: "10px",
                            borderRadius: 4,
                            border: "1px solid #ccc",
                            fontSize: 16
                        }}
                        placeholder="Enter your new password"
                    />
                </div>
                <div style={{ marginBottom: 18 }}>
                    <label style={{ display: "block", marginBottom: 6, fontWeight: 500 }}>Confirm New Password</label>
                    <input
                        type="password"
                        value={confirmNewPassword}
                        onChange={e => setConfirmNewPassword(e.target.value)}
                        required
                        style={{
                            width: "100%",
                            padding: "10px",
                            borderRadius: 4,
                            border: "1px solid #ccc",
                            fontSize: 16
                        }}
                        placeholder="Confirm your new password"
                    />
                </div>
                {error && <div style={{ color: "#d32f2f", marginBottom: 12, textAlign: "center" }}>{error}</div>}
                {success && <div style={{ color: "#388e3c", marginBottom: 12, textAlign: "center" }}>{success}</div>}
                <button
                    type="submit"
                    style={{
                        width: "100%",
                        padding: "12px",
                        background: "#1976d2",
                        color: "#fff",
                        border: "none",
                        borderRadius: 4,
                        fontSize: 16,
                        fontWeight: 600,
                        cursor: "pointer",
                        boxShadow: "0 1px 4px rgba(25, 118, 210, 0.12)"
                    }}
                >
                    Change Password
                </button>
            </form>
        </div>
    );
};

export default DoctorResetPassword;
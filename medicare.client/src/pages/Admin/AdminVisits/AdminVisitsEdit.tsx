import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { FaAlignLeft, FaSave, FaArrowLeft, FaCalendar } from "react-icons/fa";
import { getVisitByID, updateVisit } from "../../../services/visitsService";
import type { VisitsUpdateDto } from "../../../interfaces/visits.types";

export default function AdminVisitsEdit() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [visit, setVisit] = useState<VisitsUpdateDto>({
        id: 0,
        visitDate: new Date().toISOString().split("T")[0],
        visitTime: "08:00",
        status: "Scheduled",
        reason: "Checkup",
        additionalNotes: "",
        prescriptionText: "",
        visitNotes: ""
    });

    const [loading, setLoading] = useState(true);
    const [errors, setErrors] = useState<{ [key: string]: string }>({});

    useEffect(() => {
        if (!id) { setLoading(false); return; }
        (async () => {
            try {
                const data = await getVisitByID(Number(id));
                setVisit({
                    ...data,
                    visitDate: data.visitDate.split("T")[0]
                });
            } catch (err) {
                console.error(err);
                alert("Failed to load visit");
                navigate("/admin/adminVisits");
            } finally {
                setLoading(false);
            }
        })();
    }, [id]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) =>
        setVisit(prev => ({ ...prev, [e.target.name]: e.target.value }));

    function validate(): boolean {
        const newErrors: { [key: string]: string } = {};

        if (!visit.visitDate) {
            newErrors.visitDate = "Visit date is required.";
        } else if (new Date(visit.visitDate) < new Date(new Date().toISOString().split("T")[0])) {
            newErrors.visitDate = "Visit date cannot be in the past.";
        }

        if (!visit.visitTime) {
            newErrors.visitTime = "Visit time is required.";
        }

        const allowedStatuses = ["Scheduled", "Completed", "Cancelled"];
        if (!allowedStatuses.includes(visit.status)) {
            newErrors.status = "Invalid status selected.";
        }

        const allowedReasons = ["Consultation", "FollowUp", "Prescription", "Checkup"];
        if (!allowedReasons.includes(visit.reason)) {
            newErrors.reason = "Invalid reason selected.";
        }

        // Walidacja d³ugoœci pól tekstowych
        if (visit.additionalNotes && visit.additionalNotes.length > 500) {
            newErrors.additionalNotes = "Additional notes cannot exceed 500 characters.";
        }
        if (visit.prescriptionText && visit.prescriptionText.length > 500) {
            newErrors.prescriptionText = "Prescription text cannot exceed 500 characters.";
        }
        if (visit.visitNotes && visit.visitNotes.length > 500) {
            newErrors.visitNotes = "Visit notes cannot exceed 500 characters.";
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validate()) return;

        try {
            await updateVisit(visit.id, {
                ...visit,
                visitDate: new Date(visit.visitDate).toISOString()
            });
            alert("Visit updated");
            navigate("/admin/adminVisits");
        } catch (err: any) {
            console.error(err);
            alert("Error saving visit: " + (err.message ?? ""));
        }
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-4">
            <div className="card shadow-lg p-4">
                <h2 className="mb-4 text-center text-primary">Edit Visit</h2>
                <form onSubmit={handleSubmit}>
                    <div className="row g-3">
                        <div className="col-md-6">
                            <label className="form-label"><FaCalendar className="me-2" />Date</label>
                            <input
                                type="date"
                                name="visitDate"
                                value={visit.visitDate}
                                onChange={handleChange}
                                className={`form-control ${errors.visitDate ? "is-invalid" : ""}`}
                                required
                            />
                            {errors.visitDate && <div className="invalid-feedback">{errors.visitDate}</div>}
                        </div>
                        <div className="col-md-6">
                            <label className="form-label"><FaCalendar className="me-2" />Time</label>
                            <input
                                type="time"
                                name="visitTime"
                                value={visit.visitTime}
                                onChange={handleChange}
                                className={`form-control ${errors.visitTime ? "is-invalid" : ""}`}
                                required
                            />
                            {errors.visitTime && <div className="invalid-feedback">{errors.visitTime}</div>}
                        </div>

                        <div className="col-md-6">
                            <label className="form-label">Status</label>
                            <select
                                name="status"
                                value={visit.status}
                                onChange={handleChange}
                                className={`form-select ${errors.status ? "is-invalid" : ""}`}
                                required
                            >
                                <option value="Scheduled">Scheduled</option>
                                <option value="Completed">Completed</option>
                                <option value="Cancelled">Cancelled</option>
                            </select>
                            {errors.status && <div className="invalid-feedback">{errors.status}</div>}
                        </div>

                        <div className="col-md-6">
                            <label className="form-label">Reason</label>
                            <select
                                name="reason"
                                value={visit.reason}
                                onChange={handleChange}
                                className={`form-select ${errors.reason ? "is-invalid" : ""}`}
                                required
                            >
                                <option value="Consultation">Consultation</option>
                                <option value="FollowUp">Follow-up</option>
                                <option value="Prescription">Prescription</option>
                                <option value="Checkup">Checkup</option>
                            </select>
                            {errors.reason && <div className="invalid-feedback">{errors.reason}</div>}
                        </div>

                        <div className="col-md-12">
                            <label className="form-label"><FaAlignLeft className="me-2" />Additional Notes</label>
                            <textarea
                                name="additionalNotes"
                                value={visit.additionalNotes ?? ""}
                                onChange={handleChange}
                                className={`form-control ${errors.additionalNotes ? "is-invalid" : ""}`}
                                rows={3}
                            />
                            {errors.additionalNotes && <div className="invalid-feedback">{errors.additionalNotes}</div>}
                        </div>

                        <div className="col-md-12">
                            <label className="form-label">Prescription Text</label>
                            <textarea
                                name="prescriptionText"
                                value={visit.prescriptionText ?? ""}
                                onChange={handleChange}
                                className={`form-control ${errors.prescriptionText ? "is-invalid" : ""}`}
                                rows={2}
                            />
                            {errors.prescriptionText && <div className="invalid-feedback">{errors.prescriptionText}</div>}
                        </div>

                        <div className="col-md-12">
                            <label className="form-label">Visit Notes</label>
                            <textarea
                                name="visitNotes"
                                value={visit.visitNotes ?? ""}
                                onChange={handleChange}
                                className={`form-control ${errors.visitNotes ? "is-invalid" : ""}`}
                                rows={2}
                            />
                            {errors.visitNotes && <div className="invalid-feedback">{errors.visitNotes}</div>}
                        </div>
                    </div>

                    <div className="d-flex justify-content-between mt-4">
                        <button
                            type="button"
                            className="btn btn-outline-secondary"
                            onClick={() => navigate("/admin/adminVisits")}
                        >
                            <FaArrowLeft className="me-2" /> Back
                        </button>
                        <button type="submit" className="btn btn-primary">
                            <FaSave className="me-2" /> Save
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

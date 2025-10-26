import { useEffect, useState } from "react";
import { getDoctorVisitsToday } from "../../../services/visitsService";
import { FaUserMd, FaClock, FaDoorOpen } from "react-icons/fa";
import Modal from "react-bootstrap/Modal";
import Button from "react-bootstrap/Button";

const DoctorAppointmentsPage = () => {
    interface TodayVisitsDto {
        id: number;
        visitTime: string;
        patientName: string;
        reason: string;
        room: string;
        specialization: string;
    }

    interface SpecializationDto {
        id: number;
        name: string;
    }

    interface TodayVisitsResponse {
        visits: TodayVisitsDto[];
        specializations: SpecializationDto[];
    }

    const [visitsToday, setVisitsToday] = useState<TodayVisitsDto[]>([]);
    const [specializations, setSpecializations] = useState<SpecializationDto[]>([]);
    const [selectedSpec, setSelectedSpec] = useState<string>("");
    const [loading, setLoading] = useState(true);

    const [activeVisit, setActiveVisit] = useState<TodayVisitsDto | null>(null);
    const [notes, setNotes] = useState("");
    const [prescription, setPrescription] = useState("");
    const [showStartModal, setShowStartModal] = useState(false);

    useEffect(() => {
        getDoctorVisitsToday()
            .then((data: TodayVisitsResponse) => {
                setVisitsToday(data.visits);
                setSpecializations(data.specializations);
            })
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    const sortedVisits = [...visitsToday].sort((a, b) =>
        a.visitTime.localeCompare(b.visitTime)
    );

    const filteredVisits = selectedSpec
        ? sortedVisits.filter(v => v.specialization === selectedSpec)
        : sortedVisits;

    const handleStartVisit = (visit: TodayVisitsDto) => {
        setActiveVisit(visit);
        setNotes("");
        setPrescription("");
        setShowStartModal(true);
    };

    const handleSaveVisit = async () => {
        if (!activeVisit) return;

        try {
            await fetch(`https://localhost:7014/api/visits/startVisit/${activeVisit.id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ notes, prescription })
            });

            alert("Visit updated successfully!");
            setVisitsToday(prev => prev.filter(v => v.id !== activeVisit.id));
            setShowStartModal(false);
        } catch (err) {
            console.error("Error starting visit:", err);
            alert("Error while saving visit.");
        }
    };

    if (loading) return <p className="text-center mt-5">Loading...</p>;

    return (
        <main className="container my-5">
            {/* Header card */}
            <div className="card shadow-sm mb-4">
                <div className="card-body text-center">
                    <h2 className="fw-bold text-primary mb-2">Today's Appointments</h2>
                    <span className="badge bg-light text-dark fs-6">
                        {new Date().toLocaleDateString()}
                    </span>
                </div>
            </div>

            {/* Specialization Filter */}
            {specializations.length > 1 && (
                <div className="mb-4">
                    <label className="form-label fw-semibold">Filter by specialization</label>
                    <select
                        className="form-select w-auto"
                        value={selectedSpec}
                        onChange={(e) => setSelectedSpec(e.target.value)}
                    >
                        <option value="">-- All specializations --</option>
                        {specializations.map(spec => (
                            <option key={spec.id} value={spec.name}>
                                {spec.name}
                            </option>
                        ))}
                    </select>
                </div>
            )}

            {/* Visits Table */}
            <div className="card shadow-sm">
                <div className="card-body">
                    {filteredVisits.length > 0 ? (
                        <table className="table table-hover align-middle text-center">
                            <thead className="table-primary">
                                <tr>
                                    <th><FaClock className="me-1" />Hour</th>
                                    <th><FaUserMd className="me-1" />Patient</th>
                                    <th>Reason</th>
                                    <th><FaDoorOpen className="me-1" />Office</th>
                                    <th>Specialization</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                {filteredVisits.map((v) => (
                                    <tr key={v.id}>
                                        <td className="fw-semibold">{v.visitTime.slice(0, 5)}</td>
                                        <td>{v.patientName}</td>
                                        <td>
                                            <span className="badge bg-info text-dark">
                                                {v.reason}
                                            </span>
                                        </td>
                                        <td>{v.room}</td>
                                        <td>
                                            <span className="badge bg-secondary">
                                                {v.specialization}
                                            </span>
                                        </td>
                                        <td>
                                            <button
                                                className="btn btn-success btn-sm"
                                                onClick={() => handleStartVisit(v)}
                                            >
                                                <i className="bi bi-play-circle me-1"></i>Start visit
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        <p className="text-muted text-center mb-0">
                            No appointments today.
                        </p>
                    )}
                </div>
            </div>

            {/* Modal Start Visit */}
            <Modal show={showStartModal} onHide={() => setShowStartModal(false)} size="lg" centered>
                <Modal.Header closeButton>
                    <Modal.Title>Start your visit</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {activeVisit && (
                        <>
                            <p><strong>Patient:</strong> {activeVisit.patientName}</p>
                            <p><strong>Hour:</strong> {activeVisit.visitTime.slice(0, 5)}</p>
                            <p><strong>Specialization:</strong> {activeVisit.specialization}</p>
                            <p><strong>Room:</strong> {activeVisit.room}</p>

                            <div className="mb-3">
                                <label className="form-label">Doctor's notes</label>
                                <textarea
                                    className="form-control"
                                    rows={3}
                                    value={notes}
                                    onChange={(e) => setNotes(e.target.value)}
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Prescription</label>
                                <textarea
                                    className="form-control"
                                    rows={2}
                                    value={prescription}
                                    onChange={(e) => setPrescription(e.target.value)}
                                />
                            </div>
                        </>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowStartModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="success" onClick={handleSaveVisit}>
                        Save
                    </Button>
                </Modal.Footer>
            </Modal>
        </main>
    );
};

export default DoctorAppointmentsPage;

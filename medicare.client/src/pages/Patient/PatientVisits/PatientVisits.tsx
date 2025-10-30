import { useEffect, useState } from "react";
import { getPatientVisits } from "../../../services/visitsService";
import type { VisitsResponseDto } from "../../../interfaces/visits.types";
import DatePicker from "react-datepicker";
import { FaCalendarAlt } from "react-icons/fa";
import Modal from "react-bootstrap/Modal";
import Button from "react-bootstrap/Button";

export default function PatientVisits() {
    const [visits, setVisits] = useState<VisitsResponseDto[]>([]);
    const [filteredVisits, setFilteredVisits] = useState<VisitsResponseDto[]>([]);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(true);
    const [searchName, setSearchName] = useState("");
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);
    const [upcomingOnly, setUpcomingOnly] = useState(true);
    const [showModal, setShowModal] = useState<VisitsResponseDto | null>(null);

    useEffect(() => {
        getPatientVisits()
            .then((data) => {
                setVisits(data);
                setFilteredVisits(data);
                handleSearch();
            })
            .catch((err) => {
                console.error(err);
                setError("Failed to load visits.");
            })
            .finally(() => setLoading(false));
    }, []);

    const handleSearch = () => {
        const today = new Date();
        const filtered = visits.filter((visit) => {
            const visitDate = new Date(visit.visitDate);
            const matchesName = visit.doctorName.toLowerCase().includes(searchName.toLowerCase());
            let matchesDate;
            if (selectedDate) {
                matchesDate = visitDate.toDateString() === selectedDate.toDateString();
            }
            else {
                matchesDate = true;
            }

            const isUpcoming = !upcomingOnly || visitDate >= today;
            return matchesName && matchesDate && isUpcoming;
        });

        setFilteredVisits(filtered);
        setSearchName("");
        setSelectedDate(null);
    };

    const getStatusBadgeClass = (status: string | undefined) => {
        switch (status) {
            case "Scheduled":
                return "badge bg-warning text-dark";
            case "Completed":
                return "badge bg-success";
            case "Cancelled":
                return "badge bg-danger";
            default:
                return "badge bg-secondary";
        }
    };

    return (
        <div className="container py-5">
            <h2 className="mb-4 text-center">
                <i className="bi bi-calendar-check me-2"></i> My Visits
            </h2>

            {/* Search Panel */}
            <div className="card mb-4">
                <div className="card-body">
                    <div className="row gy-3 align-items-end">
                        <div className="col-md-4">
                            <label className="form-label">Search by doctor name</label>
                            <input
                                type="text"
                                className="form-control"
                                value={searchName}
                                onChange={(e) => setSearchName(e.target.value)}
                                placeholder="e.g. John Doe"
                            />
                        </div>

                        <div className="col-md-4">
                            <label className="form-label">Choose date</label>
                            <div className="input-group">
                                <DatePicker
                                    selected={selectedDate}
                                    onChange={(date) => setSelectedDate(date)}
                                    filterDate={(date) => date.getDay() !== 0 && date.getDay() !== 6}
                                    minDate={new Date()}
                                    dateFormat="yyyy-MM-dd"
                                    className="form-control"
                                    placeholderText="Select a date"
                                />
                                <span className="input-group-text">
                                    <FaCalendarAlt />
                                </span>
                            </div>
                        </div>

                        <div className="col-md-4">
                            <div className="form-check mb-2">
                                <input
                                    type="checkbox"
                                    className="form-check-input"
                                    id="upcomingOnly"
                                    checked={upcomingOnly}
                                    onChange={() => setUpcomingOnly(!upcomingOnly)}
                                />
                                <label className="form-check-label" htmlFor="upcomingOnly">
                                    Only upcoming visits
                                </label>
                            </div>
                            <button className="btn btn-primary w-100" onClick={handleSearch}>
                                Search
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* Visit Cards */}
            <div className="row g-4">
                {loading && <p>Loading visits...</p>}
                {error && <p className="text-danger">{error}</p>}
                {!loading && filteredVisits.length === 0 && !error && (
                    <p>No visits found.</p>
                )}

                {filteredVisits.map((visit) => (
                    <div className="col-md-6 col-lg-4" key={visit.id}>
                        <div className="card shadow-sm h-100">
                            <div className="card-body d-flex flex-column">
                                <h5 className="card-title">
                                    {new Date(visit.visitDate).toLocaleDateString()}{" "}
                                    <small className="text-muted">{visit.visitTime.slice(0, 5)}</small>
                                </h5>
                                <h6 className="card-subtitle mb-2 text-muted">
                                    {visit.specialization}, {visit.room}
                                </h6>
                                <p className="mb-1"><strong>Doctor:</strong> {visit.doctorName}</p>
                                <p className="mb-1"><strong>Reason:</strong> {visit.reason}</p>
                                <p className="mb-2">
                                    <span className={getStatusBadgeClass(visit.status)}>
                                        {visit.status}
                                    </span>
                                </p>
                                <div className="mt-auto text-end">
                                    <button
                                        className="btn btn-outline-primary btn-sm"
                                        onClick={() => setShowModal(visit)}
                                    >
                                        <i className="bi bi-eye me-1"></i> View details
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            {/* Visit Details Modal */}
            <Modal show={Boolean(showModal)} onHide={() => setShowModal(null)} size="lg" centered>
                <Modal.Header closeButton className="border-0">
                    <Modal.Title className="d-flex align-items-center gap-2">
                        <i className="bi bi-file-medical text-primary"></i>
                        Visit details
                    </Modal.Title>
                </Modal.Header>

                <Modal.Body className="pb-0">
                    {showModal && (
                        <div className="card border-0 shadow-sm">
                            {/* Header: date, time, status */}
                            <div className="card-body pb-3">
                                <div className="d-flex flex-wrap align-items-center gap-3">
                                    <div className="d-flex align-items-center gap-2">
                                        <i className="bi bi-calendar3 text-primary"></i>
                                        <span className="fw-semibold">
                                            {new Date(showModal.visitDate).toLocaleDateString()}
                                        </span>
                                    </div>
                                    <div className="vr d-none d-md-block" />
                                    <div className="d-flex align-items-center gap-2">
                                        <i className="bi bi-clock text-primary"></i>
                                        <span className="fw-semibold">{showModal.visitTime.slice(0, 5)}</span>
                                    </div>
                                    <div className="vr d-none d-md-block" />
                                    <span className={getStatusBadgeClass(showModal.status)}>
                                        {showModal.status}
                                    </span>
                                </div>
                            </div>

                            <hr className="my-0" />

                            {/* Key info cards */}
                            <div className="card-body pt-3">
                                <div className="row g-3">
                                    <div className="col-md-6">
                                        <div className="p-3 rounded-3 bg-light h-100">
                                            <div className="d-flex align-items-center gap-2 mb-2">
                                                <i className="bi bi-person-badge text-secondary"></i>
                                                <span className="text-muted text-uppercase small">Doctor</span>
                                            </div>
                                            <div className="fw-semibold">{showModal.doctorName}</div>
                                        </div>
                                    </div>
                                    <div className="col-md-6">
                                        <div className="p-3 rounded-3 bg-light h-100">
                                            <div className="d-flex align-items-center gap-2 mb-2">
                                                <i className="bi bi-briefcase text-secondary"></i>
                                                <span className="text-muted text-uppercase small">Specialization</span>
                                            </div>
                                            <div className="fw-semibold">{showModal.specialization}</div>
                                        </div>
                                    </div>
                                    <div className="col-md-6">
                                        <div className="p-3 rounded-3 bg-light h-100">
                                            <div className="d-flex align-items-center gap-2 mb-2">
                                                <i className="bi bi-hospital text-secondary"></i>
                                                <span className="text-muted text-uppercase small">Room</span>
                                            </div>
                                            <div className="fw-semibold">{showModal.room}</div>
                                        </div>
                                    </div>
                                    <div className="col-md-6">
                                        <div className="p-3 rounded-3 bg-light h-100">
                                            <div className="d-flex align-items-center gap-2 mb-2">
                                                <i className="bi bi-clipboard2-pulse text-secondary"></i>
                                                <span className="text-muted text-uppercase small">Reason</span>
                                            </div>
                                            <div className="fw-semibold">{showModal.reason}</div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {/* Notes section */}
                            <div className="card-body pt-2">
                                <div className="row g-3">
                                    <div className="col-12">
                                        <div className="border rounded-3 p-3">
                                            <div className="d-flex align-items-center gap-2 mb-2">
                                                <i className="bi bi-sticky text-secondary"></i>
                                                <span className="text-muted text-uppercase small">Additional notes</span>
                                            </div>
                                            <p className="mb-0">
                                                {showModal.additionalNotes ? (
                                                    showModal.additionalNotes
                                                ) : (
                                                    <span className="text-muted">None</span>
                                                )}
                                            </p>
                                        </div>
                                    </div>

                                    <div className="col-md-6">
                                        <div className="border rounded-3 p-3 h-100">
                                            <div className="d-flex align-items-center gap-2 mb-2">
                                                <i className="bi bi-journal-medical text-secondary"></i>
                                                <span className="text-muted text-uppercase small">Doctor notes</span>
                                            </div>
                                            <p className="mb-0">
                                                {showModal.visitNotes ? (
                                                    showModal.visitNotes
                                                ) : (
                                                    <span className="text-muted">None</span>
                                                )}
                                            </p>
                                        </div>
                                    </div>

                                    <div className="col-md-6">
                                        <div className="border rounded-3 p-3 h-100">
                                            <div className="d-flex align-items-center gap-2 mb-2">
                                                <i className="bi bi-capsule text-secondary"></i>
                                                <span className="text-muted text-uppercase small">Prescription</span>
                                            </div>
                                            <p className="mb-0">
                                                {showModal.prescriptionText ? (
                                                    showModal.prescriptionText
                                                ) : (
                                                    <span className="text-muted">None</span>
                                                )}
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    )}
                </Modal.Body>

                <Modal.Footer className="border-0">
                    <Button variant="secondary" onClick={() => setShowModal(null)}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>

        </div>
    );
}
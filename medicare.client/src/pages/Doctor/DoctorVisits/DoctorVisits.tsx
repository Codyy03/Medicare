import { useEffect, useState } from "react";
import { getDoctorVisits } from "../../../services/visitsService";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { FaCalendarAlt } from "react-icons/fa";
import Modal from "react-bootstrap/Modal";
import Button from "react-bootstrap/Button";
interface VisitsResponseDto {
    id: number;
    visitDate: Date;
    visitTime: string;
    patientName: string;
    specialization: string;
    room: string;
    roomNumber: number;
    reason: string;
    status: string;
    additionalNotes?: string;
    doctorName: string;
}

const DoctorVisitList = () => {
    const [visits, setVisits] = useState<VisitsResponseDto[]>([]);
    const [filteredVisits, setFilteredVisits] = useState<VisitsResponseDto[]>([]);
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [showModal, setShowModal] = useState(false);
    const [selectedVisit, setSelectedVisit] = useState<VisitsResponseDto | null>(null);

    const [searchName, setSearchName] = useState("");
    const [upcomingOnly, setUpcomingOnly] = useState(true);

    useEffect(() => {
        getDoctorVisits()
            .then((data) => {
                setVisits(data);
                setFilteredVisits(data);
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
            const matchesName = visit.patientName.toLowerCase().includes(searchName.toLowerCase());
            let matchesDate;
            if (selectedDate) {
                matchesDate = visitDate.toDateString() === selectedDate.toDateString();
            }
            else {
                matchesDate = true;
            }

            const isUpcoming = !upcomingOnly || visitDate >= today;
            console.log(visitDate, today, isUpcoming);
            return matchesName && matchesDate && isUpcoming;
        });

        setFilteredVisits(filtered);
        setSearchName("");
        setSelectedDate(null);
    };

    const handleViewDetails = (visit: VisitsResponseDto) => {
        setSelectedVisit(visit);
        setShowModal(true);
    };

    const getStatusBadgeClass = (status: string) => {
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
                            <label className="form-label">Search by patient name</label>
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

            {/* Visit Table */}
            <div className="table-responsive">
                <table className="table table-bordered table-hover align-middle">
                    <thead className="table-light">
                        <tr>
                            <th>Date</th>
                            <th>Patient name</th>
                            <th>Room</th>
                            <th>Reason</th>
                            <th>Status</th>
                            <th>Details</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading && (
                            <tr>
                                <td colSpan={5}>Loading visits...</td>
                            </tr>
                        )}
                        {error && (
                            <tr>
                                <td colSpan={5} className="text-danger">
                                    {error}
                                </td>
                            </tr>
                        )}
                        {!loading && filteredVisits.length === 0 && (
                            <tr>
                                <td colSpan={5}>No visits found.</td>
                            </tr>
                        )}
                        {filteredVisits.map((visit) => (
                            <tr key={visit.id}>
                                <td className="text-center">
                                    <div className="fw-semibold">{new Date(visit.visitDate).toLocaleDateString()}</div>
                                    <div className="text-muted">{visit.visitTime.slice(0, 5)}</div>
                                </td>

                                <td>{visit.patientName}</td>
                                <td>{visit.specialization} - {visit.room} {visit.roomNumber}</td>
                                <td>{visit.reason}</td>
                                <td>
                                    <span className={getStatusBadgeClass(visit.status)}>
                                        {visit.status}
                                    </span>
                                </td>
                                <td className="text-center align-middle">
                                    <button className="btn btn-outline-primary btn-sm" onClick={() => handleViewDetails(visit)}>
                                        <i className="bi bi-eye me-1"></i> View details
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                {/* Visit Details Modal */}
                <Modal show={showModal} onHide={() => setShowModal(false)} size="lg" centered>
                    <Modal.Header closeButton>
                        <Modal.Title>Visit Details</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {selectedVisit && (
                            <div className="card shadow-sm p-4">
                                <h4 className="mb-4">Reservation details</h4>

                                {/* Specialization & Doctor */}
                                <div className="row">
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Specialization</label>
                                        <p className="form-control-plaintext">{selectedVisit.specialization}</p>
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Doctor</label>
                                        <p className="form-control-plaintext">{selectedVisit.doctorName}</p>
                                    </div>
                                </div>

                                {/* Date & Time */}
                                <div className="row">
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Date</label>
                                        <p className="form-control-plaintext">{selectedVisit.visitDate.toString()}</p>
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Time</label>
                                        <p className="form-control-plaintext">{selectedVisit.visitTime.slice(0, 5)}</p>
                                    </div>
                                </div>

                                {/* Patient Info */}
                                <div className="row">
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Patient</label>
                                        <p className="form-control-plaintext">{selectedVisit.patientName}</p>
                                    </div>
                                    <div className="col-md-6 mb-3">
                                        <label className="form-label fw-bold">Room</label>
                                        <p className="form-control-plaintext">{selectedVisit.room} {selectedVisit.roomNumber}</p>
                                    </div>
                                </div>

                                {/* Reason & Notes */}
                                <div className="mb-3">
                                    <label className="form-label fw-bold">Visit reason</label>
                                    <p className="form-control-plaintext">{selectedVisit.reason}</p>
                                </div>

                                <div className="mb-3">
                                    <label className="form-label fw-bold">Additional notes</label>
                                    <p className="form-control-plaintext">
                                        {selectedVisit.additionalNotes || <span className="text-muted">None</span>}
                                    </p>
                                </div>

                                {/* Status */}
                                <div className="mb-3 d-flex align-items-center">
                                    <label className="form-label fw-bold me-2">Status:</label>
                                    <span className={getStatusBadgeClass(selectedVisit.status)}>
                                        {selectedVisit.status}
                                    </span>
                                </div>
                            </div>
                        )}
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={() => setShowModal(false)}>
                            Close
                        </Button>
                    </Modal.Footer>
                </Modal>

            </div>
        </div>
    );
};

export default DoctorVisitList;

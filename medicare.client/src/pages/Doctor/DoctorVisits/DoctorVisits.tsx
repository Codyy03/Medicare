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

const DoctorVisit = () => {
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
                    <Modal.Header closeButton className="border-0">
                        <Modal.Title className="d-flex align-items-center gap-2">
                            <i className="bi bi-person-vcard text-primary"></i>
                            Visit overview
                        </Modal.Title>
                    </Modal.Header>

                    <Modal.Body>
                        {selectedVisit && (
                            <div className="p-3">
                                {/* Top section: Patient + Status */}
                                <div className="d-flex justify-content-between align-items-center mb-4">
                                    <div>
                                        <h5 className="mb-1">
                                            <i className="bi bi-person-circle me-2 text-secondary"></i>
                                            {selectedVisit.patientName}
                                        </h5>
                                        <small className="text-muted">Patient</small>
                                    </div>
                                    <span className={getStatusBadgeClass(selectedVisit.status)}>
                                        {selectedVisit.status}
                                    </span>
                                </div>

                                {/* Date, Time, Room */}
                                <div className="row g-3 mb-4">
                                    <div className="col-md-4">
                                        <div className="border rounded-3 p-3 h-100">
                                            <div className="text-muted small mb-1">
                                                <i className="bi bi-calendar3 me-1"></i>Date
                                            </div>
                                            <div className="fw-semibold">
                                                {new Date(selectedVisit.visitDate).toLocaleDateString()}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="border rounded-3 p-3 h-100">
                                            <div className="text-muted small mb-1">
                                                <i className="bi bi-clock me-1"></i>Time
                                            </div>
                                            <div className="fw-semibold">{selectedVisit.visitTime.slice(0, 5)}</div>
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="border rounded-3 p-3 h-100">
                                            <div className="text-muted small mb-1">
                                                <i className="bi bi-hospital me-1"></i>Room
                                            </div>
                                            <div className="fw-semibold">
                                                {selectedVisit.room} {selectedVisit.roomNumber}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                {/* Specialization & Reason */}
                                <div className="row g-3 mb-4">
                                    <div className="col-md-6">
                                        <div className="bg-light rounded-3 p-3 h-100">
                                            <div className="text-muted small mb-1">
                                                <i className="bi bi-briefcase me-1"></i>Specialization
                                            </div>
                                            <div className="fw-semibold">{selectedVisit.specialization}</div>
                                        </div>
                                    </div>
                                    <div className="col-md-6">
                                        <div className="bg-light rounded-3 p-3 h-100">
                                            <div className="text-muted small mb-1">
                                                <i className="bi bi-clipboard2-pulse me-1"></i>Reason
                                            </div>
                                            <div className="fw-semibold">{selectedVisit.reason}</div>
                                        </div>
                                    </div>
                                </div>

                                {/* Notes */}
                                <div className="mb-3">
                                    <div className="border rounded-3 p-3">
                                        <div className="text-muted small mb-1">
                                            <i className="bi bi-sticky me-1"></i>Additional notes
                                        </div>
                                        <p className="mb-0">
                                            {selectedVisit.additionalNotes || <span className="text-muted">None</span>}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        )}
                    </Modal.Body>

                    <Modal.Footer className="border-0">
                        <Button variant="secondary" onClick={() => setShowModal(false)}>
                            Close
                        </Button>
                    </Modal.Footer>
                </Modal>


            </div>
        </div>
    );
};

export default DoctorVisit;

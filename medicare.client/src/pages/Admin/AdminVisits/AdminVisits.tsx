import { useState, useEffect } from "react";
import { FaEdit } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import type { VisitsResponseDto } from "../../../interfaces/visits.types";
import { getAllVisits } from "../../../services/visitsService";

export default function AdminVisits() {
    const navigate = useNavigate();

    const [visits, setVisits] = useState<VisitsResponseDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getAllVisits()
            .then(data => setVisits(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    // filtry
    const [searchTerm, setSearchTerm] = useState("");
    const [statusFilter, setStatusFilter] = useState("");
    const [specializationFilter, setSpecializationFilter] = useState("");
    const [selectedDate, setSelectedDate] = useState(""); // tylko jedna data

    const filtered = visits.filter((v) => {
        const matchesSearch =
            v.doctorName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            v.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            v.reason.toLowerCase().includes(searchTerm.toLowerCase());

        const matchesStatus = statusFilter ? v.status === statusFilter : true;
        const matchesSpec = specializationFilter ? v.specialization === specializationFilter : true;

        const visitDate = new Date(v.visitDate).toISOString().split("T")[0];
        const matchesDate = selectedDate ? visitDate === selectedDate : true;

        return matchesSearch && matchesStatus && matchesSpec && matchesDate;
    });

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-4">
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="text-center flex-grow-1">Visits Management</h2>
            </div>

            {/* Filtry */}
            <div className="row mb-3 g-2">
                <div className="col-md-3">
                    <input
                        type="text"
                        placeholder="Search doctor/patient/reason..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="form-control"
                    />
                </div>
                <div className="col-md-2">
                    <select
                        className="form-select"
                        value={statusFilter}
                        onChange={(e) => setStatusFilter(e.target.value)}
                    >
                        <option value="">All Statuses</option>
                        <option value="Scheduled">Scheduled</option>
                        <option value="Completed">Completed</option>
                        <option value="Cancelled">Cancelled</option>
                    </select>
                </div>
                <div className="col-md-2">
                    <select
                        className="form-select"
                        value={specializationFilter}
                        onChange={(e) => setSpecializationFilter(e.target.value)}
                    >
                        <option value="">All Specializations</option>
                        {[...new Set(visits.map(v => v.specialization))].map(spec => (
                            <option key={spec} value={spec}>{spec}</option>
                        ))}
                    </select>
                </div>
                <div className="col-md-2">
                    <input
                        type="date"
                        className="form-control"
                        value={selectedDate}
                        onChange={(e) => setSelectedDate(e.target.value)}
                    />
                </div>
            </div>

            {/* Tabela */}
            <table className="table table-striped table-hover shadow">
                <thead className="table-primary">
                    <tr>
                        <th>Date</th>
                        <th>Time</th>
                        <th>Doctor</th>
                        <th>Patient</th>
                        <th>Status</th>
                        <th>Reason</th>
                        <th>Room</th>
                        <th>Specialization</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {filtered.map((v) => (
                        <tr key={v.id}>
                            <td>{new Date(v.visitDate).toLocaleDateString()}</td>
                            <td>{v.visitTime.slice(0, 5)}</td>
                            <td>{v.doctorName}</td>
                            <td>{v.patientName}</td>
                            <td>{v.status}</td>
                            <td>{v.reason}</td>
                            <td>{v.room}</td>
                            <td>{v.specialization}</td>
                            <td className="d-flex justify-content-center align-items-center">
                                <button
                                    className="btn btn-sm btn-primary me-2"
                                    onClick={() => navigate(`/admin/adminVisitsEdit/${v.id}`)}
                                >
                                    <FaEdit />
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

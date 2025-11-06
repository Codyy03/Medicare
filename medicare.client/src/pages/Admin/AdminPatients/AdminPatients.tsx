import { useState, useEffect } from "react";
import { FaEdit, FaTrash, FaPlus, FaSearch, FaUndo } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { getAdminPatients, deleteAdminPatient } from "../../../services/patientsService";
import type { Patient } from "../../../interfaces/patients.types";

export default function AdminPatients() {
    const navigate = useNavigate();

    const [patients, setPatients] = useState<Patient[]>([]);
    const [filteredPatients, setFilteredPatients] = useState<Patient[]>([]);
    const [loading, setLoading] = useState(true);

    const [peselFilter, setPeselFilter] = useState("");
    const [emailFilter, setEmailFilter] = useState("");

    useEffect(() => {
        fetchPatients();
    }, []);

    const fetchPatients = async () => {
        setLoading(true);
        try {
            const data = await getAdminPatients();
            setPatients(data);
            setFilteredPatients(data);
        } catch (err) {
            console.error("Failed to fetch patients:", err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (window.confirm("Are you sure you want to delete this patient?")) {
            try {
                await deleteAdminPatient(id);
                const updated = patients.filter((p) => p.id !== id);
                setPatients(updated);
                setFilteredPatients(updated);
            } catch (err) {
                console.error(err);
                alert("Failed to delete patient");
            }
        }
    };

    const handleSearch = () => {
        const filtered = patients.filter((p) =>
            (peselFilter ? p.pesel.includes(peselFilter) : true) &&
            (emailFilter ? p.email.toLowerCase().includes(emailFilter.toLowerCase()) : true)
        );
        setFilteredPatients(filtered);
    };

    const handleReset = () => {
        setPeselFilter("");
        setEmailFilter("");
        setFilteredPatients(patients);
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-4">
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="text-center flex-grow-1">Patients Management</h2>
                <button
                    className="btn btn-success ms-3"
                    onClick={() => navigate("/admin/adminPatientCreate")}
                >
                    <FaPlus className="me-2" /> Add Patient
                </button>
            </div>

            {/* Filters */}
            <div className="card p-3 mb-4 shadow-sm">
                <div className="row g-2">
                    <div className="col-md-5">
                        <input
                            type="text"
                            placeholder="Search by PESEL"
                            value={peselFilter}
                            onChange={(e) => setPeselFilter(e.target.value)}
                            className="form-control"
                        />
                    </div>
                    <div className="col-md-5">
                        <input
                            type="text"
                            placeholder="Search by Email"
                            value={emailFilter}
                            onChange={(e) => setEmailFilter(e.target.value)}
                            className="form-control"
                        />
                    </div>
                    <div className="col-md-1">
                        <button className="btn btn-primary w-100" onClick={handleSearch}>
                            <FaSearch />
                        </button>
                    </div>
                    <div className="col-md-1">
                        <button className="btn btn-secondary w-100" onClick={handleReset}>
                            <FaUndo />
                        </button>
                    </div>
                </div>
            </div>

            {/* Table */}
            <table className="table table-striped table-hover shadow">
                <thead className="table-primary">
                    <tr>
                        <th>Name</th>
                        <th>Surname</th>
                        <th>PESEL</th>
                        <th>Birthday</th>
                        <th>Email</th>
                        <th>Phone</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredPatients.map((patient) => (
                        <tr key={patient.id}>
                            <td>{patient.name}</td>
                            <td>{patient.surname}</td>
                            <td>{patient.pesel}</td>
                            <td>{patient.birthday.slice(0, 10)}</td>
                            <td>{patient.email}</td>
                            <td>{patient.phoneNumber}</td>
                            <td>
                                <button
                                    className="btn btn-sm btn-primary me-2"
                                    onClick={() => navigate(`/admin/patientEdit/${patient.id}`)}
                                >
                                    <FaEdit />
                                </button>
                                <button
                                    className="btn btn-sm btn-danger"
                                    onClick={() => handleDelete(patient.id)}
                                >
                                    <FaTrash />
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

import { useState, useEffect } from "react";
import { FaEdit, FaTrash, FaPlus } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { getAdminSpecializations, deleteAdminSpecialization } from "../../../services/specializationsService";
import type { Specialization } from "../../../interfaces/specialization.types";

export default function AdminSpecializations() {
    const navigate = useNavigate();
    const [specializations, setSpecializations] = useState<Specialization[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState("");

    useEffect(() => { fetchList(); }, []);

    const fetchList = async () => {
        setLoading(true);
        try {
            const data = await getAdminSpecializations();
            setSpecializations(data);
        } catch (err) {
            console.error(err);
            alert("Unable to load specializations");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm("Are you sure you want to delete this specialization?")) return;
        try {
            await deleteAdminSpecialization(id);
            setSpecializations(prev => prev.filter(s => s.id !== id));
        } catch (err: any) {
            console.error(err);
            alert("Failed to delete specialization: " + (err.message ?? err));
        }
    };

    const filtered = specializations.filter(s => s.specializationName?.toLowerCase().includes(searchTerm.toLowerCase()));

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-4">
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="text-center flex-grow-1">Specializations Management</h2>
                <button className="btn btn-success ms-3" onClick={() => navigate("/admin/specializationCreate")}>
                    <FaPlus className="me-2" /> Add Specialization
                </button>
            </div>

            <div className="mb-3">
                <input type="text" placeholder="Search specialization..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} className="form-control" />
            </div>

            <table className="table table-striped table-hover shadow">
                <thead className="table-primary">
                    <tr>
                        <th>Name</th>
                        <th>Highlight</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {filtered.map(spec => (
                        <tr key={spec.id}>
                            <td>{spec.specializationName}</td>
                            <td>{spec.specializationHighlight}</td>
                            <td>{spec.specializationDescription}</td>
                            <td>
                                <button className="btn btn-sm btn-primary me-2" onClick={() => navigate(`/admin/specializationEdit/${spec.id}`)}><FaEdit /></button>
                                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(spec.id)}><FaTrash /></button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

import { useState, useEffect } from "react";
import { FaEdit, FaTrash, FaPlus } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { getAllNews, deleteAdminNews } from "../../../services/newsService";

interface NewsItem {
    id: number;
    title: string;
    description: string;
    imageURL?: string;
    date: string;
}

export default function AdminNews() {
    const navigate = useNavigate();

    const [news, setNews] = useState<NewsItem[]>([])
    const [loading, setLoading] = useState(true);

    const [searchTerm, setSearchTerm] = useState("");

    useEffect(() => {
        getAllNews().
            then(data => setNews(data)).
            catch(err => console.log(err)).
            finally(() => setLoading(false))
    })

    const filtered = news.filter(
        (n) =>
            n.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
            n.description.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading) return <p>Loading...</p>

    return (
        <div className="container py-4">
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="text-center flex-grow-1">News Management</h2>
                <button
                    className="btn btn-success ms-3"
                    onClick={() => navigate("/admin/adminNewsEdit")}
                >
                    <FaPlus className="me-2" /> Add News
                </button>

            </div>

            <div className="mb-3">
                <input
                    type="text"
                    placeholder="Search news..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="form-control"
                />
            </div>

            <table className="table table-striped table-hover shadow">
                <thead className="table-primary">
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>Date</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {filtered.map((n) => (
                        <tr key={n.id}>
                            <td>{n.title}</td>
                            <td>{n.description}</td>
                           
                            <td>{new Date(n.date).toLocaleDateString()}</td>
                            <td>
                                <button
                                    className="btn btn-sm btn-primary me-2"
                                    onClick={() => navigate(`/admin/adminNewsEdit/${n.id}`)}
                                >
                                    <FaEdit />
                                </button>
                                <button
                                    className="btn btn-sm btn-danger"
                                    onClick={async () => {
                                        if (!window.confirm("Are you sure you want to delete this news?")) return;
                                        try {
                                            await deleteAdminNews(n.id);
                                            setNews(prev => prev.filter(item => item.id !== n.id));
                                        } catch (err) {
                                            console.error(err);
                                            alert("Failed to delete news");
                                        }
                                    }}
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

import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { FaAlignLeft, FaSave, FaArrowLeft, FaNewspaper, FaImage, FaCalendar } from "react-icons/fa";
import { getNewsByID, updateAdminNews, createAdminNews } from "../../../services/newsService";
import type { NewsItem } from "../../../interfaces/news.types";

export default function AdminNewsEdit() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const isEdit = !!id && !isNaN(Number(id));

    const [news, setNews] = useState<NewsItem>({
        id: 0,
        title: "",
        description: "",
        imageURL: "",
        date: new Date().toISOString().split("T")[0] // domyœlnie dzisiejsza data
    });
    const [loading, setLoading] = useState(isEdit);

    useEffect(() => {
        if (!isEdit) { setLoading(false); return; }
        (async () => {
            try {
                const data = await getNewsByID(Number(id));
                // upewniamy siê, ¿e data jest w formacie YYYY-MM-DD dla input[type=date]
                setNews({ ...data, date: data.date.split("T")[0] });
            } catch (err) {
                console.error(err);
                alert("Failed to load news");
                navigate("/admin/news");
            } finally {
                setLoading(false);
            }
        })();
    }, [id]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
        setNews(prev => ({ ...prev, [e.target.name]: e.target.value }));

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (isEdit) {
                await updateAdminNews(news.id, {
                    title: news.title,
                    description: news.description,
                    imageURL: news.imageURL,
                    date: new Date(news.date).toISOString()
                });
                alert("News updated");
            }
             else {
                await createAdminNews({
                    title: news.title,
                    description: news.description,
                    imageURL: news.imageURL,
                    date: new Date(news.date).toISOString()
                });
                alert("News created");
            }
            navigate("/admin/adminNews");
        } catch (err: any) {
            console.error(err);
            alert("Error saving news: " + (err.message ?? ""));
        }
    };

    const handleDelete = async (id: number) => {
        if (window.confirm("Are you sure you want to delete this news?")) {
            try {
                await deleteDoctor(id);
                setDoctors(doctors.filter((doc) => doc.id !== id));
            } catch (err) {
                console.error(err);
                alert("Failed to delete news");
            }
        }
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-4">
            <div className="card shadow-lg p-4">
                <h2 className="mb-4 text-center text-primary">{isEdit ? "Edit News" : "Add News"}</h2>
                <form onSubmit={handleSubmit}>
                    <div className="row g-3">
                        <div className="col-md-6">
                            <label className="form-label"><FaNewspaper className="me-2" />Title</label>
                            <input type="text" name="title" value={news.title} onChange={handleChange} className="form-control" required />
                        </div>
                        <div className="col-md-6">
                            <label className="form-label"><FaImage className="me-2" />Image URL</label>
                            <input type="text" name="imageURL" value={news.imageURL ?? ""} onChange={handleChange} className="form-control" />
                        </div>
                        <div className="col-md-12">
                            <label className="form-label"><FaAlignLeft className="me-2" />Description</label>
                            <textarea name="description" value={news.description ?? ""} onChange={handleChange} className="form-control" rows={4} required />
                        </div>
                        <div className="col-md-6">
                            <label className="form-label"><FaCalendar className="me-2" />Date</label>
                            <input type="date" name="date" value={news.date} onChange={handleChange} className="form-control" required />
                        </div>
                    </div>

                    <div className="d-flex justify-content-between mt-4">
                        <button type="button" className="btn btn-outline-secondary" onClick={() => navigate("/admin/adminNews")}>
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

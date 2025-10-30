import { useEffect, useState } from 'react';
import { useParams } from "react-router-dom";
import { getNewsByID } from "../../../services/newsService";
import type { NewsItem } from "../../../interfaces/news.types";
import "./SelectedNews.css";
export default function SelectedNews() {
    const [news, setNews] = useState<NewsItem | null>(null);
    const [loading, setLoading] = useState(true);
    const { id } = useParams<{ id: string }>();

    useEffect(() => {
        const fetchNews = async () => {
            try {
                if (!id) return;
                const data = await getNewsByID(Number(id));
                setNews(data);
            } catch (err) {
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchNews();
    }, [id]);

    if (loading) return <p>Loading...</p>;
    if (!news) return <p>News not found</p>;
    return(
        <div className="selectedNews-container">
            {/* HERO */}
            <section className="newsPage-hero">
                <div className="newsPage-hero-content">
                    <h1>News</h1>
                    <p>Stay up to date with our events</p>
                </div>
            </section>

            {/* CONTENT */}
            <section className="selectedNews-body container">
                <div className="row justify-content-center">
                
                    <h1 className="selectedNews-title">{news.title}</h1>
                    <p className="selectedNews-date">
                        {new Date(news.date).toLocaleDateString("en-EN", {
                            day: "numeric",
                            month: "long",
                            year: "numeric",
                        })}
                    </p>
                        <div className="selectedNews-description">
                            <p>{news.description}</p>
                        </div>
                </div>
            </section>
        </div>

    );
}
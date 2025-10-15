import { Link } from "react-router-dom";
import { useEffect, useState } from 'react';
import { getAllNews } from "../../../services/newsService";
import 'bootstrap/dist/css/bootstrap.min.css';
import "./AllNews.css";

export default function AllNews() {
    interface NewsItem {
        id: number;
        title: string;
        description: string;
        imageURL: string;
        date: string;
    }
    const [news, setNews] = useState<NewsItem[]>([]);
    const [loading, setLoading] = useState(true);

    const monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];

    const now = new Date();
    const currentMonth = now.getMonth();
    const currentYear = now.getFullYear();

    const lastFiveMonths = Array.from({ length: 5 }, (_, i) => {
        const date = new Date(currentYear, currentMonth - i, 1);
        return {
            month: date.getMonth() + 1,
            year: date.getFullYear(),
            label: `${monthNames[date.getMonth()]} ${date.getFullYear()}`
        };
    });

    useEffect(() => {
        const fetchLatest = async () => {
            try {
                const data = await getAllNews("desc");
                setNews(data);
            } catch (err) {
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        fetchLatest();
    }, []);

    const handleFilterMonth = async (month: number, year: number) => {
        setLoading(true);
        try {
            const data = await getAllNews("desc", month, year);
            setNews(data);
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const showAllNewsDesc = async () => {
        setLoading(true);
        try {
            const data = await getAllNews("desc");
            setNews(data);
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <p>Loading...</p>;
    return (
        <div className="newsPage">
            {/* HERO */}
            <section className="newsPage-hero">
                <div className="newsPage-hero-content">
                    <h1>News</h1>
                    <p>Stay up to date with our events</p>
                </div>
            </section>

            {/* main section */}
            <div className="container my-5">
                <div className="row">
                    {/* Archives */}
                    <aside className="col-md-3 newsPage-archive">
                        <h3>Archives</h3>
                        <ul>
                            {lastFiveMonths.map((m, idx) => (
                                <li key={idx} className="archive-li">
                                    <button type="button" onClick={() => handleFilterMonth(m.month, m.year)}>
                                        {m.label}
                                    </button>
                                </li>
                            ))}
                            <li className="archive-li"> <button type="button" onClick={() => showAllNewsDesc()}> Show all</button> </li>
                        </ul>
                    </aside>

                    {/* list news */}
                    <div className="col-md-9">
                        <div className="row g-4">
                            {news.map((item, index) => (
                                <div className="col-md-6" key={index}>
                                    <div className="card h-100 shadow-sm newsPage-card-box">
                                        <div className="card-body d-flex flex-column align-items-center text-center">
                                            <small className="text-muted newsPage-date">
                                                {new Date(item.date).toISOString().split("T")[0]}
                                            </small>
                                            <h4 className="card-title mt-2">
                                                <Link
                                                    to={`/selectedNews/${item.id}`}
                                                    className="text-decoration-none text-dark newsPage-title-link"
                                                >
                                                    {item.title}
                                                </Link>
                                            </h4>
                                            {item.imageURL && (
                                                <Link to={`/selectedNews/${item.id}`}>
                                                    <img
                                                        src={item.imageURL}
                                                        className="card-img-top img-fluid my-3 newsPage-img"
                                                        alt={item.title}
                                                    />
                                                </Link>
                                            )}
                                            <Link to={`/selectedNews/${item.id}`} className="newsPage-more-link">
                                                More &gt;
                                            </Link>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
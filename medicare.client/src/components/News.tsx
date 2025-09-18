import { Link} from "react-router-dom";
import { useEffect, useState } from 'react';
import { getNews } from "../services/newsService";
import 'bootstrap/dist/css/bootstrap.min.css';
import './News.css';

function News() {
    interface NewsItem {
        title: string;
        description: string;
        imageURL: string;
        date: string;
    }
    const [news, setNews] = useState<NewsItem[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getNews()
            .then(data => setNews(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container news-card my-5 d-flex flex-column align-items-center text-center">
            <h2 className="mb-4">NEWS</h2>
            <div className="row g-4">
                {news.map((item, index) => (
                    <div className="col-md-4" key={index}>
                        <div className="card h-100 shadow-sm news-card-box">
                            <div className="card-body d-flex flex-column align-items-center text-center">
                                <small className="text-muted">
                                    {new Date(item.date).toISOString().split("T")[0]}
                                </small>
                                <h4 className="card-title mt-2">
                                    <Link to={"#"} className="text-decoration-none text-dark">
                                        {item.title}
                                    </Link>
                                </h4>
                                {item.imageURL && (
                                    <Link to={"#"}>
                                        <img
                                            src={item.imageURL}
                                            className="card-img-top img-fluid my-3"
                                            alt={item.title}
                                        />
                                    </Link>
                                )}
                                <Link to={"#"} className="">
                                    More &gt;
                                </Link>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            <div className="mt-4">
                <Link to="/news" className="btn-anim">
                    <span>More news &gt;</span>  
                </Link>
            </div>
        </div>

    );
}

export default News;
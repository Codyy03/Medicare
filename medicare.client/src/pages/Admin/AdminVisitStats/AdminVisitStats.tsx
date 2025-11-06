import { useEffect, useState } from "react";
import {
    BarChart, Bar,
    LineChart, Line,
    XAxis, YAxis, Tooltip, Legend, CartesianGrid,
    PieChart, Pie, Cell
} from "recharts";
import api from "../../../services/api";

interface DailyVisitStats {
    date: string;
    total: number;
    completed: number;
    cancelled: number;
    scheduled: number;
}

interface VisitSummary {
    total: number;
    completed: number;
    cancelled: number;
    scheduled: number;
}

const COLORS = ["#8884d8", "#82ca9d", "#ff6961"];

export default function AdminVisitStats() {
    const [stats, setStats] = useState<DailyVisitStats[]>([]);
    const [summary, setSummary] = useState<VisitSummary | null>(null);

    useEffect(() => {
        api.get("https://localhost:7014/api/visitStatistics/stats/daily?days=30")
            .then(res => setStats(res.data))
            .catch(err => console.error(err));

        api.get("https://localhost:7014/api/visitStatistics/stats/summary")
            .then(res => setSummary(res.data))
            .catch(err => console.error(err));
    }, []);

    const pieData = summary ? [
        { name: "Scheduled", value: summary.scheduled },
        { name: "Completed", value: summary.completed },
        { name: "Cancelled", value: summary.cancelled }
    ] : [];

    return (
        <main className="container my-5 text-center">
            <h1 className="mb-4 text-primary fw-bold">Visit Statistics Dashboard</h1>

            {/* Podsumowanie liczbowe */}
            {summary && (
                <div className="row text-center mb-4">
                    <div className="col-md-3">
                        <div className="card shadow-sm p-3">
                            <h6>Total Visits</h6>
                            <h3 className="text-dark">{summary.total}</h3>
                        </div>
                    </div>
                    <div className="col-md-3">
                        <div className="card shadow-sm p-3">
                            <h6>Scheduled</h6>
                            <h3 className="text-primary">{summary.scheduled}</h3>
                        </div>
                    </div>
                    <div className="col-md-3">
                        <div className="card shadow-sm p-3">
                            <h6>Completed</h6>
                            <h3 className="text-success">{summary.completed}</h3>
                        </div>
                    </div>
                    <div className="col-md-3">
                        <div className="card shadow-sm p-3">
                            <h6>Cancelled</h6>
                            <h3 className="text-danger">{summary.cancelled}</h3>
                        </div>
                    </div>
                </div>
            )}

            <div className="row">
                <div className="col-lg-6 mb-4">
                    <h5>Daily Visits (last 30 days)</h5>
                    <LineChart width={500} height={300} data={stats}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="date" />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Line type="monotone" dataKey="total" stroke="#8884d8" />
                        <Line type="monotone" dataKey="completed" stroke="#82ca9d" />
                        <Line type="monotone" dataKey="cancelled" stroke="#ff6961" />
                    </LineChart>
                </div>

                <div className="col-lg-6 mb-4">
                    <h5>Status Distribution (per day)</h5>
                    <BarChart width={500} height={300} data={stats}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="date" />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="scheduled" fill="#8884d8" />
                        <Bar dataKey="completed" fill="#82ca9d" />
                        <Bar dataKey="cancelled" fill="#ff6961" />
                    </BarChart>
                </div>
            </div>

            <div className="row">
                <div className="col-lg-4 mb-4 ">
                    <h5 className="text-center">Overall Status Distribution</h5>
                    <PieChart width={400} height={300}>
                        <Pie
                            data={pieData}
                            dataKey="value"
                            nameKey="name"
                            cx="50%"
                            cy="50%"
                            outerRadius={100}
                            label
                        >
                            {pieData.map((_, index) => (
                                <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                            ))}
                        </Pie>
                        <Legend />
                    </PieChart>
                </div>
            </div>
        </main>
    );
}

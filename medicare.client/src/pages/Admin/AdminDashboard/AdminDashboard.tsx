import { NavLink } from "react-router-dom";
import { FaUserMd, FaUsers, FaCalendarAlt, FaDoorOpen, FaListAlt, FaNewspaper } from "react-icons/fa";
import "./AdminDashboard.css";

export default function AdminDashboard() {
    const tiles = [
        { title: "Doctors", path: "/admin/doctors", icon: <FaUserMd size={25} /> },
        { title: "Patients", path: "/admin/patients", icon: <FaUsers size={25} /> },
        { title: "Visits", path: "/admin/visits", icon: <FaCalendarAlt size={25} /> },
        { title: "Rooms", path: "/admin/rooms", icon: <FaDoorOpen size={25} /> },
        { title: "Specializations", path: "/admin/specializations", icon: <FaListAlt size={25} /> },
        { title: "News", path: "/admin/news", icon: <FaNewspaper size={25} /> },
    ];

    return (
        <div className="container py-5">
            <h2 className="mb-4 text-center fw-bold display-6 text-primary">
                Admin Panel
            </h2>
            <div className="row g-4">
                {tiles.map((tile) => (
                    <div className="col-12 col-sm-6 col-md-4" key={tile.title}>
                        <NavLink to={tile.path} className="text-decoration-none">
                            <div className="admin-card h-100 shadow">
                                <div className="card-body d-flex flex-column align-items-center justify-content-center text-center">
                                    <div className="mb-2">{tile.icon}</div>
                                    <h4 className="card-title text-white m-0">{tile.title}</h4>
                                </div>
                            </div>
                        </NavLink>
                    </div>
                ))}
            </div>
        </div>
    );
}

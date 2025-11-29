import { useState, useEffect } from "react";
import { FaEdit, FaTrash, FaPlus } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import type { RoomsDto } from "../../../interfaces/rooms.types";
import { getRooms } from "../../../services/roomsService.ts";
import { deleteRoom } from "../../../services/roomsService.ts";

export default function AdminRoom() {
    const navigate = useNavigate();

    const [rooms, setRooms] = useState<RoomsDto[]>([]);
    const [filteredRooms, setFilteredRooms] = useState<RoomsDto[]>([]);
    const [loading, setLoading] = useState(true);

    const [roomTypeFilter, setRoomTypeFilter] = useState("");

    useEffect(() => {
        getRooms()
        .then((data) => {
            setRooms(data);
            setFilteredRooms(data);
        }).catch((err) => {
            console.error(err);
        }).finally(() => {
            setLoading(false);
        })

    }, []);

    function handleDelete(id: number) {
        if (window.confirm("Are you sure you want to delete " + filteredRooms.find(room => room.id == id)?.roomNumber + " room?")) {
            try {
                deleteRoom(id);
                const updated = filteredRooms.filter((p) => p.id !== id);
                setRooms(updated);
                setFilteredRooms(updated);
            } catch (err) {
                console.error(err);
                alert("Failed to delete room");
            }
        }
    };

    function handleSearch(text: string) {
        setRoomTypeFilter(text);
        const filtered = rooms.filter((p) =>
            p.roomType.includes(text)
        );
        setFilteredRooms(filtered);
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-4">
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="text-center flex-grow-1">Rooms Management</h2>
                <button
                    className="btn btn-success ms-3"
                    onClick={() => navigate("/admin/adminRoomCreate")}
                >
                    <FaPlus className="me-2" /> Add Room
                </button>
            </div>

            <div className="mb-3">
                <input type="text" placeholder="Search specialization..." value={roomTypeFilter} onChange={(e) => handleSearch(e.target.value)} className="form-control" />
            </div>

            {/* Table */}
            <table className="table table-striped table-hover shadow">
                <thead className="table-primary">
                    <tr>
                        <th>RoomNumber</th>
                        <th>RoomType</th>
                        <th>Specializations</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {filteredRooms.map((room) => (
                        <tr key={room.id}>
                            <td>{room.roomNumber}</td>
                            <td>{room.roomType}</td>
                            <td>{room.specializations.map(spec => spec.specializationName).join(", ")}</td>
                            <td>
                                <button
                                    className="btn btn-sm btn-primary me-2"
                                    onClick={() => navigate(`/admin/AdminRoomEdit/${room.id}`)}
                                >
                                    <FaEdit />
                                </button>
                                <button
                                    className="btn btn-sm btn-danger"
                                    onClick={() => handleDelete(room.id!)}
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

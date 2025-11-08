import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { FaUserMd, FaArrowLeft, FaSave, FaUser } from "react-icons/fa";
import type { RoomsDto } from "../../../interfaces/rooms.types";
import { getAdminRoomById, updateAdminRoom } from "../../../services/roomsService";
import Select from "react-select";
import type { SpecializationNameDto } from "../../../interfaces/specialization.types";
import { getSpecializationNames } from "../../../services/specializationsService";

export default function RoomEdit() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [room, setRoom] = useState<RoomsDto | null>(null);
    const [specializations, setSpecializations] = useState<SpecializationNameDto[]>([]);

    useEffect(() => {
        getAdminRoomById(Number(id))
            .then((room: RoomsDto | null) => {
                setRoom(room);
            })
            .catch((err: Error) => console.error(err))
            .finally(() => setLoading(false));
        getSpecializationNames()
            .then(data => setSpecializations(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);


    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        if (room) {
            setRoom({ ...room, [e.target.name]: e.target.value });
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (room) {
            try {
                await updateAdminRoom(room);
                alert("Rooms updated successfully");
                navigate("/admin/rooms");
            } catch (err: any) {
                alert("Error updating rooms: " + err.message);
            }
        }

    };

    if (loading) return <p>Loading...</p>;
    if (!room) return <p>Room not found</p>;

    return (
        <div className="container py-4">
            <div className="card shadow-lg p-4">
                <h2 className="mb-4 text-center text-primary">Edit Room</h2>
                <form onSubmit={handleSubmit} className="room-edit-form">
                    <div className="row g-3">
                        {/* Room Number */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUser className="me-2" />Room Number</label>
                            <input type="text" name="roomNumber" value={room.roomNumber} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Room Type */}
                        <div className="col-md-6">
                            <label className="form-label"><FaUser className="me-2" />Room Type</label>
                            <input type="text" name="roomType" value={room.roomType} onChange={handleChange} className="form-control" required />
                        </div>

                        {/* Specializations */}
                        <div className="col-md-12">
                            <label className="form-label"><FaUserMd className="me-2" />Specializations</label>
                            <Select<SpecializationNameDto, true>
                                isMulti
                                name="specializations"
                                options={specializations}
                                getOptionLabel={(spec) => spec.specializationName}
                                getOptionValue={(spec) => spec.id}
                                value={room.specializations}
                                onChange={(selected) => {
                                    setRoom({ ...room, specializations: selected ?? [] });
                                }}
                            />
                        </div>
                    </div>

                    <div className="d-flex justify-content-between mt-4">
                        <button type="button" className="btn btn-outline-secondary" onClick={() => navigate("/admin/rooms")}>
                            <FaArrowLeft className="me-2" /> Back
                        </button>
                        <button type="submit" className="btn btn-primary">
                            <FaSave className="me-2" /> Save Changes
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

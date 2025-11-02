import React, { useState, useEffect} from "react";
import { FaEdit, FaTrash, FaPlus } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import "./AdminDoctors.css";
import { getDoctors, deleteDoctor } from "../../../services/doctorsService"
import type { DoctorDto } from "../../../interfaces/doctors.types";

export default function AdminDoctors() {
    const navigate = useNavigate();

    const [doctors, setDoctors] = useState<DoctorDto[]>([])
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getDoctors().
            then(data => setDoctors(data)).
            catch(err => console.log(err)).
            finally(() => setLoading(false))
    },[])

    const handleDelete = async (id: number) => {
        if (window.confirm("Are you sure you want to delete this doctor?")) {
            try {
                await deleteDoctor(id);
                setDoctors(doctors.filter((doc) => doc.id !== id));
            } catch (err) {
                console.error(err);
                alert("Failed to delete doctor");
            }
        }
    };

    if (loading) return <p> Loading... </p>

    return (
        <div className="container py-4">
            <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="text-center flex-grow-1">Doctors Management</h2>
                <button
                    className="btn btn-success ms-3"
                    onClick={() => navigate("/admin/adminDoctorCreate")}
                >
                    <FaPlus className="me-2" /> Add Doctor
                </button>
            </div>

            <table className="table table-striped table-hover shadow">
                <thead className="table-primary">
                    <tr>
                        <th>Name</th>
                        <th>Surname</th>
                        <th>Email</th>
                        <th>Phone</th>
                        <th>Facility</th>
                        <th>Hours</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {doctors.map((doc) => (
                        <tr key={doc.id}>
                            <td>{doc.name}</td>
                            <td>{doc.surname}</td>
                            <td>{doc.email}</td>
                            <td>{doc.phoneNumber}</td>
                            <td>{doc.facility}</td>
                            <td>
                                {doc.startHour.slice(0, 5)} - {doc.endHour.slice(0, 5)}
                            </td>
                            <td>
                                <button
                                    className="btn btn-sm btn-primary me-2"
                                    onClick={() => navigate(`/admin/doctorEdit/${doc.id}`)}
                                >
                                    <FaEdit />
                                </button>
                                <button
                                    className="btn btn-sm btn-danger"
                                    onClick={() => handleDelete(doc.id)}
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

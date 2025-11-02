import React, { useState, useEffect } from "react";
import { FaEdit, FaTrash, FaPlus } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import "./AdminDoctors.css";
import { getDoctorsByFilters, deleteDoctor } from "../../../services/doctorsService";
import { getSpecializationNames } from "../../../services/specializationsService";
import type { DoctorDto } from "../../../interfaces/doctors.types";
import type { SpecializationsNamesID } from "../../../interfaces/visits.types";

export default function AdminDoctors() {
    const navigate = useNavigate();

    const [doctors, setDoctors] = useState<DoctorDto[]>([]);
    const [loading, setLoading] = useState(true);

    const [surname, setSurname] = useState("");
    const [specializationID, setSpecializationID] = useState<number | undefined>();
    const [availableAt, setAvailableAt] = useState("");
    const [specializations, setSpecializations] = useState<SpecializationsNamesID[]>([]);

    useEffect(() => {
        // get specializations
        getSpecializationNames()
            .then(setSpecializations)
            .catch(console.error);
    }, []);

    useEffect(() => {
        // get all doctors
        handleFilter();
    }, []);

    const handleFilter = async () => {
        setLoading(true);
        try {
            const data = await getDoctorsByFilters(specializationID, surname, availableAt);
            setDoctors(data);
        } catch (err) {
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleReset = async () => {
        setSurname("");
        setSpecializationID(undefined);
        setAvailableAt("");
        await handleFilter();
    };

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

    if (loading) return <p>Loading...</p>;

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

            {/*  filters */}
            <div className="card p-3 mb-4 shadow-sm">
                <div className="row g-2">
                    <div className="col-md-4">
                        <input
                            type="text"
                            placeholder="Surname"
                            value={surname}
                            onChange={(e) => setSurname(e.target.value)}
                            className="form-control"
                        />
                    </div>
                    <div className="col-md-4">
                        <select
                            className="form-select"
                            value={specializationID ?? ""}
                            onChange={(e) =>
                                setSpecializationID(e.target.value ? Number(e.target.value) : undefined)
                            }
                        >
                            <option value="">All specializations</option>
                            {specializations.map((spec) => (
                                <option key={spec.id} value={spec.id}>
                                    {spec.specializationName}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="col-md-3">
                        <input
                            type="time"
                            value={availableAt}
                            onChange={(e) => setAvailableAt(e.target.value)}
                            className="form-control"
                        />
                    </div>
                    <div className="col-md-1">
                        <button className="btn btn-primary w-100" onClick={handleFilter}>
                            Filter
                        </button>
                    </div>
                    <div className="col-md-1">
                        <button className="btn btn-secondary w-100" onClick={handleReset}>
                            Reset
                        </button>
                    </div>
                </div>
            </div>

            {/* doctor table */}
            <table className="table table-striped table-hover shadow">
                <thead className="table-primary">
                    <tr>
                        <th>Name</th>
                        <th>Surname</th>
                        <th>Email</th>
                        <th>Phone</th>
                        <th>Facility</th>
                        <th>Hours</th>
                        <th>Specializations</th>
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
                            <td>{doc.specializations.join(", ")}</td>
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

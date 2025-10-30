import { useState, useEffect } from "react"
import { getDoctors } from "../../../services/doctorsService"
import { getDoctorsByFilters } from "../../../services/doctorsService"
import { getSpecializationNames } from "../../../services/specializationsService"
import type { DoctorDto, SpecializationsNamesID } from "../../../interfaces/doctors.types";
import { Link } from "react-router-dom";

function DoctorsList() {
    // data
    const [specializations, setSpecializations] = useState<SpecializationsNamesID[]>([]);
    const [doctors, setDoctors] = useState<DoctorDto[]>([]);

    // filters
    const [selectedSpec, setSelectedSpec] = useState("");
    const [surname, setSurname] = useState("");
    const [availableAt, setAvailableAt] = useState("");

    // loading
    const [loadingDoctors, setLoadingDoctors] = useState(true);
    const [loadingSpecs, setLoadingSpecs] = useState(true);

    useEffect(() => {
        getDoctors()
            .then(setDoctors)
            .catch(console.error)
            .finally(() => setLoadingDoctors(false));
    }, []);

    useEffect(() => {
        getSpecializationNames()
            .then(setSpecializations)
            .catch(console.error)
            .finally(() => setLoadingSpecs(false));
    }, []);

    if (loadingDoctors || loadingSpecs) return <p>Loading...</p>;


    const applyFilters = async (
        specId?: string,
        name?: string,
        at?: string,
    ) => {
        setLoadingDoctors(true);
        try {
            const data = await getDoctorsByFilters(
                specId ? parseInt(specId) : undefined,
                name || undefined,
                at || undefined,
            );
            setDoctors(data);
        } catch (err) {
            console.error(err);
        } finally {
            setLoadingDoctors(false);
        }
    };


    if (loadingDoctors || loadingSpecs) return <p>Loading...</p>

    return (
        <div className="container my-5">
            <h2 className="mb-2 text-center fw-bold">Our doctors</h2>
            <p className="text-center text-muted mb-5">
                Find the right specialist and book your appointment with ease
            </p>

            <div className="row">
                <div className="col-md-3">
                    <div
                        className="card shadow-sm border-0 rounded-3 sticky-top"
                        style={{ top: "100px" }}
                    >
                        <div className="card-body">
                            <h5 className="card-title mb-3">Filters</h5>

                            {/* specializations */}
                            <div className="mb-3">
                                <label className="form-label">Specialization</label>
                                <select
                                    className="form-select"
                                    value={selectedSpec}
                                    onChange={(e) => {
                                        setSelectedSpec(e.target.value);
                                        applyFilters(e.target.value, surname, availableAt);
                                    }}
                                >
                                    <option value="">Select specialization</option>
                                    {specializations.map(spec => (
                                        <option key={spec.id} value={spec.id}>
                                            {spec.specializationName}
                                        </option>
                                    ))}
                                </select>

                            </div>

                            {/* Surname */}
                            <div className="mb-3">
                                <label className="form-label">Search by surname</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    value={surname}
                                    onChange={(e) => setSurname(e.target.value)}
                                    placeholder="e.g. Smith"
                                />
                            </div>

                            {/* Hours */}
                            <div className="mb-3">
                                <label className="form-label">Available at</label>
                                <input
                                    type="time"
                                    className="form-control"
                                    value={availableAt}
                                    onChange={(e) => setAvailableAt(e.target.value)}
                                />
                            </div>

                            <button
                                className="btn btn-primary w-100 mb-2"
                                onClick={() => applyFilters(selectedSpec, surname, availableAt)}
                            >
                                Szukaj
                            </button>

                            <button
                                className="btn btn-outline-secondary w-100"
                                onClick={() => {
                                    setSelectedSpec("");
                                    setSurname("");
                                    setAvailableAt("");
                                    applyFilters();
                                }}
                            >
                                Reset
                            </button>
                        </div>
                    </div>
                </div>

                {/* doctors list */}
                <div className="col-md-9">
                    <div className="row">
                        {doctors.map((d) => (
                            <div className="col-md-6 mb-4" key={d.id}>
                                <div className="card h-100 shadow-sm border-0 rounded-3 d-flex flex-column">
                                    <div className="card-body text-center d-flex flex-column">
                                        <h5 className="card-title fw-bold">
                                            Dr. {d.name} {d.surname}
                                        </h5>

                                        <div className="mb-3">
                                            {d.specializations.map((s, i) => (
                                                <span
                                                    key={i}
                                                    className="badge bg-info text-dark me-1"
                                                >
                                                    {s}
                                                </span>
                                            ))}
                                        </div>

                                        {/* work hours */}
                                        <p className="card-text mt-auto">
                                            <small className="text-secondary d-flex flex-column">
                                                <span>Work hours: Mon&ndash;Fri</span>
                                                <span>
                                                    {d.startHour.slice(0, 5)} -{" "}
                                                    {d.endHour.slice(0, 5)}
                                                </span>
                                            </small>
                                        </p>

                                        <Link
                                            to={`/doctorInfo/${d.id}`}
                                            state={{ doctor: d }}
                                            className="btn btn-primary mt-2 w-100"
                                        >
                                            See profile
                                        </Link>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default DoctorsList;

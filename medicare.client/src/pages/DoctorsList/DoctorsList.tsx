import { useState, useEffect } from "react"
import { getDoctors } from "../../services/doctorsService"
import { Link } from "react-router-dom";

function DoctorsList() {
    interface DoctorDto {
        id: number;
        name: string;
        surname: string;
        email: string;
        startHour: string;
        endHour: string;
        specializations: string[];
    }

    const [doctors, setDoctors] = useState<DoctorDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getDoctors().
            then((data) => setDoctors(data)).
            catch(err => console.error(err)).
            finally(() => setLoading(false))
    }, []);

    if (loading) return <p>Loading...</p>
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
                                <select className="form-select">
                                    <option value="">Select the specialization you are looking for</option>
                                    {doctors.flatMap((d) =>
                                        d.specializations.map((spec, i) => (
                                            <option key={`${d.id}-${i}`}>{spec}</option>
                                        ))
                                    )}
                                </select>
                            </div>

                            {/* Surname */}
                            <div className="mb-3">
                                <label className="form-label">Search by name</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    placeholder="e.g. Smith"
                                />
                            </div>

                            {/* Hours */}
                            <div className="mb-3">
                                <label className="form-label">Available from</label>
                                <input type="time" className="form-control" />
                            </div>
                            <div className="mb-3">
                                <label className="form-label">Available until</label>
                                <input type="time" className="form-control" />
                            </div>

                            <button className="btn btn-outline-secondary w-100">
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
                                            to={`/doctors/${d.id}`}
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

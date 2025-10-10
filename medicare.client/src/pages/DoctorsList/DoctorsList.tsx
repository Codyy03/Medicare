import { useState, useEffect } from "react"
import { getDoctors } from "../../services/doctorsService"

function DoctorsList() {
    interface DoctorDto {
        id: number;
        name: string;
        surname: string;
        email: string;
        startHour: string;
        endHour: string;
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
            <h2 className="mb-4 text-center">Our doctors</h2>

            {/* Filtry */}
            <div className="row mb-4">
                <div className="col-md-4">
                    <select className="form-select">
                        <option value="">Wybierz specjalizacjê</option>
                        <option>Kardiolog</option>
                        <option>Dermatolog</option>
                        <option>Ortopeda</option>
                    </select>
                </div>
                <div className="col-md-4">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Szukaj po nazwisku..."
                    />
                </div>
                <div className="col-md-4 text-end">
                    <button className="btn btn-outline-secondary">Resetuj</button>
                </div>
            </div>

            <div className="row">
                {doctors.map((d) => (
                    <div className="col-md-4 mb-4" key={d.id}>
                        <div className="card h-100 shadow-sm">
                            <img
                                src="https://via.placeholder.com/300x200"
                                className="card-img-top"
                                alt={`Dr ${d.name} ${d.surname}`}
                            />
                            <div className="card-body">
                                <h5 className="card-title">
                                    Dr {d.name} {d.surname}
                                </h5>
                                <p className="card-text text-muted">
                                    Specjalizacja: Kardiolog
                                </p>
                                <p className="card-text">
                                    <small>
                                        Work hours: Mon–Fri {d.startHour} – {d.endHour}
                                    </small>
                                </p>
                                <a href={`/doctors/${d.id}`} className="btn btn-primary">
                                    Zobacz profil
                                </a>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default DoctorsList;

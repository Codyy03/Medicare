import { useEffect, useState } from 'react';
import { getPatients } from "../../services/patientsService";

interface Patient {
    id: number;
    name: string;
    surname: string;
    email: string;
    birthday: Date;
}

export default function PatientsPage() {
    const [patients, setPatients] = useState<Patient[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getPatients()
            .then(data => setPatients(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    if (loading) return <p>Loading...</p>;

    return (
        <div>
            <h1>Patients list</h1>
            <ul>
                {patients.map(p => (
                    <li key={p.id}>
                        {p.name} {p.surname} - {p.email} {new Date(p.birthday).toISOString().split("T")[0]}
                    </li>
                ))}
            </ul>
        </div>
    );
}

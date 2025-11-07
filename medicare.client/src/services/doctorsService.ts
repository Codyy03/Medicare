import api from "./api";

export async function getDoctorMe() {
    const res = await api.get("/doctors/me");
    return res.data;
}

export async function getDoctorById(id: number) {
    const res = await api.get(`/doctors/${id}`);
    return res.data;
}

export async function getDoctors() {
    const response = await fetch(`https://localhost:7014/api/doctors`);

    if (!response.ok) {
        throw new Error("Error when downolading doctors");
    }

    return response.json();
}

export async function deleteDoctor(id: number) {
    const response = await fetch(`https://localhost:7014/api/doctors/${id}`, {
        method: "DELETE"
    });

    if (!response.ok) {
        throw new Error(`Failed to delete doctor with id ${id}`);
    }
}

export async function getDoctorsByFilters(
    specializationID?: number,
    surname?: string,
    availableAt?: string
) {
    const params: Record<string, string> = {};

    if (specializationID) params.specializationID = specializationID.toString();
    if (surname) params.surname = surname;
    if (availableAt) params.availableAt = availableAt;

    const res = await api.get("/doctors/by-filter", { params });
    return res.data;
}

export async function getDoctorsBySpecialization(id: number) {
    const res = await api.get(`/doctors/doctorsBySpecialization/${id}`);
    return res.data;
}
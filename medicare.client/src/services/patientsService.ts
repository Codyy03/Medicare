import api from "./api";

export async function getPatients() {
    const res = await api.get("/patients");
    return res.data;
}

export async function getPatientMe() {
    const res = await api.get("/patients/me");
    return res.data;
}


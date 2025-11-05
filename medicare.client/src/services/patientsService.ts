import api from "./api";

export async function getPatients() {
    const res = await api.get("/patients");
    return res.data;
}

export async function getPatientMe() {
    const res = await api.get("/patients/me");
    return res.data;
}

export async function getAdminPatients() {
    const res = await api.get("/AdminPatients");
    return res.data;
}

export async function getAdminPatientById(id: number) {
    const res = await api.get(`/AdminPatients/${id}`);
    return res.data;
}

export async function createAdminPatient(dto: any) {
    const res = await api.post("/AdminPatients", dto, {
        headers: {
            "Content-Type": "application/json"
            }
        });
    return res.data;
}

export async function updateAdminPatient(id: number, dto: any) {
    const res = await api.put(`/AdminPatients/${id}`, dto);
    return res.data;
}

export async function deleteAdminPatient(id: number) {
    const res = await api.delete(`/AdminPatients/${id}`);
    return res.data;
}


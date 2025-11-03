import type { Specialization } from "../interfaces/specialization.types";

function authHeader(): Record<string, string> {
    const token = localStorage.getItem("token");

    return token ? { Authorization: `Bearer ${token}` } : {};
}

export async function getSpecializations() {
    const response = await fetch("https://localhost:7014/api/specializations/highlights");

    if (!response.ok) throw new Error("Error when downloading specializations");

    return response.json();
}

export async function getSpecializationNames() {
    const response = await fetch("https://localhost:7014/api/specializations/specializationsNames");

    if (!response.ok) throw new Error("Error when downloading specializations names");

    return response.json();
}

export async function getAdminSpecializations() {
    const response = await fetch("https://localhost:7014/api/admin/specializations", {

        headers: { "Content-Type": "application/json", ...authHeader() }

    });

    if (!response.ok) throw new Error("Error when downloading admin specializations");

    return response.json();
}

export async function getAdminSpecialization(id: number) {
    const response = await fetch(`https://localhost:7014/api/admin/specializations/${id}`, {

        headers: { "Content-Type": "application/json", ...authHeader() }

    });

    if (!response.ok) throw new Error("Error when downloading admin specialization");

    return response.json();
}

export async function createAdminSpecialization(specialization: Omit<Specialization, "id">) {
    const response = await fetch("https://localhost:7014/api/admin/specializations", {

        method: "POST",
        headers: { "Content-Type": "application/json", ...authHeader() },
        body: JSON.stringify(specialization)
    });

    if (!response.ok) throw new Error("Error when creating admin specialization");

    return response.json();
}

export async function updateAdminSpecialization(id: number, specialization: Specialization) {
    const response = await fetch(`https://localhost:7014/api/admin/specializations/${id}`, {

        method: "PUT",
        headers: { "Content-Type": "application/json", ...authHeader() },
        body: JSON.stringify({ ...specialization })

    });
    if (!response.ok) throw new Error("Error when updating admin specialization");

    return;
}

export async function deleteAdminSpecialization(id: number) {
    const response = await fetch(`https://localhost:7014/api/admin/specializations/${id}`, {

        method: "DELETE",
        headers: { ...authHeader() }
    });

    if (!response.ok) throw new Error("Error when deleting admin specialization");

    return;
}

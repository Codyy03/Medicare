import api from "./api";
import type { VisitsUpdateDto } from "../interfaces/visits.types";

export async function getVisitByID(id: number) {
    const res = await api.get(`/visits/${id}`);
    return res.data;
}

// utworzenie nowej wizyty
export async function createVisit(dto: Omit<VisitsUpdateDto, "id">) {
    const res = await api.post("/visits", dto);
    return res.data;
}

// aktualizacja istniej¹cej wizyty
export async function updateVisit(id: number, dto: VisitsUpdateDto) {
    await api.put(`/visits/update/${id}`, dto);
}

export async function getVisitsTime(id: number, date: Date) {
    const dateString = formatDateLocal(date);
    const res = await api.get("/visits/visitsTime", {
        params: { id, date: dateString },
    });
    return res.data;
}

export async function getFreeRoomsForDay(
    specId: number,
    doctorId: number,
    date: string
) {
    const res = await api.get(`/visits/freeRoomsForDay/${specId}`, {
        params: { doctorId, date },
    });
    return res.data as Record<string, RoomDto[]>;
}

export async function getDoctorVisitsToday() {
    const res = await api.get("/visits/visitsToday");
    return res.data;
}

export async function getDoctorVisits() {
    const res = await api.get("/visits/doctor");
    return res.data;
}

export async function getPatientVisits() {
    const res = await api.get("/visits/patient");
    return res.data;
}

export async function getAllVisits() {
    const res = await api.get("/visits");
    return res.data;
}

// helper
function formatDateLocal(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, "0");
    const day = date.getDate().toString().padStart(2, "0");
    return `${year}-${month}-${day}`;
}

interface RoomDto {
    id: number;
    roomType: string;
    roomNumber: number;
}

export async function getVisitsTime(id: number, date: Date) {
    const dateString = formatDateLocal(date);

    const response = await fetch(
        `https://localhost:7014/api/visits/visitsTime?id=${id}&date=${dateString}`
    );

    if (!response.ok) {
        throw new Error("Error when downloading visits");
    }

    return response.json();
}

export async function getFreeRoomsBySpecialization(specId: number, doctorId: number, date: string, time: string) {
    const response = await fetch(
        `https://localhost:7014/api/visits/roomsBySpecialization/${specId}?doctorId=${doctorId}&date=${date}&time=${time}`
    );
    if (!response.ok) throw new Error("Error when downloading free rooms");
    return response.json();
}
interface RoomDto {
    id: number;
    roomType: string;
    roomNumber: number;
}
export async function getFreeRoomsForDay(specId: number, doctorId: number, date: string) {
    const response = await fetch(
        `https://localhost:7014/api/visits/freeRoomsForDay/${specId}?doctorId=${doctorId}&date=${date}`
    );

    if (!response.ok) {
        throw new Error("Error when downloading free rooms for day");
    }

    return response.json() as Promise<Record<string, RoomDto[]>>;
}


function formatDateLocal(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, "0");
    const day = date.getDate().toString().padStart(2, "0");

    return `${year}-${month}-${day}`;
}

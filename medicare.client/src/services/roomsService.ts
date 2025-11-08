import api from "./api";
import type { RoomsDto } from "../interfaces/rooms.types";
export async function getRooms() {
    return (await api.get("AdminRooms")).data;
}

export async function deleteRoom(id: number) {
    return (await api.delete("AdminRooms/" + id)).data;
}

export async function createAdminRoom(dto: RoomsDto) {
    return await api.post("AdminRooms", dto, {
        headers: {
            "Content-Type": "application/json"
        }
    })
}

export async function getAdminRoomById(id: number) {
    return (await api.get("AdminRooms/" + id)).data;
}

export async function updateAdminRoom(room: RoomsDto) {
    return await api.put("AdminRooms", room);
}
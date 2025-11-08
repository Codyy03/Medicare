import type { SpecializationNameDto } from "./specialization.types";
export interface RoomsDto {
    id?: number;
    roomNumber: number | null;
    roomType: string;
    specializations: SpecializationNameDto[];
}
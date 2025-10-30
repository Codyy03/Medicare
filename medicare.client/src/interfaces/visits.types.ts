export interface VisitsResponseDto {
    id: number;
    visitDate: Date;
    visitTime: string;
    doctorName: string;
    specialization: string;
    patientName: string;
    room: number;
    status: string;
    reason: string;
    additionalNotes?: string;
    prescriptionText?: string;
    visitNotes?: string;
}
export interface TodayVisitsDto {
    id: number;
    visitTime: string;
    patientName: string;
    reason: string;
    room: string;
    specialization: string;
}

export interface SpecializationDto {
    id: number;
    name: string;
}
export interface TodayVisitsResponse {
    visits: TodayVisitsDto[];
    specializations: SpecializationDto[];
}
export interface SpecializationsNamesID {
    id: number;
    specializationName: string;
}
export interface DoctorApointmentsDto {
    id: number;
    name: string;
    surname: string;
    startHour: string;
    endHour: string;
}

export interface RoomDto {
    id: number;
    roomType: string;
    roomNumber: number;
}
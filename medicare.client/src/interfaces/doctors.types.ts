export interface DoctorDto {
    id: number;
    name: string;
    surname: string;
    email: string;
    phoneNumber: string,
    startHour: string;
    endHour: string;
    facility: string;
    doctorDescription: string;
    specializations: string[];
}
export interface DoctorPrifileDto {
    name: string;
    surname: string;
    startHour: string;
    endHour: string;
    email: string;
    phoneNumber: string;
    specializations: string[];
}
export interface SpecializationsNamesID {
    id: number;
    specializationName: string;
}
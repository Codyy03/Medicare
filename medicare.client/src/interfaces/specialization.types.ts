export interface Specialization {
    id: number;
    specializationName: string;
    specializationHighlight?: string | null;
    specializationDescription?: string | null;
}

export interface SpecializationNameDto {
    id: number;
    specializationName: string;
}

export interface SpecializationHighlightDto {
    specializationName: string;
    specializationHighlight?: string | null;
    specializationDescription?: string | null;
}

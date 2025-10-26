export interface LoginDto {
    email: string;
    password: string;
}
export async function login(dto: LoginDto): Promise<{ accessToken: string; refreshToken: string } | null> {
    const response = await fetch("https://localhost:7014/api/Patients/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(dto),
    });

    if (!response.ok) {
        return null;
    }

    const data = await response.json();
    return { accessToken: data.accessToken, refreshToken: data.refreshToken };
}

export async function loginDoctor(dto: LoginDto): Promise<{ accessToken: string; refreshToken: string } | null> {
    const response = await fetch("https://localhost:7014/api/doctors/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(dto),
    });

    if (!response.ok) {
        return null;
    }

    const data = await response.json();

    return { accessToken: data.accessToken, refreshToken: data.refreshToken };
}

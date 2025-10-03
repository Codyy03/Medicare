export interface LoginDto {
    email: string;
    password: string;
}
export async function login(dto: LoginDto): Promise<string | null> {
    const response = await fetch("https://localhost:7014/api/Patients/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(dto),
    });

    if (!response.ok) {
        return null;
    }

    const data = await response.json();

    return data.token;
}
export async function getPatients() {
    const response = await fetch("https://localhost:7014/api/patients");

    if (!response.ok) {
        throw new Error("Error when downolading patients");
    }
    return response.json();
}
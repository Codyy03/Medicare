export async function getPatients() {
    const token = localStorage.getItem("token");

    const response = await fetch("https://localhost:7014/api/patients", {
        headers: {
            "Content-Type": "application/json",
            "Authorization": token ? `Bearer ${token}` : ""
        }
    });

    if (!response.ok) {
        throw new Error("Error when downolading patients");
    }
    return response.json();
}
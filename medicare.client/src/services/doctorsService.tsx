export async function getDoctorMe(){
    const token = localStorage.getItem("token");

    const response = await fetch("https://localhost:7014/api/doctors/me", {
        headers: {
            "Content-Type": "application/json",
            "Authorization": token ? `Bearer ${token}` : ""
        }
    });

    if (!response.ok) {
        throw new Error("Error when downloading doctor");
    }
    return response.json();
}

export async function getDoctors() {
    const response = await fetch("https://localhost:7014/api/doctors");

    if (!response.ok) {
        throw new Error("Error when downloading doctors");
    }
    return response.json();
}
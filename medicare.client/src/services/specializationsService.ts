export async function getSpecializations(){
    const response = await fetch("https://localhost:7014/api/specializations/highlights");

    if (!response.ok) {
        throw new Error("Error when downolading specializations");
    }

    return response.json();
}
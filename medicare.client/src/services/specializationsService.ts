export async function getSpecializations(){
    const responce = await fetch("https://localhost:7014/api/specializations/highlights");

    if (!responce.ok) {
        throw new Error("Error when downolading specializations");
    }

    return responce.json();
}
export async function getNewsByID(id: number) {

    const response = await fetch(`https://localhost:7014/api/news/${id}`);

    if (!response.ok) {
        throw new Error("Error when downolading news");
    }

    return response.json();
}
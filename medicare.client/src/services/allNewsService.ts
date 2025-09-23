export async function getAllNews(sort: "asc" | "desc" = "desc", month?: number, year?: number) {
    let url = `https://localhost:7014/api/news/by-date?sort=${sort}`

    if (month && year) {
        url += `&month=${month}&year=${year}`;
    }

    const response = await fetch(url);

    if (!response.ok) {
        throw new Error("Error when downloading News");
    }

    return response.json();
}
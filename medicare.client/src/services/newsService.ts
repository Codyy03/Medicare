export async function getNews(count = 3) {
    const response = await fetch(`https://localhost:7014/api/News/latest/${count}`);

    if (!response.ok) {
        throw new Error("Error when downloading News");
    }

    return response.json();
}

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

export async function getNewsByID(id: number) {

    const response = await fetch(`https://localhost:7014/api/news/${id}`);

    if (!response.ok) {
        throw new Error("Error when downolading news");
    }

    return response.json();
}
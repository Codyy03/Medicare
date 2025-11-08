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

export async function createAdminNews(news: { title: string; description: string; imageURL?: string; date: string }) {
    const response = await fetch(`https://localhost:7014/api/AdminNews/createNews`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(news),
    });
    if (!response.ok) {
        throw new Error("Error when creating news");
    }
    return; 
}

export async function updateAdminNews(
    id: number,
    news: { title: string; description: string; imageURL?: string; date: string }
) {
    const response = await fetch(`https://localhost:7014/api/AdminNews/update/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(news),
    });
    if (!response.ok) {
        throw new Error("Error when updating news");
    }
    return response.json().catch(() => ({}));
}

export async function deleteAdminNews(id: number) {
    const response = await fetch(`https://localhost:7014/api/News/${id}`, {
        method: "DELETE"
    });
    if (!response.ok) {
        throw new Error("Error when deleting news");
    }
}


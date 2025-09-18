export async function getNews(count = 3) {
    const response = await fetch(`https://localhost:7014/api/News/latest/${count}`);

    if (!response.ok) {
        throw new Error("Error when downloading News");
    }

    return response.json();
}

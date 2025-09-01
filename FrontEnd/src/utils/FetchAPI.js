// fetchAPI.js
export async function fetchAPI(url, options = {}) {
  try {
    const res = await fetch(url, options);
    if (!res.ok) {
      throw new Error(`Failed to fetch ${url}: ${res.status} ${res.statusText}`);
    }
    return await res.json();
  } catch (error) {
    console.error(error);
    return null;
  }
}

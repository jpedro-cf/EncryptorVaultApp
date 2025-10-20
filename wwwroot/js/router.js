import {initUploadModal} from "./upload-files.js"
async function navigate(url) {
    try {
        const response = await fetch(url, { headers: { "X-Requested-With": "XMLHttpRequest" } });
        if (!response.ok) throw new Error("Page not found");
        const html = await response.text();

        document.getElementById("app").innerHTML = html;
        
        initUploadModal()

        history.pushState({}, "", url);

    } catch (err) {
        document.getElementById("app").innerHTML = "<h2>404</h2><p>Page not found.</p>";
    }
}

document.addEventListener("click", (e) => {
    const link = e.target.closest("[data-link]");
    if (link) {
        e.preventDefault();
        navigate(link.href);
    }
});

// Handle back/forward
window.addEventListener("popstate", () => navigate(location.pathname));
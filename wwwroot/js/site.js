// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
import "./upload-files.js"

async function navigate(url) {
    try {
        const response = await fetch(url, { headers: { "X-Requested-With": "XMLHttpRequest" } });
        if (!response.ok) throw new Error("Page not found");
        const html = await response.text();

        document.getElementById("app").innerHTML = html;

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

import {formatFileSize} from "./utils.js";

class ItemComponent extends HTMLElement {
    constructor() {
        super();

        this.id = null;
        this.isEncrypted = true;
        this.isFolder = false;

        this.encryptedKey = null;
        this.keyEncryptedByRoot = null;

        this.name = null;
        this.createdAt = null;
        this.parentId = null
    }

    static get observedAttributes() {
        return ["data-content"];
    }

    attributeChangedCallback(name, oldValue, newValue) {
        if (name === "data-content") {
            this.render();
        }
    }

    render() {
        const dataAttr = this.getAttribute("data-content");
        if (!dataAttr) return;

        let data;
        try {
            data = JSON.parse(dataAttr);
        } catch {
            this.innerHTML = `<p class="text-danger">Invalid JSON</p>`;
            return;
        }

        const {id, createdAt, name, encryptedKey, keyEncryptedByRoot} = data

        this.id = id
        this.name = name
        this.encryptedKey = encryptedKey
        this.keyEncryptedByRoot = keyEncryptedByRoot
        this.createdAt = createdAt
        this.parentId = data.parentFolderId ?? data.parentId
        
        this.isFolder = !("storageKey" in data)

        console.log(data)

        const formattedDate = new Date(createdAt).toLocaleString();

        if (this.isFolder) {
            this.innerHTML = `
                <div class="folder-card d-flex align-items-start gap-2 bg-yellow-100 rounded-2 p-2 encrypted">
                    <div class="icon bg-yellow-500 text-yellow-100 rounded-circle d-flex align-items-center justify-content-center">
                        <i data-lucide="folder-closed" width="18" height="18"></i>
                    </div>
                    <div class="overflow-hidden">
                        <span class="title fw-semibold">${name}</span>
                        <span class="date d-block small text-muted">${formattedDate}</span>
                    </div>
                    <i data-lucide="external-link" class="ms-auto text-yellow-900"></i>
                </div>
            `;
        } else {
            this.innerHTML = `
                <div class="file-card d-flex align-items-start gap-2 bg-purple-200 rounded-2 p-2">
                    <div class="icon bg-purple-600 text-purple-100 rounded-circle d-flex align-items-center justify-content-center">
                        <i data-lucide="file-spreadsheet" width="18" height="18"></i>
                    </div>
                    <div class="overflow-hidden">
                        <span class="title fw-semibold">${name}</span>
                        <span class="date d-block small text-muted">${formattedDate}</span>
                    </div>
                    <span class="size ms-auto">${formatFileSize(data.size)}</span>
                </div>
            `;
        }
        if (window.lucide) lucide.createIcons();
    }
    
    

}
customElements.define("item-component", ItemComponent);
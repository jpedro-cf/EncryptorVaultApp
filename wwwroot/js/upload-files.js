import {dragDrop} from "./drag-drop.js";
import {state} from "./state.js";
import {Encryption} from "./encryption.js";
import {fileToUin8Array, formatFileSize, stringToUint8Array, uint8ArrayToBase64} from "./utils.js";

let formState = {files: []}
let modal = document.getElementById('add-file-modal')

document.addEventListener("DOMContentLoaded", () => {
    modal = document.getElementById('add-file-modal')
    const container = modal.querySelector(".files")
    const form = modal.querySelector('form')

    modal.addEventListener("show.bs.modal", (e) => {
        // clear everything
        formState = {files: []}
        const hiddenElement = container.querySelector(".d-none");
        container.innerHTML = '';
        if (hiddenElement) {
            container.appendChild(hiddenElement);
        }
        
        dragDrop(handleAddFiles)
    })
    
    form.addEventListener("submit",(e) => {
        e.preventDefault()
        handleFormSubmit(e)
    })
})

function handleAddFiles(files){
    const newFiles = Array.from(files).map((f) => ({
        id: crypto.randomUUID(),
        data: f
    }))
    
    const container = modal.querySelector(".files")
    const fileInfoItem = container.querySelector(".file-info")
    
    for(const file of newFiles){
        const newItem = fileInfoItem.cloneNode(true)
        
        newItem.classList.remove("d-none")
        newItem.querySelector(".title").textContent = file.data.name
        newItem.querySelector(".size").textContent = formatFileSize(file.data.size)
        
        const closeButton = newItem.querySelector(".close")
        closeButton.setAttribute("data-id", file.id) 
        
        closeButton.addEventListener("click", (e) => {
            const id = closeButton.getAttribute("data-id")
            formState.files = formState.files.filter((f) => f.id !== id)
            container.removeChild(newItem)
        })
        
        container.appendChild(newItem)
        formState.files.push({id: file.id, data: file.data, element: newItem});
    }

}

async function handleFormSubmit(e){
    for(const item of formState.files){
        const file = await encryptFile(item.data)
        
        const res = await fetch("/api/files", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({
                fileName: item.data.name,
                fileSize: file.encryptedFile.combined.length,
                encryptionKey: uint8ArrayToBase64(file.encryptedKey.combined),
                rootEncryptionKey: uint8ArrayToBase64(file.keyEncryptedByRoot.combined)
            }),
            credentials: "include"
        })
        
        let partsUploaded = 0
        
        if(res.ok){
            const {uploadId, key: storageKey, urls} = await res.json()
            const result = await uploadParts(
                uploadId, 
                storageKey, 
                urls,
                file.encryptedFile.combined,
                file.encryptedFile.combined.length,
                (completedPartNumber) => {
                    partsUploaded += 1
                    const percentage = (partsUploaded/urls.length) * 100
                    item.element.querySelector(".loader").style.width = `${percentage}%`
                }
            )
            if(!result.ok){
                return
            }
            
            await completeUpload(uploadId, storageKey, result.uploadedParts)
            item.element.style.display = "none"
        }
    }
}

async function encryptFile(file){
    const rootKey = state.keys["root"]
    const fileKey = Encryption.generateRandomBase64Key()

    const encryptedKey = await Encryption.encrypt(stringToUint8Array(fileKey), rootKey)
    const keyEncryptedByRoot = await Encryption.encrypt(stringToUint8Array(fileKey), rootKey)
    
    
    const encryptedFile = await Encryption.encrypt(await fileToUin8Array(file), fileKey)
    
    return {encryptedKey, keyEncryptedByRoot, encryptedFile}
}

async function uploadParts(uploadId, storageKey, urls, file, fileSize, onUpdate){
    const uploadedParts = [];
    const uploadPromises = [];

    const chunkSize = Math.ceil(fileSize/urls.length)
    let offset = 0;
    
    for (const urlObj of urls) {
        const chunk = file.slice(offset, offset + chunkSize);
        offset += chunkSize;

        const promise = fetch(urlObj.url, {
            method: "PUT",
            body: chunk
        }).then((response) => {
            if (!response.ok) return
            
            const etag = response.headers.get("ETag") || response.headers.get("etag");
            uploadedParts.push({
                partNumber: urlObj.partNumber,
                ETag: etag
            });
            
            onUpdate(urlObj.partNumber)
        });

        uploadPromises.push(promise);
    }
    
    await Promise.all(uploadPromises);
    if(uploadedParts.length < urls.length){
        return {ok: false, uploadedParts: []}
    }
    
    return {ok: true, uploadedParts}
}

async function completeUpload(uploadId, storageKey, uploadedParts){
    const res = await fetch("/api/files/uploads/complete", {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({
            uploadId,
            key: storageKey,
            parts: uploadedParts
        }),
        credentials: "include"
    });
}
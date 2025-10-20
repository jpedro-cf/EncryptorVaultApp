import {dragDrop} from "./drag-drop.js";
import {state} from "./state.js";
import {Encryption} from "./encryption.js";
import {base64ToUint8Array, fileToUint8Array, formatFileSize, stringToUint8Array, uint8ArrayToBase64} from "./utils.js";
import {completeUpload, startUpload, uploadParts} from "./s3.js";

let formState = {files: []}
let modal = document.getElementById('add-file-modal')
let parentId = null

const toasts = {
    "SUCCESS": document.getElementById("file-uploaded-success-toast"),
    "FAILED": document.getElementById("file-uploaded-failed-toast"),
}

function operationResult(success = false, item) {
    const toastEl = toasts[success ? "SUCCESS" : "FAILED"];
    if (!toastEl) return;

    const toast = new bootstrap.Toast(toastEl);

    toast.show();

    item.element.classList.add("d-none")
}

export function initUploadModal(){
    modal = document.getElementById('add-file-modal')

    dragDrop(handleItemsSelect)

    const container = modal.querySelector(".files")
    const form = modal.querySelector('form')
    parentId = modal.querySelector(".submit-button").getAttribute("data-parent-id")

    modal.addEventListener("show.bs.modal", () => cleanUp())

    form.addEventListener("submit",async (e) => {
        e.preventDefault()
        await handleFormSubmit(e)
        cleanUp()
    })

    function cleanUp(){
        formState = {files: []}
        const hiddenElement = container.querySelector(".d-none");
        container.innerHTML = '';
        if (hiddenElement) {
            container.appendChild(hiddenElement);
        }
    }
}

function handleItemsSelect(files){
    const newFiles = Array.from(files).map((f) => ({
        id: crypto.randomUUID(),
        data: f
    }))
    
    const container = modal.querySelector(".files")
    const fileItem = container.querySelector(".file-info")
    
    for (const file of newFiles){
        const newItem = fileItem.cloneNode(true)
        
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
    for (const item of formState.files){
        const fileKey = Encryption.generateRandomBase64Key()
        
        const encryptedData = await encryptFile(item.data, fileKey)
        
        const encryptedFileName = uint8ArrayToBase64(encryptedData.encryptedFileName.combined)
        const encryptedKey = uint8ArrayToBase64(encryptedData.encryptedKey.combined)
        const keyEncryptedByRoot = uint8ArrayToBase64(encryptedData.keyEncryptedByRoot.combined)
        const fileSize = encryptedData.encryptedFile.combined.length;
        
        const initialUpload = await startUpload(
            fileSize,
            encryptedFileName,
            encryptedKey,
            keyEncryptedByRoot
        )
        
        if(!initialUpload.ok){
            operationResult(false, item)
            continue
        }
        
        const { uploadId, key: storageKey, urls} = await initialUpload.json()
        
        let partsUploaded = 0
        
        function updateInterface(completedPartNumber){
            partsUploaded += 1
            const percentage = (partsUploaded/urls.length) * 100
            item.element.querySelector(".loader").style.width = `${percentage}%`
        }
        
        const parts = await uploadParts(
            uploadId,
            storageKey,
            urls,
            // the actual encrypted file
            encryptedData.encryptedFile.combined,
            updateInterface
        )
        
        if(!parts.ok){
            operationResult(false, item)
            continue
        }

        const completed = await completeUpload(uploadId, storageKey, parts.uploadedParts)
        if(!completed){
            operationResult(false, item)
            continue
        }
        
        formState.files = formState.files.filter((f) => f.id !== item.id)
        operationResult(true, item)
    }
}

async function encryptFile(file, fileKey){
    const rootKey = state.keys["root"]
    const parentEncryptionKey = parentId ? state.keys.folders[parentId] : rootKey
    
    if(!parentEncryptionKey){
        throw new Error("Encryption key not found.")
    }

    const encryptedKey = await Encryption.encrypt(base64ToUint8Array(fileKey), parentEncryptionKey)
    const keyEncryptedByRoot = await Encryption.encrypt(base64ToUint8Array(fileKey), rootKey)
    const encryptedFileName = await Encryption.encrypt(stringToUint8Array(file.name), fileKey)

    const encryptedFile = await Encryption.encrypt(await fileToUint8Array(file), fileKey)

    return {encryptedKey, keyEncryptedByRoot, encryptedFile, encryptedFileName}
}


document.addEventListener("DOMContentLoaded", () => initUploadModal())
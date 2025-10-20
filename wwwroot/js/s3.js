export async function startUpload(fileSize, encryptedFileNameBase64, encryptedKeyBase64, keyEncryptedByRootBase64) {
    return await fetch("/api/files", {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({
            fileName: encryptedFileNameBase64,
            fileSize,
            encryptedKey: encryptedKeyBase64,
            keyEncryptedByRoot: keyEncryptedByRootBase64
        }),
        credentials: "include"
    })
}

export async function uploadParts(uploadId, storageKey, urls, encryptedFile, onUpdate){
    const uploadedParts = [];
    const uploadPromises = [];

    const fileSize = encryptedFile.length // Uint8Array.length()
    const chunkSize = Math.ceil(fileSize/urls.length)

    let offset = 0;
    for (const urlObj of urls) {
        const chunk = encryptedFile.slice(offset, offset + chunkSize);
        offset += chunkSize;

        const partUploadPromise = fetch(urlObj.url, {
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

        uploadPromises.push(partUploadPromise);
    }

    await Promise.all(uploadPromises);
    if(uploadedParts.length < urls.length){
        return {ok: false, uploadedParts: []}
    }

    return {ok: true, uploadedParts}
}

export async function completeUpload(uploadId, storageKey, uploadedParts){
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

    return res.ok
}

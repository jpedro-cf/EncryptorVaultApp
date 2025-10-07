export function formatFileSize(bytes, decimalPoint = 2) {
    if (bytes === 0) {
        return '0 Bytes'
    }

    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))

    return (
        parseFloat((bytes / Math.pow(k, i)).toFixed(decimalPoint)) +
        ' ' +
        sizes[i]
    )
}

export function stringToUint8Array(text){
    const encoder = new TextEncoder();
    return encoder.encode(text);
}

export function stringToBase64(string){
    return btoa(string)
}

export function uint8ArrayToBase64(uint8array){
    const binaryString = String.fromCharCode(...uint8array);
    return btoa(binaryString)
}

export function base64ToUint8Array(base64String){
    const binaryString = atob(base64String);

    const keyData = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        keyData[i] = binaryString.charCodeAt(i);
    }
    
    return keyData
}

export async function fileToUin8Array(file){
    const arrayBuffer = await file.arrayBuffer();
    return new Uint8Array(arrayBuffer);
}
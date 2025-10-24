import { clsx, type ClassValue } from 'clsx'
import { twMerge } from 'tailwind-merge'

export function cn(...inputs: ClassValue[]) {
    return twMerge(clsx(inputs))
}

export function uint8ArrayToBase64(data: Uint8Array): string {
    let binaryString = ''
    for (let i = 0; i < data.length; i++) {
        binaryString += String.fromCharCode(data[i])
    }
    return btoa(binaryString)
}

export function base64ToUint8Array(
    base64String: string
): Uint8Array<ArrayBuffer> {
    const binaryString = atob(base64String)
    const uint8Array = new Uint8Array(binaryString.length)
    for (let i = 0; i < binaryString.length; i++) {
        uint8Array[i] = binaryString.charCodeAt(i)
    }

    return uint8Array
}

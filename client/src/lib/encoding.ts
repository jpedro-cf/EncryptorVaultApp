export class Encoding {
    public static uint8ArrayToBase64(data: Uint8Array): string {
        let binaryString = ''
        for (let i = 0; i < data.length; i++) {
            binaryString += String.fromCharCode(data[i])
        }
        return btoa(binaryString)
    }

    public static base64ToUint8Array(
        base64String: string
    ): Uint8Array<ArrayBuffer> {
        const binaryString = atob(base64String)
        const uint8Array = new Uint8Array(binaryString.length)
        for (let i = 0; i < binaryString.length; i++) {
            uint8Array[i] = binaryString.charCodeAt(i)
        }

        return uint8Array
    }

    public static textToUint8Array(text: string): Uint8Array<ArrayBuffer> {
        const encoder = new TextEncoder()
        return encoder.encode(text)
    }

    public static uint8ArrayToText(
        uint8Array: Uint8Array<ArrayBuffer>
    ): string {
        const decoder = new TextDecoder()
        return decoder.decode(uint8Array)
    }
}

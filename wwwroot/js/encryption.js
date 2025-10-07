import {base64ToUint8Array, stringToUint8Array, uint8ArrayToBase64} from "./utils.js";

export class Encryption {
    /**
     * @param {ArrayBuffer | Uint8Array} data - Data to encrypt
     * @param {string} base64Key - Base64 Encryption key
     * @returns {Promise<{iv: Uint8Array<ArrayBuffer>, encryptedData: Uint8Array<ArrayBuffer>, combined: Uint8Array<ArrayBuffer>}>} - Encryption Result
     */
    static async encrypt(data, base64Key){
        if (!(data instanceof ArrayBuffer || ArrayBuffer.isView(data))) {
            throw new TypeError("Data must be ArrayBuffer or TypedArray (e.g: Uint8Array)");
        }
        const iv = crypto.getRandomValues(new Uint8Array(16));

        const encryptedData = await crypto.subtle.encrypt(
            {
                name: "AES-GCM",
                iv: iv,
            },
            await this.importKeyFromBase64(base64Key),
            data
        );

        const combined = new Uint8Array(iv.length + encryptedData.byteLength);
        combined.set(iv, 0)
        combined.set(new Uint8Array(encryptedData), iv.length)
        
        return {
            iv: iv, 
            encryptedData: new Uint8Array(encryptedData), 
            combined: combined
        };
    }

    /**
     * @param {Uint8Array} iv - Iv
     * @param {ArrayBuffer | Uint8Array} encryptedData - Encrypted Data
     * @param {string} base64Key - Base64 Encryption key
     * @returns {Promise<Uint8Array>} - Decrypted data
     */
    static async decrypt(iv, encryptedData, base64Key){
        if (!(encryptedData instanceof ArrayBuffer || ArrayBuffer.isView(encryptedData))) {
            throw new TypeError("Data must be ArrayBuffer or TypedArray (e.g: Uint8Array)");
        }
        if (!(iv instanceof Uint8Array)) {
            throw new TypeError("'Iv' must be of type Uint8Array");
        }
        
        const decryptedData = await window.crypto.subtle.decrypt(
            {
                name: "AES-GCM",
                iv: iv,
            },
            await this.importKeyFromBase64(base64Key),
            encryptedData
        );

        return new Uint8Array(decryptedData);
    }

    /**
     * @param {string} secret - Base64 encoded secret
     * @returns {Promise<{salt: string, key: string}>} - Salt & Key
     */
    static async deriveBase64KeyFromSecret(secret){
        const salt = crypto.getRandomValues(new Uint8Array(16));

        const keyMaterial = await crypto.subtle.importKey(
            'raw',
            stringToUint8Array(secret),
            { name: 'PBKDF2' },
            false,
            ['deriveBits', 'deriveKey']
        );

        const derivedKey = await crypto.subtle.deriveBits(
            {
                name: 'PBKDF2',
                salt: salt,
                iterations: 100000,
                hash: 'SHA-256',
            },
            keyMaterial,
            256
        );

        return {salt: uint8ArrayToBase64(salt), key: uint8ArrayToBase64(new Uint8Array(derivedKey))};
    }

    /**
     * @param {string} salt - Base64 encoded salt
     * @param {string} secret - Base64 encoded secret     
     * @returns {Promise<string>} - Salt & Key
     */
    static async deriveBase64KeyFromSecret(salt, secret){
        const saltUint8Array = base64ToUint8Array(salt)
        
        const keyMaterial = await crypto.subtle.importKey(
            'raw',
            stringToUint8Array(secret),
            { name: 'PBKDF2' },
            false,
            ['deriveBits', 'deriveKey']
        );

        const derivedKey = await crypto.subtle.deriveBits(
            {
                name: 'PBKDF2',
                salt: saltUint8Array,
                iterations: 100000,
                hash: 'SHA-256',
            },
            keyMaterial,
            256
        );
        
        return uint8ArrayToBase64(new Uint8Array(derivedKey))
    }
    
    static generateRandomBase64Key(){
        const key = crypto.getRandomValues(new Uint8Array(32));
        return uint8ArrayToBase64(key)
    }
    
    static async importKeyFromBase64(base64Key){
        const keyData = base64ToUint8Array(base64Key)
        
        return await crypto.subtle.importKey(
            "raw",
            keyData,
            {name: "AES-GCM"},
            false,
            ["encrypt", "decrypt"]
        );
    }
}
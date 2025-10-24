import { base64ToUint8Array } from '@/lib/utils'

interface EncryptData {
    data: Uint8Array<ArrayBuffer>
    key: Uint8Array<ArrayBuffer>
}

interface EncryptionResult {
    iv: Uint8Array<ArrayBuffer>
    encryptedData: Uint8Array<ArrayBuffer>
    combined: Uint8Array<ArrayBuffer>
}

interface DecryptData {
    iv: Uint8Array<ArrayBuffer>
    encryptedData: Uint8Array<ArrayBuffer>
    key: Uint8Array<ArrayBuffer>
}

interface DerivedKey {
    salt: Uint8Array<ArrayBuffer>
    key: Uint8Array<ArrayBuffer>
}

interface DecryptVaultKey {
    base64Secret: string
    base64Key: string // salt + iv + data
}

export class Encryption {
    public static async encrypt(
        params: EncryptData
    ): Promise<EncryptionResult> {
        const iv = this.generateRandomSecret(12)

        const encryptedData = await crypto.subtle.encrypt(
            {
                name: 'AES-GCM',
                iv: iv,
            },
            await this.importKey(params.key),
            params.data
        )

        const combined = new Uint8Array(iv.length + encryptedData.byteLength)
        combined.set(iv, 0)
        combined.set(new Uint8Array(encryptedData), iv.length)

        return {
            iv: iv,
            encryptedData: new Uint8Array(encryptedData),
            combined: combined,
        }
    }

    public static async decrypt(
        params: DecryptData
    ): Promise<Uint8Array<ArrayBuffer>> {
        const decryptedData = await window.crypto.subtle.decrypt(
            {
                name: 'AES-GCM',
                iv: params.iv,
            },
            await this.importKey(params.key),
            params.encryptedData
        )

        return new Uint8Array(decryptedData)
    }

    public static async decryptVaultKey(
        params: DecryptVaultKey
    ): Promise<Uint8Array<ArrayBuffer> | null> {
        const secret = base64ToUint8Array(params.base64Secret)
        const combined = base64ToUint8Array(params.base64Key)

        const salt = combined.slice(0, 16)
        const iv = combined.slice(16, 28)
        const encryptedKey = combined.slice(28)

        const { key: decryptionKey } = await this.deriveKeyFromSecret(
            secret,
            salt
        )

        try {
            return await this.decrypt({
                encryptedData: encryptedKey,
                iv,
                key: decryptionKey,
            })
        } catch (e) {
            return null
        }
    }

    static async deriveKeyFromSecret(
        secret: Uint8Array<ArrayBuffer>,
        salt?: Uint8Array<ArrayBuffer>
    ): Promise<DerivedKey> {
        const s = salt ? salt : crypto.getRandomValues(new Uint8Array(16))

        const keyMaterial = await crypto.subtle.importKey(
            'raw',
            secret,
            { name: 'PBKDF2' },
            false,
            ['deriveBits', 'deriveKey']
        )

        const derivedKey = await crypto.subtle.deriveBits(
            {
                name: 'PBKDF2',
                salt: s,
                iterations: 100000,
                hash: 'SHA-256',
            },
            keyMaterial,
            256
        )

        return { salt: s, key: new Uint8Array(derivedKey) }
    }

    public static generateRandomSecret(size?: number): Uint8Array<ArrayBuffer> {
        return window.crypto.getRandomValues(new Uint8Array(size ?? 32))
    }

    private static async importKey(key: Uint8Array<ArrayBuffer>) {
        return await crypto.subtle.importKey(
            'raw',
            key,
            { name: 'AES-GCM' },
            false,
            ['encrypt', 'decrypt']
        )
    }
}

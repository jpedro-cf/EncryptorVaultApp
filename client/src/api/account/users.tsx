import type { VaultSecretSchema } from '@/components/register/RegisterForm'
import { api } from '../axios'
import type { AxiosError } from 'axios'
import { useMutation, useQuery } from '@tanstack/react-query'
import { toast } from 'sonner'
import type { User } from '@/types/account'
import { Encryption } from '@/services/encryption'
import { base64ToUint8Array, uint8ArrayToBase64 } from '@/lib/utils'

export function useCurrentUser() {
    async function request(): Promise<User> {
        const res = await api.get('/users/me')
        const user = res.data

        return user
    }

    return useQuery({
        queryFn: request,
        queryKey: ['account'],
        retry: 0,
        refetchOnWindowFocus: false,
        refetchOnMount: false,
        refetchOnReconnect: false,
    })
}

export function useUpdateVaultSecret() {
    async function request(data: VaultSecretSchema): Promise<void> {
        const rootKeyToEncrypt = Encryption.generateRandomSecret()
        const { salt, key } = await Encryption.deriveKeyFromSecret(
            base64ToUint8Array(data.secret)
        )

        const { iv, encryptedData: encryptedKey } = await Encryption.encrypt({
            key,
            data: rootKeyToEncrypt,
        })

        const combined = new Uint8Array(
            salt.byteLength + iv.byteLength + encryptedKey.byteLength
        )
        combined.set(salt, 0)
        combined.set(iv, salt.byteLength)
        combined.set(encryptedKey, salt.byteLength + iv.byteLength)

        await api.patch('/users/me/vault-key', {
            vaultKey: uint8ArrayToBase64(combined),
        })
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while creating your account.'
            ),
    })
}

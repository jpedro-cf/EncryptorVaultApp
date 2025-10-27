import type { VaultSecretSchema } from '@/components/register/RegisterForm'
import { api } from '../axios'
import type { AxiosError } from 'axios'
import { useMutation, useQuery } from '@tanstack/react-query'
import { toast } from 'sonner'
import type { CurrentUserData, User } from '@/types/account'
import { Encryption } from '@/lib/encryption'
import { Encoding } from '@/lib/encoding'
import { useKeys } from '@/hooks/use-keys'

export function useCurrentUser() {
    async function request(): Promise<CurrentUserData> {
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
    const { rootKey } = useKeys()

    async function request(data: VaultSecretSchema): Promise<void> {
        const rootKeyToEncrypt = rootKey ?? Encryption.generateRandomSecret()

        const { salt, key } = await Encryption.deriveKeyFromSecret(
            Encoding.textToUint8Array(data.secret)
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
            vaultKey: Encoding.uint8ArrayToBase64(combined),
        })
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            ),
    })
}

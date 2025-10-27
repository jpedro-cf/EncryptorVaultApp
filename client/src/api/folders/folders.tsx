import type { Folder, FolderResponse } from '@/types/folders'
import { api } from '../axios'
import { useMutation, useQuery } from '@tanstack/react-query'
import type { FolderItem } from '@/types/items'
import { useKeys } from '@/hooks/use-keys'
import type { FolderFormSchema } from '@/components/folders/FolderForm'
import { Encryption } from '@/lib/encryption'
import { Encoding } from '@/lib/encoding'
import type { AxiosError } from 'axios'
import { toast } from 'sonner'
import { decryptItem } from '../items/items'

interface UseFolderProps {
    folderId: string
    shareId: string | null
    enabled?: boolean
}
export function useFolder({ folderId, shareId, enabled }: UseFolderProps) {
    const { rootKey, folderKeys, setFileKey, setFolderKey } = useKeys()
    const shareKey = window.location.hash

    async function request(): Promise<Folder> {
        const res: FolderResponse = (
            await api.get(`/folders/${folderId}`, {
                headers: {
                    'X-Share-Id': shareId,
                },
            })
        ).data

        const decryptionKey =
            rootKey ?? folderKeys[res.parentId ?? 'none'] ?? null
        const keyToDecrypt =
            rootKey != null ? res.keyEncryptedByRoot : res.encryptedKey

        if (!decryptionKey) {
            throw new Error('Decryption key not found.')
        }

        const folderKey = await Encryption.decrypt({
            encryptedData: Encoding.base64ToUint8Array(keyToDecrypt.data),
            iv: Encoding.base64ToUint8Array(keyToDecrypt.iv),
            key: decryptionKey,
        })

        const encryptedName = res.encryptedName

        const decryptedName = await Encryption.decrypt({
            encryptedData: Encoding.base64ToUint8Array(encryptedName.data),
            iv: Encoding.base64ToUint8Array(encryptedName.iv),
            key: folderKey,
        })

        const decryptedChildrenPromises = res.children.map((item) =>
            decryptItem({
                key: {
                    root: false,
                    value: folderKey,
                },
                item,
            }).then((decryptedItem) => {
                if ('contentType' in decryptedItem) {
                    setFileKey(decryptedItem.id, decryptedItem.key)
                } else {
                    setFolderKey(decryptedItem.id, decryptedItem.key)
                }
                return decryptedItem
            })
        )

        setFolderKey(res.id, folderKey)

        const children = await Promise.all(decryptedChildrenPromises)

        return {
            id: res.id,
            children: children,
            createdAt: res.createdAt,
            key: folderKey,
            name: Encoding.uint8ArrayToText(decryptedName),
            parentId: res.parentId,
        }
    }

    return useQuery({
        queryFn: request,
        queryKey: ['folder', folderId],
        retry: 0,
        enabled: enabled != undefined ? enabled : true,
        refetchOnWindowFocus: false,
        refetchOnMount: false,
        refetchOnReconnect: false,
    })
}

export function useFolderMutation() {
    const { rootKey, folderKeys, setFolderKey } = useKeys()

    async function request(data: FolderFormSchema): Promise<void> {
        const folderEncryptionKey = Encryption.generateRandomSecret()
        const parentEncryptionKey = data.parentId
            ? folderKeys[data.parentId]
            : rootKey

        if (!parentEncryptionKey || !rootKey) {
            throw new Error('The encryption key was not found.')
        }

        const { combined: encryptedName } = await Encryption.encrypt({
            key: folderEncryptionKey,
            data: Encoding.textToUint8Array(data.name),
        })

        const { combined: encryptedKey } = await Encryption.encrypt({
            key: parentEncryptionKey,
            data: folderEncryptionKey,
        })

        const { combined: keyEncryptedByRoot } = await Encryption.encrypt({
            key: rootKey,
            data: folderEncryptionKey,
        })

        const folder: FolderResponse = (
            await api.post('/folders', {
                name: Encoding.uint8ArrayToBase64(encryptedName),
                parentId: data.parentId,
                encryptedKey: Encoding.uint8ArrayToBase64(encryptedKey),
                keyEncryptedByRoot:
                    Encoding.uint8ArrayToBase64(keyEncryptedByRoot),
            })
        ).data

        setFolderKey(folder.id, folderEncryptionKey)

        return
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) => {
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            )
        },
    })
}

async function decryptFolder() {}

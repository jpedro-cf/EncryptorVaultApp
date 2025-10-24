import type { FileItem, FolderItem, Item } from '@/types/items'
import { api } from '../axios'
import { useQuery } from '@tanstack/react-query'
import { useKeys } from '@/hooks/use-keys'
import { Encryption } from '@/lib/encryption'
import { Encoding } from '@/lib/encoding'

interface UseItems {
    enabled: boolean
}
export function useItems({ enabled }: UseItems) {
    const { rootKey, setFileKey, setFolderKey } = useKeys()

    async function request(): Promise<(FileItem | FolderItem)[]> {
        if (rootKey == null) {
            throw new Error(
                'You did not provide the app secret to access your vault.'
            )
        }

        const items: Item[] = (await api.get('/items')).data

        const decryptionPromise = items.map((item) =>
            decryptItem({ item, key: { root: true, value: rootKey! } }).then(
                (decryptedItem) => {
                    if ('contentType' in decryptedItem) {
                        setFileKey(decryptedItem.id, decryptedItem.key)
                    } else {
                        setFolderKey(decryptedItem.id, decryptedItem.key)
                    }
                    return decryptedItem
                }
            )
        )

        return await Promise.all(decryptionPromise)
    }

    return useQuery({
        queryFn: request,
        queryKey: ['items'],
        retry: 0,
        enabled,
        refetchOnWindowFocus: false,
        refetchOnMount: false,
        refetchOnReconnect: false,
    })
}

interface DecryptItem {
    key: { root: boolean; value: Uint8Array<ArrayBuffer> }
    item: Item
}
export async function decryptItem(
    params: DecryptItem
): Promise<FileItem | FolderItem> {
    const { item, key } = params
    const { iv, data } =
        key.root && item.keyEncryptedByRoot
            ? item.keyEncryptedByRoot
            : item.encryptedKey

    const decryptedKey = await Encryption.decrypt({
        encryptedData: Encoding.base64ToUint8Array(data),
        iv: Encoding.base64ToUint8Array(iv),
        key: key.value,
    })

    const decryptedName = await Encryption.decrypt({
        encryptedData: Encoding.base64ToUint8Array(data),
        iv: Encoding.base64ToUint8Array(iv),
        key: decryptedKey,
    })

    if (item.type == 'file') {
        return {
            id: item.id,
            name: Encoding.uint8ArrayToText(decryptedName),
            size: item.size!,
            contentType: item.contentType!,
            url: item.url!,
            createdAt: item.createdAt,
            key: decryptedKey,
        }
    }

    return {
        id: item.id,
        name: Encoding.uint8ArrayToText(decryptedName),
        createdAt: item.createdAt,
        key: decryptedKey,
    }
}

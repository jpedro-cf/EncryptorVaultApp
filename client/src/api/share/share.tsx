import { useKeys } from '@/hooks/use-keys'
import { Encryption } from '@/lib/encryption'
import type {
    FileItem,
    FolderItem,
    ItemResponse,
    ItemType,
} from '@/types/items'
import { api } from '../axios'
import { Encoding } from '@/lib/encoding'
import type { SharedContentResponse, SharedItemResponse } from '@/types/share'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import type { AxiosError } from 'axios'
import { decryptItem } from '../items/items'
import type { Folder } from '@/types/folders'

interface CreateSharedLink {
    itemId: string
    itemType: ItemType
}

interface UseSharedLink {
    shareId: string
    enabled?: boolean
}

export function useCreateSharedLink() {
    const queryClient = useQueryClient()

    const { folderKeys, fileKeys } = useKeys()

    async function request(data: CreateSharedLink) {
        if (data.itemType == 'File') {
            return await handleFile(data)
        }
        return await handleFolder(data)
    }

    async function handleFile(data: CreateSharedLink) {
        const encryptionKey = Encryption.generateRandomSecret()
        const fileKey = fileKeys[data.itemId]

        if (!fileKey) {
            throw new Error('Encryption key for file not found.')
        }

        const { combined: encryptedKey } = await Encryption.encrypt({
            data: fileKey,
            key: encryptionKey,
        })

        const sharedItem: SharedItemResponse = (
            await api.post('/shared-links', {
                ...data,
                encryptedKey: Encoding.uint8ArrayToBase64(encryptedKey),
            })
        ).data

        return { item: sharedItem, id: sharedItem.id, key: encryptionKey }
    }

    async function handleFolder(data: CreateSharedLink) {
        const encryptionKey = Encryption.generateRandomSecret()

        const folderKey = folderKeys[data.itemId]

        if (!folderKey) {
            throw new Error('Encryption key for folder not found.')
        }

        const { combined: encryptedKey } = await Encryption.encrypt({
            data: folderKey,
            key: encryptionKey,
        })

        const sharedItem: SharedItemResponse = (
            await api.post('/shared-links', {
                ...data,
                encryptedKey: Encoding.uint8ArrayToBase64(encryptedKey),
            })
        ).data

        return { item: sharedItem, id: sharedItem.id, key: encryptionKey }
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            ),
        onSuccess: (data) => {
            const links: SharedItemResponse[] =
                queryClient.getQueryData(['shared-links']) ?? []

            queryClient.setQueryData(['shared-links'], [data.item, ...links])
        },
    })
}

export function useSharedLink({ shareId, enabled }: UseSharedLink) {
    const { setFileKey, setFolderKey } = useKeys()

    async function request(): Promise<(FileItem | FolderItem)[]> {
        const hash = window.location.hash.slice(1)
        if (hash.length <= 0) {
            throw new Error('Key not provided.')
        }

        const decryptionKey = Encoding.decodeUrlSafeBase64(hash)

        const { items, keyToDecryptItems, shareType }: SharedContentResponse = (
            await api.get(`/share/${shareId}`)
        ).data

        const itemsKey = await Encryption.decrypt({
            encryptedData: Encoding.base64ToUint8Array(keyToDecryptItems.data),
            iv: Encoding.base64ToUint8Array(keyToDecryptItems.iv),
            key: decryptionKey,
        })

        const decryptionPromise = items.map((item) => {
            if (shareType == 'File') {
                return decryptSharedItem(itemsKey, item)
            }
            return decryptItem({
                key: { root: false, value: itemsKey },
                item,
            })
        })

        if (shareType == 'File') {
            setFileKey(items[0].id, itemsKey)
        } else {
            setFolderKey(items[0].parentId!, itemsKey)
        }

        return await Promise.all(decryptionPromise)
    }

    return useQuery({
        queryFn: request,
        queryKey: ['shared-link'],
        retry: 0,
        enabled: enabled != undefined ? enabled : true,
        refetchOnWindowFocus: false,
        refetchOnMount: false,
        refetchOnReconnect: false,
    })
}

export function useSharedLinks() {
    async function request(): Promise<SharedItemResponse[]> {
        return (await api.get('/shared-links')).data
    }

    return useQuery({
        queryKey: ['shared-links'],
        queryFn: request,
        retry: 0,
        refetchOnWindowFocus: false,
        refetchOnMount: false,
        refetchOnReconnect: false,
    })
}

export function useDeleteSharedLink() {
    const queryClient = useQueryClient()

    async function request(id: string): Promise<void> {
        await api.delete(`/shared-links/${id}`)
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            ),
        onSuccess: (_, variables) => {
            const links: SharedItemResponse[] =
                queryClient.getQueryData(['shared-links']) ?? []

            const id = variables

            queryClient.setQueryData(
                ['shared-links'],
                links.filter((l) => l.id !== id)
            )
        },
    })
}

async function decryptSharedItem(
    key: Uint8Array<ArrayBuffer>,
    item: ItemResponse
): Promise<FileItem | FolderItem> {
    const decryptedName = await Encryption.decrypt({
        encryptedData: Encoding.base64ToUint8Array(item.encryptedName.data),
        iv: Encoding.base64ToUint8Array(item.encryptedName.iv),
        key,
    })

    if (item.type == 'File') {
        return {
            id: item.id,
            name: Encoding.uint8ArrayToText(decryptedName),
            size: item.size!,
            contentType: item.contentType!,
            createdAt: item.createdAt,
            parentId: item.parentId,
            key: key,
        }
    }

    return {
        id: item.id,
        name: Encoding.uint8ArrayToText(decryptedName),
        createdAt: item.createdAt,
        parentId: item.parentId,
        key: key,
    }
}

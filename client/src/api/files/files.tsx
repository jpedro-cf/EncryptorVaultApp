import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../axios'
import { AxiosError } from 'axios'
import { toast } from 'sonner'
import type { FileItem, FolderItem } from '@/types/items'
import { useAuth } from '@/hooks/use-auth'
import type { Folder } from '@/types/folders'
import type { FileResponse } from '@/types/files'
import { useKeys } from '@/hooks/use-keys'
import { Encryption } from '@/lib/encryption'

export function useFileDeletion() {
    const queryClient = useQueryClient()
    const { updateStorageUsage } = useAuth()

    async function request(file: FileItem) {
        await api.delete(`/files/${file.id}`)
        updateStorageUsage(file.contentType, -file.size)
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            ),
        onSuccess: (_, variables) => {
            const file = variables

            const queryKey = file.parentId
                ? ['folder', { id: file.parentId }]
                : ['items']

            const previous = queryClient.getQueryData(queryKey)

            if (!file.parentId) {
                const previousItems = previous as (FolderItem | FileItem)[]
                queryClient.setQueryData(
                    queryKey,
                    previousItems.filter((d) => d.id != file.id)
                )
            } else {
                const previousFolder = previous as Folder
                queryClient.setQueryData(queryKey, {
                    ...previousFolder,
                    children: previousFolder.children.filter(
                        (d) => d.id != file.id
                    ),
                })
            }
        },
    })
}

interface UseFile {
    enabled: boolean
    fileId: string
    sharedLinkId?: string
}
export function useFile({ enabled, fileId, sharedLinkId }: UseFile) {
    const { fileKeys } = useKeys()

    async function request() {
        const key = fileKeys[fileId]

        if (!key) {
            throw new Error('File key not found')
        }

        const res: FileResponse = (
            await api.get(`/files/${fileId}`, {
                headers: {
                    'X-Share-Id': sharedLinkId,
                },
            })
        ).data

        const download = await fetch(res.url)

        const encryptedFile = await download.arrayBuffer()

        const iv = encryptedFile.slice(0, 12)
        const encryptedData = encryptedFile.slice(12)

        const decryptionResult = await Encryption.decrypt({
            iv: new Uint8Array(iv),
            encryptedData: new Uint8Array(encryptedData),
            key,
        })

        return {
            id: res.id,
            fileContent: decryptionResult.buffer,
            contentType: res.contentType,
        }
    }

    return useQuery({
        queryKey: ['file', { id: fileId }],
        queryFn: request,
        enabled: enabled,
        retry: 1,
        refetchOnMount: false,
        refetchOnWindowFocus: false,
        refetchOnReconnect: false,
    })
}

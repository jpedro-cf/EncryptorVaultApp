import { useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '../axios'
import type { AxiosError } from 'axios'
import { toast } from 'sonner'
import type { FileItem, FolderItem } from '@/types/items'
import { useAuth } from '@/hooks/use-auth'
import type { Folder } from '@/types/folders'

export function useFileDeletion() {
    const queryClient = useQueryClient()
    const { updateStorageUsage } = useAuth()

    async function request({ file }: { file: FileItem }) {
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
            const { file } = variables

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

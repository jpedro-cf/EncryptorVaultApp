import type { Folder } from '@/types/folders'
import type { FileItem, FolderItem } from '@/types/items'
import { createContext, useContext } from 'react'

type ExplorerContext = {
    folderTree: Folder[]
    pushFolder: (folder: Folder) => void

    currentFolderId: string | null
    setCurrentFolderId: (id: string | null) => void

    currentFile: FileItem | null
    setCurrentFile: (file: FileItem | null) => void

    items: (FileItem | FolderItem)[]

    shareId: string | null
}

export const ExplorerContext = createContext<ExplorerContext | null>(null)

export function useExplorerContext() {
    const context = useContext(ExplorerContext)
    if (context == null) {
        throw new Error('Explorer context must be used within a Provider')
    }
    return context
}

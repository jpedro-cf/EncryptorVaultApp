import type { EncryptedData, FileItem, FolderItem, ItemResponse } from './items'

export interface FolderResponse {
    id: string
    encryptedName: EncryptedData
    encryptedKey: EncryptedData
    keyEncryptedByRoot: EncryptedData
    parentId?: string
    children: ItemResponse[]
    createdAt: Date
}

export interface Folder {
    id: string
    name: string
    key: Uint8Array<ArrayBuffer>
    parentId?: string
    children: (FileItem | FolderItem)[]
    createdAt: Date
}

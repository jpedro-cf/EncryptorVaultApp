export interface ItemResponse {
    id: string
    type: ItemType
    encryptedName: EncryptedData
    size?: number
    contentType?: ContentType
    url?: string
    createdAt: Date
    parentId?: string
    encryptedKey: EncryptedData
    keyEncryptedByRoot?: EncryptedData
}

export interface FileItem {
    id: string
    name: string
    size: number
    contentType: ContentType
    url: string
    createdAt: Date
    key: Uint8Array<ArrayBuffer>
}

export interface FolderItem {
    id: string
    name: string
    parentId?: string
    createdAt: Date
    key: Uint8Array<ArrayBuffer>
}

export type ItemType = 'folder' | 'file'

export type ContentType = 'application' | 'text' | 'image' | 'video' | 'audio'

export interface EncryptedData {
    iv: string
    data: string
}

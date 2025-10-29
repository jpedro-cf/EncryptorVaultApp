export interface ItemResponse {
    id: string
    type: ItemType
    encryptedName: EncryptedData
    size?: number
    contentType?: ContentType
    createdAt: Date
    parentId?: string
    encryptedKey: EncryptedData
    keyEncryptedByRoot?: EncryptedData
}

export interface FileItem {
    id: string
    name: string
    size: number
    parentId?: string
    contentType: ContentType
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

export type ItemType = 'Folder' | 'File'

export type ContentType = 'Application' | 'Text' | 'Image' | 'Video' | 'Audio'

export interface EncryptedData {
    iv: string
    data: string
}

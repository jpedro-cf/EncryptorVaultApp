import type { EncryptedData, ItemResponse, ItemType } from './items'

export interface SharedItemResponse {
    id: string
    itemId: string
    type: ItemType
    encryptedKey: EncryptedData
    createdAt: Date
}

export interface SharedContentResponse {
    items: ItemResponse[]
    shareType: ItemType
    keyToDecryptItems: EncryptedData
}

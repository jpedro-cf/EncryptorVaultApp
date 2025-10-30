import type { EncryptedData, ItemResponse, ItemType } from './items'

export interface SharedLinkResponse {
    id: string
    itemId: string
    type: ItemType
    encryptedKey: EncryptedData
    createdAt: Date
}

export interface SharedContentResponse {
    items: ItemResponse[]
    itemType: ItemType
    keyToDecryptItems: EncryptedData
}

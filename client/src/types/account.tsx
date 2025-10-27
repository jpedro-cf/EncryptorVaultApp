import type { ContentType } from './items'

export interface User {
    id: string
    email: string
    vaultKey: string
}

export type StorageUsageSummary = Record<ContentType, number>

export interface CurrentUserData {
    user: User
    storageUsage: StorageUsageSummary
}

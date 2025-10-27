import type { StorageUsageSummary, User } from '@/types/account'
import type { ContentType } from '@/types/items'
import { create } from 'zustand'

interface AuthState {
    account: User | null
    setAccount: (data: User | null) => void

    storageUsage: StorageUsageSummary
    setStorageUsage: (data: StorageUsageSummary) => void
    updateStorageUsage: (contentType: ContentType, value: number) => void
}

export const useAuth = create<AuthState>()((set) => ({
    account: null,
    setAccount: (data) => set((state) => ({ account: data })),

    storageUsage: {
        Application: 0,
        Audio: 0,
        Image: 0,
        Text: 0,
        Video: 0,
    },
    setStorageUsage: (data) => set((state) => ({ storageUsage: data })),
    updateStorageUsage: (contentType, value) =>
        set((state) => {
            return {
                storageUsage: {
                    ...state.storageUsage,
                    [contentType]: state.storageUsage[contentType] + value,
                },
            }
        }),
}))

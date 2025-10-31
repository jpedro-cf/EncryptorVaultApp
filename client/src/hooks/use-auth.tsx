import type { StorageUsageSummary, User } from '@/types/account'
import type { ContentType } from '@/types/items'
import { create } from 'zustand'

const defaultStorageData = {
    Application: 0,
    Audio: 0,
    Image: 0,
    Text: 0,
    Video: 0,
}

interface AuthState {
    account: User | null
    setAccount: (data: User | null) => void

    storageUsage: StorageUsageSummary | null
    setStorageUsage: (data: StorageUsageSummary | null) => void
    updateStorageUsage: (contentType: ContentType, value: number) => void

    clear: () => void
}

export const useAuth = create<AuthState>()((set) => ({
    account: null,
    setAccount: (data) => set(() => ({ account: data })),

    storageUsage: defaultStorageData,
    setStorageUsage: (data) => set(() => ({ storageUsage: data })),
    updateStorageUsage: (contentType, delta) =>
        set((state) => ({
            storageUsage: {
                ...state.storageUsage!,
                [contentType]: (state.storageUsage![contentType] ?? 0) + delta,
            },
        })),

    clear: () =>
        set(() => ({ account: null, storageUsage: defaultStorageData })),
}))

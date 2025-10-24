import { create } from 'zustand'

interface KeyState {
    rootKey: Uint8Array<ArrayBuffer> | null
    folderKeys: Record<string, Uint8Array<ArrayBuffer>>
    fileKeys: Record<string, Uint8Array<ArrayBuffer>>
    setRootKey: (key: Uint8Array<ArrayBuffer> | null) => void
    setFileKey: (id: string, key: Uint8Array<ArrayBuffer>) => void
    setFolderKey: (id: string, key: Uint8Array<ArrayBuffer>) => void
}

export const useAuth = create<KeyState>()((set) => ({
    rootKey: null,
    folderKeys: {},
    fileKeys: {},
    setRootKey: (key) =>
        set(() => ({
            rootKey: key,
        })),
    setFileKey: (id, key) =>
        set((state) => ({
            fileKeys: {
                ...state.fileKeys,
                [id]: key,
            },
        })),

    setFolderKey: (id, key) =>
        set((state) => ({
            folderKeys: {
                ...state.folderKeys,
                [id]: key,
            },
        })),
}))

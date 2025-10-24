import type { User } from '@/types/account'
import { create } from 'zustand'

interface AuthState {
    account: User | null
    setAccount: (data: User | null) => void
}

export const useAuth = create<AuthState>()((set) => ({
    account: null,
    setAccount: (data) => set((state) => ({ account: data })),
}))

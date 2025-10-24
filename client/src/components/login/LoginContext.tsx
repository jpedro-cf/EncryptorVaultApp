import type { LoginStep } from '@/pages/LoginPage'
import { createContext, useContext } from 'react'

type LoginContext = {
    currentStep: LoginStep
    setCurrentStep: (step: LoginStep) => void
}
export const LoginContext = createContext<LoginContext | null>(null)

export function useLoginContext() {
    const context = useContext(LoginContext)
    if (!context) {
        throw new Error('Login context must be used within a LoginProvider')
    }

    return context
}

import type { RegistrationStep } from '@/pages/RegisterPage'
import { createContext, useContext } from 'react'

type RegistrationContext = {
    currentStep: RegistrationStep
    setCurrentStep: (step: RegistrationStep) => void
}
export const RegistrationContext = createContext<RegistrationContext | null>(
    null
)

export function useRegistrationContext() {
    const context = useContext(RegistrationContext)
    if (!context) {
        throw new Error(
            'Registration Context must be used within a RegistrationProvider'
        )
    }

    return context
}

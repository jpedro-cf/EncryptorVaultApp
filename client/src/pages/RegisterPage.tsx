import { RegisterForm } from '@/components/register/RegisterForm'
import { RegistrationContext } from '@/components/register/RegistrationContext'
import { RegistrationSteps } from '@/components/register/RegistrationSteps'
import { Card, CardContent, CardHeader } from '@/components/ui/card'
import { useAuth } from '@/hooks/use-auth'
import { Lock } from 'lucide-react'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router'

const RegistrationStep = {
    ACCOUNT: 1,
    VAULT_SECRET: 2,
    MFA: 3,
} as const

type RegistrationStep = (typeof RegistrationStep)[keyof typeof RegistrationStep]

export { RegistrationStep }

export function RegisterPage() {
    const [currentStep, setCurrentStep] = useState<RegistrationStep>(
        RegistrationStep.ACCOUNT
    )

    return (
        <main className="bg-linear-to-br from-slate-900 via-slate-800 to-slate-900 min-h-screen w-full flex items-center justify-center">
            <div className="w-full max-w-md">
                <Card className="border-slate-700 bg-slate-800 w-full h-full overflow-hidden">
                    <CardHeader>
                        <div className="flex gap-3">
                            <div className="inline-flex items-center justify-center w-12 h-12 rounded-lg bg-blue-600 mb-4">
                                <Lock className="w-6 h-6 text-white" />
                            </div>
                            <div>
                                <h1 className="text-2xl font-bold text-white">
                                    SecureVault
                                </h1>
                                <p className="text-slate-400">
                                    Create your account
                                </p>
                            </div>
                        </div>
                    </CardHeader>
                    <CardContent>
                        <RegistrationContext.Provider
                            value={{ currentStep, setCurrentStep }}
                        >
                            <RegistrationSteps />
                            <RegisterForm />
                        </RegistrationContext.Provider>
                    </CardContent>
                </Card>
            </div>
        </main>
    )
}

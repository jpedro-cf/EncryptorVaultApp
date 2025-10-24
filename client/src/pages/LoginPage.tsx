import { LoginContext } from '@/components/login/LoginContext'
import { LoginForm } from '@/components/login/LoginForm'
import { Card, CardHeader } from '@/components/ui/card'
import { useAuth } from '@/hooks/use-auth'
import { Lock } from 'lucide-react'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router'

const LoginStep = {
    DEFAULT: 1,
    TWO_FACTOR: 2,
} as const

type LoginStep = (typeof LoginStep)[keyof typeof LoginStep]

export { LoginStep }

export function LoginPage() {
    const navigate = useNavigate()
    const { account } = useAuth()

    const [currentStep, setCurrentStep] = useState<LoginStep>(LoginStep.DEFAULT)

    useEffect(() => {
        if (account != null) {
            navigate('/', { replace: true })
        }
    }, [account, navigate])

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
                                    Login to your account
                                </p>
                            </div>
                        </div>
                        <LoginContext.Provider
                            value={{ currentStep, setCurrentStep }}
                        >
                            <LoginForm />
                        </LoginContext.Provider>
                    </CardHeader>
                </Card>
            </div>
        </main>
    )
}

import { cn } from '@/lib/utils'
import { RegistrationStep } from '@/pages/RegisterPage'
import { CheckCircle2 } from 'lucide-react'
import { useRegistrationContext } from './RegistrationContext'

export function RegistrationSteps({ className }: React.ComponentProps<'div'>) {
    const { currentStep } = useRegistrationContext()

    const steps: RegistrationStep[] = [
        RegistrationStep.ACCOUNT,
        RegistrationStep.VAULT_SECRET,
        RegistrationStep.MFA,
    ]
    return (
        <div
            className={cn('flex items-center justify-between mb-4', className)}
        >
            {steps.map((step) => (
                <div key={step} className="flex items-center">
                    <div
                        className={`min-w-8 min-h-8 rounded-full flex items-center justify-center font-semibold text-sm ${
                            step <= currentStep
                                ? 'bg-blue-600 text-white'
                                : 'bg-slate-700 text-slate-400'
                        }`}
                    >
                        {step < currentStep ? (
                            <CheckCircle2 className="w-4 h-4" />
                        ) : (
                            step
                        )}
                    </div>
                    {step < 3 && (
                        <div
                            className={`w-12 h-1 mx-2 ${
                                step < currentStep
                                    ? 'bg-blue-600'
                                    : 'bg-slate-700'
                            }`}
                        />
                    )}
                </div>
            ))}
        </div>
    )
}

import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'
import { useAuth } from '@/hooks/use-auth'
import { MfaForm } from './MfaForm'
import { cn } from '@/lib/utils'
import { ShieldUser } from 'lucide-react'

interface Props {
    onMfaSuccess: () => void
}
export function ProfileMfa({ onMfaSuccess }: Props) {
    const { account } = useAuth()
    return (
        <Card className="border-slate-700 bg-slate-800 w-full overflow-hidden text-slate-100">
            <CardHeader className="gap-0">
                <CardTitle className="text-2xl">
                    Two Factor Authentication
                </CardTitle>
                <CardDescription className="text-slate-300">
                    Setup two factor authentication.
                </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
                <div className="flex items-center justify-between p-4 bg-slate-700/50 rounded-lg border border-slate-600">
                    <div>
                        <p className="font-semibold text-slate-100">Status</p>
                        <p
                            className={cn(
                                'text-sm',
                                account?.twoFactorEnabled
                                    ? 'text-green-400'
                                    : 'text-red-400'
                            )}
                        >
                            {account?.twoFactorEnabled ? 'Enabled' : 'Disabled'}
                        </p>
                    </div>
                    <span
                        className={
                            account?.twoFactorEnabled
                                ? 'text-green-400'
                                : 'text-red-400'
                        }
                    >
                        <ShieldUser />
                    </span>
                </div>
                <MfaForm onSuccess={onMfaSuccess} />
            </CardContent>
        </Card>
    )
}

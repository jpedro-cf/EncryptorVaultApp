import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'
import { ProfileForm } from './ProfileForm'

export function ProfileInformation() {
    return (
        <Card className="border-slate-700 bg-slate-800 w-full overflow-hidden text-slate-100">
            <CardHeader className="gap-0">
                <CardTitle className="text-2xl">Profile Information</CardTitle>
                <CardDescription className="text-slate-300">
                    Manage your account and preferences.
                </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
                <ProfileForm />
            </CardContent>
        </Card>
    )
}

import { ProfileInformation } from '@/components/account/ProfileInformation'
import { ProfileMfa } from '@/components/account/ProfileMfa'
import { useAuth } from '@/hooks/use-auth'
import { useEffect } from 'react'
import { useNavigate } from 'react-router'

export function ProfilePage() {
    const { account, setAccount } = useAuth()
    const navigate = useNavigate()

    function onMfaSuccess() {
        setAccount({ ...account!, twoFactorEnabled: true })
    }

    useEffect(() => {
        if (!account) {
            navigate('/register', { replace: true })
        }
    }, [account, navigate])

    return (
        <div className="flex-1 overflow-auto p-6 relative space-y-6">
            <ProfileInformation />
            <ProfileMfa onMfaSuccess={onMfaSuccess} />
        </div>
    )
}

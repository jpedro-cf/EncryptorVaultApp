import { ProfileInformation } from '@/components/account/ProfileInformation'
import { useAuth } from '@/hooks/use-auth'
import { useEffect } from 'react'
import { useNavigate } from 'react-router'

export function ProfilePage() {
    const { account } = useAuth()
    const navigate = useNavigate()

    useEffect(() => {
        if (!account) {
            navigate('/register', { replace: true })
        }
    }, [account, navigate])

    return (
        <div className="flex-1 overflow-auto p-6 space-y-6 relative">
            <ProfileInformation />
        </div>
    )
}

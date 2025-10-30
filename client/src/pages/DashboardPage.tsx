import { Explorer } from '@/components/dashboard/explorer/Explorer'
import { useAuth } from '@/hooks/use-auth'
import { useEffect } from 'react'
import { useNavigate } from 'react-router'

export function DashboardPage() {
    const navigate = useNavigate()
    const { account } = useAuth()

    useEffect(() => {
        if (!account) {
            navigate('/register', { replace: true })
        }
    }, [account, navigate])

    return (
        <div className="flex-1 overflow-auto p-6 space-y-6 relative">
            <Explorer folderId={null} />
        </div>
    )
}

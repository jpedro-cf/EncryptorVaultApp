import { SharedExplorer } from '@/components/dashboard/explorer/SharedExplorer'
import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router'

export function SharePage() {
    const { id } = useParams()
    const navigate = useNavigate()

    useEffect(() => {
        if (!id) {
            navigate('/', { replace: true })
        }
    }, [id, navigate])

    return (
        <div className="flex-1 overflow-auto p-6 space-y-6 relative">
            <SharedExplorer sharedLinkId={id!} />
        </div>
    )
}

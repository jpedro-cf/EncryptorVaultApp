import { useSharedLinks } from '@/api/share/share'
import { SharedCardSkeleton } from '@/components/shared-links/SharedCardSkeleton'
import { SharedLinkCard } from '@/components/shared-links/SharedLinkCard'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { useAuth } from '@/hooks/use-auth'
import { AlertCircle } from 'lucide-react'
import { useEffect } from 'react'
import { useNavigate } from 'react-router'

export function SharedLinksPage() {
    const { account } = useAuth()
    const navigate = useNavigate()

    const { data, isLoading, isError } = useSharedLinks()

    useEffect(() => {
        if (!account) {
            navigate('/register', { replace: true })
        }
    }, [account, navigate])

    if (isLoading) {
        return (
            <div className="flex-1 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 p-6">
                {[1, 2, 3].map((i) => (
                    <SharedCardSkeleton key={i} />
                ))}
            </div>
        )
    }

    if (isError || !data) {
        return (
            <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2">
                <div className="text-blue-400">
                    <AlertCircle className="h-4 w-4" />
                </div>
                <AlertDescription className="text-blue-300 text-sm">
                    An error occured while fetching your content, try again
                    later or contact us.
                </AlertDescription>
            </Alert>
        )
    }

    return (
        <div className="flex-1 overflow-auto space-y-6 relative">
            <h1 className="text-3xl font-bold text-white mb-1">Shared links</h1>
            <p className="text-slate-400">
                Visualize and manage your shared links
            </p>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-4">
                {data?.map((item) => (
                    <SharedLinkCard key={item.id} sharedLink={item} />
                ))}
            </div>
        </div>
    )
}

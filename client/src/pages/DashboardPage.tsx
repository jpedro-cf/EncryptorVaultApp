import { useItems } from '@/api/items/items'
import { StorageUsage } from '@/components/dashboard/StorageUsage'
import { ItemCardSkeleton } from '@/components/items/ItemCardSkeleton'
import { ItemsGrid } from '@/components/items/ItemsGrid'
import { useAuth } from '@/hooks/use-auth'
import { useEffect } from 'react'
import { useNavigate } from 'react-router'

export function DashboardPage() {
    const navigate = useNavigate()
    const { account } = useAuth()

    const { data, isLoading, isError } = useItems({ enabled: account != null })

    useEffect(() => {
        if (!account) {
            navigate('/register', { replace: true })
        }
    }, [account])

    return (
        <div className="flex h-full">
            <div className="flex-1 overflow-auto p-6 space-y-6">
                {isLoading && (
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                        {[1, 2, 3, 4].map((i) => (
                            <ItemCardSkeleton key={i} />
                        ))}
                    </div>
                )}
                {!isError && <ItemsGrid items={data ?? []} />}
            </div>
            <StorageUsage />
        </div>
    )
}

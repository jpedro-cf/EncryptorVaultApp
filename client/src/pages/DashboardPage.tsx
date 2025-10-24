import { StorageUsage } from '@/components/dashboard/StorageUsage'

export function DashboardPage() {
    return (
        <div className="flex h-full">
            <div className="flex-1 overflow-auto"></div>
            <StorageUsage />
        </div>
    )
}

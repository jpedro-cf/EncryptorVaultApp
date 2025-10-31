import { Skeleton } from '../ui/skeleton'

export function FileDisplaySkeleton() {
    return (
        <div className="flex flex-col space-y-3 bg-slate-800 p-4 rounded-md">
            <Skeleton className="h-10 w-[90vw] sm:w-[50vw] rounded-full" />
            <div className="space-y-3">
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-3 w-1/2" />
                <Skeleton className="h-3 w-1/3" />
            </div>
        </div>
    )
}

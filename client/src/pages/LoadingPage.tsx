import { Spinner } from '@/components/ui/spinner'

export function LoadingPage() {
    return (
        <div className="flex items-center justify-center min-h-screen w-full bg-linear-to-br from-slate-900 via-slate-800 to-slate-900 absolute top-0 left-0 z-50">
            <Spinner size={80} color="#fff" />
        </div>
    )
}

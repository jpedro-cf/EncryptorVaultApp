import { useAuth } from '@/hooks/use-auth'
import { cn } from '@/lib/utils'
import { CircleUser, Files, Link2, Lock } from 'lucide-react'
import { useLocation, Link } from 'react-router'

const links = [
    { href: '/', label: 'Dashboard', icon: Files },
    { href: '/shared-links', label: 'Shared Links', icon: Link2 },
    { href: '/profile', label: 'Profile', icon: CircleUser },
]
export function Sidebar() {
    const location = useLocation()
    const pathname = location.pathname

    return (
        <aside
            className={
                'fixed lg:static inset-y-0 left-0 z-50 w-64 bg-slate-800 border-r border-slate-700'
            }
        >
            <div className="flex flex-col h-full">
                {/* Header */}
                <div className="flex items-center justify-between p-6 border-b border-slate-700">
                    <div className="flex items-center gap-2">
                        <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center">
                            <Lock className="w-5 h-5 text-white" />
                        </div>
                        <span className="font-bold text-white">
                            SecureVault
                        </span>
                    </div>
                </div>

                <nav className="flex-1 p-4 space-y-2">
                    {links.map((link) => {
                        const Icon = link.icon
                        const isActive = pathname === link.href
                        return (
                            <Link
                                key={link.href}
                                to={link.href}
                                className={cn(
                                    'flex items-center gap-3 px-4 py-2 rounded-lg transition-colors',
                                    isActive
                                        ? 'bg-blue-600 text-white'
                                        : 'text-slate-400 hover:text-white hover:bg-slate-700'
                                )}
                            >
                                <Icon className="w-5 h-5" />
                                <span>{link.label}</span>
                            </Link>
                        )
                    })}
                </nav>
                <div className="p-4 border-t border-slate-700">
                    <p className="text-xs text-slate-500">
                        &copy; {new Date().getFullYear()} SecureVault
                    </p>
                </div>
            </div>
        </aside>
    )
}

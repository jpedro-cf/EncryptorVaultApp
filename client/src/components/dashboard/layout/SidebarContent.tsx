import { DrawerClose } from '@/components/ui/drawer'
import { Progress } from '@/components/ui/progress'
import { config } from '@/config/config'
import { useAuth } from '@/hooks/use-auth'
import { cn, formatFileSize } from '@/lib/utils'
import { CircleUser, Files, HardDrive, Link2, Lock } from 'lucide-react'
import { useLocation, Link, useNavigate } from 'react-router'

const totalStorage = config.TOTAL_STORAGE

const links = [
    { href: '/', label: 'Dashboard', icon: Files },
    { href: '/shared-links', label: 'Shared Links', icon: Link2 },
    { href: '/profile', label: 'Profile', icon: CircleUser },
]

interface Props {
    onLinkClick?: () => void
}
export function SidebarContent({ onLinkClick }: Props) {
    const { storageUsage } = useAuth()
    const navigate = useNavigate()

    const usedStorage = Object.values(storageUsage).reduce(
        (prev, curr) => curr + prev,
        0
    )
    const percentage = (usedStorage / totalStorage) * 100
    const location = useLocation()
    const pathname = location.pathname

    return (
        <div className="flex flex-col h-full">
            <div className="flex items-center justify-between p-6 border-b border-slate-700">
                <div className="flex items-center gap-2">
                    <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center">
                        <Lock className="w-5 h-5 text-white" />
                    </div>
                    <span className="font-bold text-white">SecureVault</span>
                </div>
            </div>

            <nav className="flex-1 p-4">
                <ul className="space-y-2">
                    {links.map((link) => {
                        const Icon = link.icon
                        const isActive = pathname === link.href
                        return (
                            <li key={link.href}>
                                <Link
                                    aria-current={isActive}
                                    to={link.href}
                                    onClick={() => onLinkClick?.()}
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
                            </li>
                        )
                    })}
                </ul>
            </nav>
            <div className="p-4 mb-5 block lg:hidden">
                <h3 className="text-md font-semibold text-white mb-4 flex items-center gap-2">
                    <HardDrive className="w-5 h-5 text-blue-400" />
                    Storage Usage
                </h3>
                <div className="space-y-3">
                    <div>
                        <div className="flex items-center justify-between mb-2">
                            <span className="text-xs text-slate-300">
                                {formatFileSize(usedStorage)} of{' '}
                                {formatFileSize(totalStorage)}
                            </span>
                            <span className="text-xs font-semibold text-blue-400">
                                {Math.round(percentage)}%
                            </span>
                        </div>
                        <Progress
                            value={percentage}
                            className="h-2 bg-slate-700"
                        />
                    </div>
                </div>
            </div>
            <div className="p-4 border-t border-slate-700">
                <p className="text-xs text-slate-500">
                    &copy; {new Date().getFullYear()} SecureVault
                </p>
            </div>
        </div>
    )
}

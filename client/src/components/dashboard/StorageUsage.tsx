import { HardDrive, Lock, Share2 } from 'lucide-react'
import { Progress } from '@/components/ui/progress'
import { Button } from '@/components/ui/button'
import { Link } from 'react-router'
import { useAuth } from '@/hooks/use-auth'
import type { ContentType } from '@/types/items'
import { formatFileSize } from '@/lib/utils'
import { config } from '@/config/config'

const colors: Record<ContentType, string> = {
    Text: 'bg-blue-500',
    Image: 'bg-purple-500',
    Video: 'bg-pink-500',
    Audio: 'bg-green-500',
    Application: 'bg-slate-500',
}

const totalStorage = config.TOTAL_STORAGE

export function StorageUsage() {
    const { storageUsage } = useAuth()

    if (!storageUsage) {
        return <></>
    }

    const usedStorage = Object.values(storageUsage).reduce(
        (prev, curr) => curr + prev,
        0
    )
    const percentage = (usedStorage / totalStorage) * 100

    return (
        <aside className="w-80 border-l border-slate-700 bg-slate-800 p-6 overflow-y-auto hidden lg:block">
            <div className="space-y-6">
                <div>
                    <h3 className="text-lg font-semibold text-white mb-4 flex items-center gap-2">
                        <HardDrive className="w-5 h-5 text-blue-400" />
                        Storage Usage
                    </h3>
                    <div className="space-y-3">
                        <div>
                            <div className="flex items-center justify-between mb-2">
                                <span className="text-sm text-slate-300">
                                    {formatFileSize(usedStorage)} of{' '}
                                    {formatFileSize(totalStorage)}
                                </span>
                                <span className="text-sm font-semibold text-blue-400">
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

                <div>
                    <h4 className="text-sm font-semibold text-white mb-3">
                        Breakdown
                    </h4>
                    <div className="space-y-2">
                        {Object.entries(storageUsage).map(([type, value]) => (
                            <div
                                key={type}
                                className="flex items-center justify-between"
                            >
                                <div className="flex items-center gap-2">
                                    <div
                                        className={`w-2 h-2 rounded-full ${
                                            colors[type as ContentType]
                                        }`}
                                    />
                                    <span className="text-sm text-slate-300">
                                        {type}
                                    </span>
                                </div>
                                <span className="text-sm font-medium text-slate-200">
                                    {formatFileSize(value)}
                                </span>
                            </div>
                        ))}
                    </div>
                </div>

                <div className="p-4 bg-slate-700/50 rounded-lg border border-slate-600 space-y-3">
                    <div className="flex items-center gap-2">
                        <Lock className="w-4 h-4 text-green-400" />
                        <span className="text-sm font-semibold text-white">
                            End-to-End Encrypted
                        </span>
                    </div>
                    <p className="text-xs text-slate-400">
                        All your files are encrypted with with E2EE, our servers
                        have no idea about the content of your files.
                    </p>
                </div>

                <div className="p-4 bg-slate-700/50 rounded-lg border border-slate-600 space-y-3">
                    <div className="flex items-center gap-2">
                        <Share2 className="w-4 h-4 text-blue-400" />
                        <span className="text-sm font-semibold text-white">
                            Shared Items
                        </span>
                    </div>
                    <Button variant="outline" className="w-full">
                        <Link to="/shared-links">Manage Sharing</Link>
                    </Button>
                </div>
            </div>
        </aside>
    )
}

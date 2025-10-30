import type { PropsWithChildren } from 'react'
import { Sidebar } from './Sidebar'
import { Header } from './Header'
import { StorageUsage } from '../StorageUsage'

export function DashboardLayout({ children }: PropsWithChildren) {
    return (
        <div
            className="flex h-screen bg-slate-900 text-slate-100"
            aria-hidden="false"
        >
            <Sidebar />
            <div className="flex-1 flex flex-col overflow-hidden">
                <Header />
                <main className="flex-1 flex overflow-auto">
                    {children}

                    <StorageUsage />
                </main>
            </div>
        </div>
    )
}

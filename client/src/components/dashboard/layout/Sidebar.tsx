import { SidebarContent } from './SidebarContent'

export function Sidebar() {
    return (
        <aside
            className={
                'hidden md:block static inset-y-0 left-0 z-50 w-64 bg-slate-800 border-r border-slate-700 max-h-screen overflow-y-scroll'
            }
        >
            <SidebarContent />
        </aside>
    )
}

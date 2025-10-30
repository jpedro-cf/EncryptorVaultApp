import {
    Drawer,
    DrawerContent,
    DrawerTitle,
    DrawerTrigger,
} from '@/components/ui/drawer'
import { useState, type PropsWithChildren } from 'react'
import { SidebarContent } from './SidebarContent'

export function SidebarDrawer({ children }: PropsWithChildren) {
    const [open, setOpen] = useState(false)
    return (
        <Drawer direction="left" open={open} onOpenChange={setOpen}>
            <DrawerTrigger asChild>{children}</DrawerTrigger>
            <DrawerContent
                aria-describedby={undefined}
                className="bg-slate-800 border-r border-slate-700 max-h-screen overflow-y-scroll"
            >
                <DrawerTitle className="hidden" />
                <SidebarContent onLinkClick={() => setOpen(false)} />
            </DrawerContent>
        </Drawer>
    )
}

import { type PropsWithChildren } from 'react'
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { AlertCircle } from 'lucide-react'

interface Props extends PropsWithChildren {
    open: boolean
    setOpen: (open: boolean) => void
}
export function ItemSharedDialog({ setOpen, open, children }: Props) {
    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogContent
                className="sm:max-w-[425px]"
                onClick={(e) => e.stopPropagation()}
            >
                <DialogHeader>
                    <DialogTitle>Shared link created!</DialogTitle>
                    <DialogDescription>
                        Copy this link to start sharing with others.
                    </DialogDescription>
                </DialogHeader>
                {children}
                <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2 mt-2">
                    <div className="text-blue-400">
                        <AlertCircle className="h-4 w-4" />
                    </div>
                    <AlertDescription className="text-blue-300 text-sm">
                        Warning! By sharing this item, other users may have full
                        access to this content and therefore be able to perform
                        malicious activities with it.
                    </AlertDescription>
                </Alert>
            </DialogContent>
        </Dialog>
    )
}

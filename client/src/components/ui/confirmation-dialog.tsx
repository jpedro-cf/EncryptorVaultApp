import { Button } from '@/components/ui/button'
import {
    Dialog,
    DialogClose,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/components/ui/dialog'
import { useState, type PropsWithChildren } from 'react'

interface Props extends PropsWithChildren {
    extraElement?: React.ReactNode
    onConfirm: () => void
}
export function ConfirmationDialog({
    children,
    onConfirm,
    extraElement,
}: Props) {
    const [open, setOpen] = useState(false)
    return (
        <Dialog onOpenChange={setOpen} open={open}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent
                className="sm:max-w-[425px] text-slate-50"
                onClick={(e) => e.stopPropagation()}
            >
                <DialogHeader>
                    <DialogTitle>Are you sure?</DialogTitle>
                    <DialogDescription>
                        Once your confirm this action, it cannot be undone.
                    </DialogDescription>
                </DialogHeader>
                {extraElement}
                <DialogFooter>
                    <DialogClose asChild>
                        <Button variant="outline">Cancel</Button>
                    </DialogClose>
                    <Button
                        type="button"
                        variant={'primary'}
                        onClick={() => {
                            onConfirm()
                            setOpen(false)
                        }}
                    >
                        Confirm
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

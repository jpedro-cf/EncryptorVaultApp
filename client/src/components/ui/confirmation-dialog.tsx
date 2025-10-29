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
    onConfirm: () => void
}
export function ConfirmationDialog({ children, onConfirm }: Props) {
    const [open, setOpen] = useState(false)
    return (
        <Dialog onOpenChange={setOpen} open={open}>
            <form>
                <DialogTrigger asChild>{children}</DialogTrigger>
                <DialogContent className="sm:max-w-[425px]">
                    <DialogHeader>
                        <DialogTitle>Are you sure?</DialogTitle>
                        <DialogDescription>
                            Once your confirm this action, it cannot be undone.
                        </DialogDescription>
                    </DialogHeader>
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
            </form>
        </Dialog>
    )
}

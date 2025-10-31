import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/components/ui/dialog'
import { useState, type PropsWithChildren } from 'react'
import { FolderForm } from './FolderForm'
import { useExplorerContext } from '../dashboard/explorer/ExplorerContext'

export function FolderDialog({ children }: PropsWithChildren) {
    const [open, setOpen] = useState(false)
    const { folderTree } = useExplorerContext()

    const currentFolder =
        folderTree.length > 0 ? folderTree[folderTree.length - 1] : null
    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Folder</DialogTitle>
                    <DialogDescription>
                        Make changes to a folder or create one.
                    </DialogDescription>
                </DialogHeader>
                <FolderForm
                    parentId={currentFolder?.id}
                    onComplete={() => setOpen(false)}
                />
            </DialogContent>
        </Dialog>
    )
}

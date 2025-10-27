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
import type { PropsWithChildren } from 'react'
import { FolderForm } from './FolderForm'
import { useExplorerContext } from '../dashboard/explorer/ExplorerContext'

export function FolderDialog({ children }: PropsWithChildren) {
    const { folderTree } = useExplorerContext()

    const currentFolder =
        folderTree.length > 0 ? folderTree[folderTree.length - 1] : null
    return (
        <Dialog>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Folder</DialogTitle>
                    <DialogDescription>
                        Make changes to a folder or create one.
                    </DialogDescription>
                </DialogHeader>
                <FolderForm parentId={currentFolder?.id} />
            </DialogContent>
        </Dialog>
    )
}

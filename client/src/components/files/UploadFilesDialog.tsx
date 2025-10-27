import { Button } from '@/components/ui/button'
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/components/ui/dialog'
import { useState, type PropsWithChildren } from 'react'
import { useExplorerContext } from '../dashboard/explorer/ExplorerContext'
import { UploadFilesForm } from './UploadFilesForm'

export function UploadFilesDialog({ children }: PropsWithChildren) {
    const [open, setOpen] = useState(false)
    const { folderTree } = useExplorerContext()

    const currentFolder =
        folderTree.length > 0 ? folderTree[folderTree.length - 1] : null

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[650px]">
                <DialogHeader>
                    <DialogTitle>Upload files</DialogTitle>
                    <DialogDescription>
                        Upload one or multiple files, they will be encrypted
                        before uploading.
                    </DialogDescription>
                </DialogHeader>
                <UploadFilesForm
                    parentId={currentFolder?.id}
                    onComplete={() => setOpen(false)}
                />
            </DialogContent>
        </Dialog>
    )
}

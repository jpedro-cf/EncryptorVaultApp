import { AlertCircle, MoreVertical } from 'lucide-react'
import { Button } from '../ui/button'
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '../ui/dropdown-menu'
import type { FileItem, FolderItem } from '@/types/items'
import { useFileDeletion } from '@/api/files/files'
import { useCreateSharedLink } from '@/api/share/share'
import { useState } from 'react'
import { ItemSharedDialog } from './ItemSharedDialog'
import { Encoding } from '@/lib/encoding'
import { config } from '@/config/config'
import { Input } from '../ui/input'
import { CopyToClipboard } from '../ui/copy-to-clipboard'
import { useDeleteFolder } from '@/api/folders/folders'
import { ConfirmationDialog } from '../ui/confirmation-dialog'
import { Alert, AlertDescription } from '../ui/alert'

interface Props {
    item: FileItem | FolderItem
}
export function ItemCardOptions({ item }: Props) {
    const [sharedLink, setSharedLink] = useState('')
    const [dialogOpen, setDialogOpen] = useState(false)

    const { mutate: deleteFile, isPending: deletingFile } = useFileDeletion()
    const { mutate: deleteFolder, isPending: deletingFolder } =
        useDeleteFolder()
    const { mutate: share, isPending: isSharing } = useCreateSharedLink()

    function handleDelete() {
        if ('contentType' in item) {
            deleteFile(item)
        } else {
            deleteFolder(item)
        }
    }

    function handleShare() {
        share(
            {
                itemId: item.id,
                itemType: 'contentType' in item ? 'File' : 'Folder',
            },
            {
                onSuccess: (data) => {
                    const urlSafeKey = Encoding.encodeUrlSafeBase64(data.key)
                    const link = `${config.APP_URL}/s/${data.id}#${urlSafeKey}`

                    setSharedLink(link)
                    setDialogOpen(true)
                },
            }
        )
    }

    const isDeleting = deletingFile || deletingFolder
    return (
        <>
            <ItemSharedDialog open={dialogOpen} setOpen={setDialogOpen}>
                <div className="relative flex items-center justify-center">
                    <Input
                        placeholder={`${config.APP_URL}/{id}#{key}`}
                        value={sharedLink}
                        disabled={true}
                        className="bg-slate-600 border-slate-500 text-white placeholder:text-slate-400 text-center placeholder:text-xs tracking-widest font-mono"
                    />
                    <CopyToClipboard
                        className="absolute right-1"
                        content={sharedLink}
                    />
                </div>
            </ItemSharedDialog>
            <DropdownMenu>
                <DropdownMenuTrigger asChild>
                    <Button
                        onClick={(e) => e.stopPropagation()}
                        variant="ghost"
                        type="button"
                        size="icon"
                        className="text-slate-400 hover:text-white"
                    >
                        <MoreVertical className="w-4 h-4" />
                    </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent
                    align="end"
                    className="bg-slate-800 border-slate-700"
                >
                    <DropdownMenuItem
                        className="text-slate-200 focus:bg-slate-700 focus-visible:text-slate-200"
                        asChild
                        disabled
                    >
                        <Button
                            variant={'ghost'}
                            className="w-full justify-start cursor-pointer"
                            disabled={true}
                        >
                            Download
                        </Button>
                    </DropdownMenuItem>
                    <DropdownMenuItem
                        className="text-slate-200 focus:bg-slate-700 focus:text-slate-200"
                        asChild
                    >
                        <Button
                            variant={'ghost'}
                            className="w-full justify-start cursor-pointer"
                            type="button"
                            onClick={(e) => {
                                e.stopPropagation()
                                handleShare()
                            }}
                            disabled={isSharing}
                        >
                            Share
                        </Button>
                    </DropdownMenuItem>
                    <DropdownMenuItem
                        className="text-red-400 focus:bg-slate-700 focus:text-red-300"
                        asChild
                    >
                        <ConfirmationDialog
                            onConfirm={handleDelete}
                            extraElement={<DeletionAlert />}
                        >
                            <Button
                                variant={'ghost'}
                                className="w-full justify-start cursor-pointer text-red-400 focus:bg-slate-700 focus:text-red-300 px-2"
                                type="button"
                                onClick={(e) => e.stopPropagation()}
                                disabled={isDeleting}
                            >
                                Delete
                            </Button>
                        </ConfirmationDialog>
                    </DropdownMenuItem>
                </DropdownMenuContent>
            </DropdownMenu>
        </>
    )
}

export function DeletionAlert() {
    return (
        <Alert className="border-yellow-500/50 bg-yellow-500/10 flex items-start gap-2">
            <div className="text-yellow-400">
                <AlertCircle className="h-4 w-4" />
            </div>
            <AlertDescription className="text-yellow-300 text-sm">
                Deletion of folders might take some time to affect your storage
                usage.
            </AlertDescription>
        </Alert>
    )
}

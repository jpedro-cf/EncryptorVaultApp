import { useFile } from '@/api/files/files'
import { Button } from '@/components/ui/button'
import {
    Dialog,
    DialogClose,
    DialogContent,
    DialogDescription,
    DialogTitle,
    DialogTrigger,
} from '@/components/ui/dialog'
import { useEffect, useState, type PropsWithChildren } from 'react'
import { useExplorerContext } from '../dashboard/explorer/ExplorerContext'
import { AlertCircle, XCircle } from 'lucide-react'
import {
    ApplicationDisplay,
    AudioDisplay,
    ImageDisplay,
    TextDisplay,
    VideoDisplay,
    type DisplayProps,
} from './FileDisplays'
import type { ContentType } from '@/types/items'
import { Alert, AlertDescription } from '../ui/alert'
import { FileDisplaySkeleton } from './FileDisplaySkeleton'

export function FileViewerDialog() {
    const [open, setOpen] = useState(false)
    const { shareId, currentFile } = useExplorerContext()

    const displays: Record<ContentType, React.ComponentType<DisplayProps>> = {
        Text: TextDisplay,
        Audio: AudioDisplay,
        Image: ImageDisplay,
        Application: ApplicationDisplay,
        Video: VideoDisplay,
    }

    const { data, isLoading, isError, isSuccess } = useFile({
        enabled: open && currentFile != null,
        fileId: currentFile?.id ?? '',
        sharedLinkId: shareId ?? undefined,
    })

    useEffect(() => {
        setOpen(currentFile != null)
    }, [currentFile])

    if (!currentFile) {
        return <></>
    }

    const Display = displays[currentFile.contentType]

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogContent
                className="max-w-[95vw] max-h-[90vh] sm:max-w-auto w-auto overflow-x-hidden text-slate-100 bg-transparent justify-center border-none p-0 overflow-y-scroll"
                showCloseButton={false}
            >
                <DialogTitle className="hidden">File Viewer Dialog</DialogTitle>
                <DialogDescription className="hidden">
                    View your file content below:
                </DialogDescription>
                {isError && (
                    <div className="bg-slate-900 p-5">
                        <Alert className="border-red-500/50 bg-red-500/10 flex items-start gap-2">
                            <div className="text-red-400">
                                <AlertCircle className="h-4 w-4" />
                            </div>
                            <AlertDescription className="text-red-300 text-sm">
                                An error occurred. Try again later or contact
                                us.
                            </AlertDescription>
                        </Alert>
                    </div>
                )}
                {isLoading && <FileDisplaySkeleton />}
                {data && isSuccess && (
                    <Display
                        contentType={data.contentType}
                        fileContent={data.fileContent}
                        id={data.id}
                    />
                )}
            </DialogContent>
        </Dialog>
    )
}

import type { SharedLinkResponse } from '@/types/share'
import { Card } from '../ui/card'
import { cn } from '@/lib/utils'
import { LinkIcon, Trash } from 'lucide-react'
import { Button } from '../ui/button'
import { useDeleteSharedLink } from '@/api/share/share'
import { ConfirmationDialog } from '../ui/confirmation-dialog'

interface Props {
    sharedLink: SharedLinkResponse
}
export function SharedLinkCard({ sharedLink }: Props) {
    const { mutate, isPending } = useDeleteSharedLink()

    function handleDelete() {
        mutate(sharedLink.id)
    }

    return (
        <Card
            className={cn(
                'h-full p-4 bg-slate-800 border border-blue-600/40 rounded-lg  text-slate-100 flex items-start justify-between flex-row'
            )}
        >
            <div className="flex items-start justify-between mb-3">
                <LinkIcon />
            </div>
            <div className="flex-1 max-w-full wrap-break-words w-1/2">
                <h3 className="font-semibold text-white truncate mb-1">
                    {sharedLink.id}
                </h3>
                <p className="text-xs text-slate-400 mb-2">
                    {sharedLink.itemId}
                </p>
                <p className="text-xs text-slate-500">
                    {new Date(sharedLink.createdAt).toDateString()}
                </p>
            </div>
            <ConfirmationDialog onConfirm={handleDelete}>
                <Button
                    variant={'destructive'}
                    size={'icon-sm'}
                    disabled={isPending}
                >
                    <Trash />
                </Button>
            </ConfirmationDialog>
        </Card>
    )
}

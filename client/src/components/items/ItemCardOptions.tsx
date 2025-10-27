import { MoreVertical } from 'lucide-react'
import { Button } from '../ui/button'
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '../ui/dropdown-menu'
import type { ItemType } from '@/types/items'

interface Props {
    type: ItemType
    id: string
}
export function ItemCardOptions({ type, id }: Props) {
    function handleDelete() {}

    function handleShare() {}
    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button
                    variant="ghost"
                    type="button"
                    size="icon"
                    className="opacity-0 group-hover:opacity-100 text-slate-400 hover:text-white"
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
                        onClick={handleShare}
                    >
                        Share
                    </Button>
                </DropdownMenuItem>
                <DropdownMenuItem
                    className="text-red-400 focus:bg-slate-700 focus:text-red-300"
                    asChild
                >
                    <Button
                        variant={'ghost'}
                        className="w-full justify-start cursor-pointer"
                        type="button"
                        onClick={handleDelete}
                    >
                        Delete
                    </Button>
                </DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    )
}

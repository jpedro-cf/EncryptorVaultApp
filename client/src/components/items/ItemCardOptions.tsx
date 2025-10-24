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
    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button
                    variant="ghost"
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
                        disabled={false}
                    >
                        Download
                    </Button>
                </DropdownMenuItem>
                <DropdownMenuItem
                    className="text-slate-200 focus:bg-slate-700 focus-visible:text-slate-200"
                    asChild
                >
                    <Button
                        variant={'ghost'}
                        className="w-full justify-start cursor-pointer"
                        disabled={false}
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
                    >
                        Delete
                    </Button>
                </DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    )
}

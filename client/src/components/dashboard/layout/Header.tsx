import { useLogout } from '@/api/account/auth'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { Button } from '@/components/ui/button'
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { useAuth } from '@/hooks/use-auth'
import { Menu, User } from 'lucide-react'
import { Link } from 'react-router'

export function Header() {
    const { account } = useAuth()
    const { mutate, isPending } = useLogout()
    return (
        <header className="border-b border-slate-700 bg-slate-800 px-6 py-2 flex items-center justify-between text-slate-100">
            <Button variant="ghost" size="icon" className="md:hidden">
                <Menu className="w-5 h-5" />
            </Button>

            <div className="flex-1" />

            <div className="flex items-center gap-4">
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button
                            variant="ghost"
                            className="py-5 flex items-center gap-3"
                        >
                            <Avatar className="w-8 h-8 text-slate-800">
                                <AvatarFallback>
                                    {account?.email.split('')[0]}
                                </AvatarFallback>
                            </Avatar>
                            <span>{account?.email}</span>
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent
                        align="end"
                        className="bg-slate-800 border-slate-700"
                    >
                        <DropdownMenuItem
                            className="text-slate-200 focus:bg-slate-700 focus:text-slate-100"
                            asChild
                        >
                            <Link
                                to="/profile"
                                className="hover:text-slate-200 cursor-pointer w-full"
                            >
                                <User className="w-4 h-4 mr-2" />
                                <span>Profile</span>
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuSeparator className="bg-slate-700" />
                        <DropdownMenuItem
                            className="text-red-400 focus:bg-slate-700 focus:text-red-300"
                            asChild
                        >
                            <Button
                                variant={'ghost'}
                                className="w-full cursor-pointer"
                                disabled={isPending}
                                onClick={() => mutate()}
                            >
                                Sign Out
                            </Button>
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            </div>
        </header>
    )
}

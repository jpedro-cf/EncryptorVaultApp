import { AnimatePresence, motion } from 'motion/react'
import { CheckIcon, CopyIcon } from 'lucide-react'
import { useCallback, useState } from 'react'
import { buttonVariants } from './button'
import { cn } from '@/lib/utils'

interface Props {
    content: string
    className?: string
}
const delay = 3000
export function CopyToClipboard({ content, className }: Props) {
    const [localIsCopied, setLocalIsCopied] = useState(false)
    const Icon = localIsCopied ? CheckIcon : CopyIcon

    function handleIsCopied(isCopied: boolean) {
        setLocalIsCopied(isCopied)
    }
    const handleCopy = useCallback(
        (e: React.MouseEvent<HTMLButtonElement>) => {
            if (content) {
                navigator.clipboard
                    .writeText(content)
                    .then(() => {
                        handleIsCopied(true)
                        setTimeout(() => handleIsCopied(false), delay)
                    })
                    .catch((error) => {
                        console.error('Error copying command', error)
                    })
            }
        },
        [content, delay, handleIsCopied]
    )
    return (
        <motion.button
            data-slot="copy-button"
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            className={cn(
                buttonVariants({ variant: 'primary', size: 'icon-sm' }),
                className
            )}
            onClick={handleCopy}
        >
            <AnimatePresence mode="wait">
                <motion.span
                    key={localIsCopied ? 'check' : 'copy'}
                    data-slot="copy-button-icon"
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    exit={{ scale: 0 }}
                    transition={{ duration: 0.15 }}
                >
                    <Icon />
                </motion.span>
            </AnimatePresence>
        </motion.button>
    )
}

import { useDropzone } from 'react-dropzone'
import { Button } from './button'
import { cn } from '@/lib/utils'
import type { PropsWithChildren } from 'react'
import { UploadCloudIcon } from 'lucide-react'

interface Props extends PropsWithChildren {
    onDrop: (files: File[]) => void
    disabled?: boolean
    className?: string
}
export function DropZone({ onDrop, disabled, className, children }: Props) {
    const { getRootProps, isDragActive, getInputProps } = useDropzone({
        onDrop: onDrop,
    })

    return (
        <Button
            className={cn(
                'relative h-auto w-full flex-col overflow-hidden p-8 border-2 border-dashed border-slate-700',
                isDragActive && 'border-solid border-slate-700 bg-slate-700',
                className
            )}
            disabled={disabled}
            type="button"
            variant="ghost"
            {...getRootProps()}
        >
            <input {...getInputProps()} disabled={disabled} />
            {children}
        </Button>
    )
}

interface ContentProps extends PropsWithChildren {
    className?: string
}
export function DropZoneContent({ children, className }: ContentProps) {
    if (children) return children
    return (
        <div
            className={cn(
                'flex flex-col items-center justify-center',
                className
            )}
        >
            <div className="flex size-8 items-center justify-center rounded-md bg-blue-500 text-blue-100 mb-3">
                <UploadCloudIcon size={16} />
            </div>
            <p className="w-full text-wrap text-blue-50 text-xs">
                Drag and drop or click to replace
            </p>
            <p className="w-full text-wrap text-blue-50 text-xs">
                We’ll store it safely in the cloud
            </p>
        </div>
    )
}

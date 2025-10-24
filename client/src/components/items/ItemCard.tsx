import type { ContentType, FileItem, FolderItem, ItemType } from '@/types/items'
import {
    File,
    FileAudio,
    FilePlay,
    Folder,
    Image,
    NotepadText,
} from 'lucide-react'
import type { ReactElement } from 'react'
import { ItemCardOptions } from './ItemCardOptions'
import { cn, formatFileSize } from '@/lib/utils'

interface ItemCardProps {
    data: FolderItem | FileItem
}
export function ItemCard({ data }: ItemCardProps) {
    if ('contentType' in data) {
        return <FileCard file={data} />
    }
    return <FolderCard folder={data} />
}

export function FolderCard({ folder }: { folder: FolderItem }) {
    return (
        <div className="group p-4 bg-slate-800 border border-yellow-800/80 rounded-lg hover:border-yellow-700 transition-colors cursor-pointer">
            <div className="flex items-start justify-between mb-3">
                <div className="p-2 bg-yellow-900 rounded-lg group-hover:bg-yellow-600 transition-colors">
                    <Folder className="w-6 h-6 text-yellow-400 group-hover:text-white" />
                </div>
                <ItemCardOptions type="folder" id={folder.id} />
            </div>
            <h3 className="font-semibold text-white truncate mb-1">
                {folder.name}
            </h3>
            <p className="text-xs text-slate-500">
                {folder.createdAt.toDateString()}
            </p>
        </div>
    )
}

export function FileCard({ file }: { file: FileItem }) {
    const styles: Record<ContentType, string> = {
        application: 'border-slate-700 hover:border-slate-600',
        audio: 'border-green-900/60 hover:border-green-800',
        image: 'border-purple-900/60',
        text: '',
        video: 'border-red-900/30',
    }
    return (
        <div
            className={cn(
                'group p-4 bg-slate-800 border border-transparent rounded-lg transition-colors cursor-pointer',
                styles[file.contentType]
            )}
        >
            <div className="flex items-start justify-between mb-3">
                <FileIcon type={file.contentType} />
                <ItemCardOptions type="file" id={file.id} />
            </div>
            <h3 className="font-semibold text-white truncate mb-1">
                {file.name}
            </h3>
            <p className="text-xs text-slate-400 mb-2">
                {formatFileSize(file.size)}
            </p>
            <p className="text-xs text-slate-500">
                {file.createdAt.toDateString()}
            </p>
        </div>
    )
}

export function FileIcon({ type }: { type: ContentType }) {
    const icons: Record<ContentType, ReactElement> = {
        application: (
            <div className="p-2 bg-slate-700 rounded-lg group-hover:bg-slate-300 transition-colors">
                <File className="w-6 h-6 text-slate-400 group-hover:text-slate-900" />
            </div>
        ),
        audio: (
            <div className="p-2 bg-slate-700 rounded-lg group-hover:bg-green-900 transition-colors">
                <FileAudio className="w-6 h-6 text-green-400 group-hover:text-white" />
            </div>
        ),
        image: (
            <div className="p-2 bg-slate-700 rounded-lg group-hover:bg-purple-600 transition-colors">
                <Image className="w-6 h-6 text-purple-400 group-hover:text-white" />
            </div>
        ),
        text: (
            <div className="p-2 bg-slate-700 rounded-lg group-hover:bg-blue-600 transition-colors">
                <NotepadText className="w-6 h-6 text-blue-400 group-hover:text-white" />
            </div>
        ),
        video: (
            <div className="p-2 bg-slate-700 rounded-lg group-hover:bg-red-800 transition-colors">
                <FilePlay className="w-6 h-6 text-red-400 group-hover:text-white" />
            </div>
        ),
    }

    return icons[type] ?? icons['application']
}

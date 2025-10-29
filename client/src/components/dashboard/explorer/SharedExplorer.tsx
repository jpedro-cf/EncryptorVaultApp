import type { Folder } from '@/types/folders'
import { useEffect, useState } from 'react'
import { ExplorerContext } from './ExplorerContext'
import { useKeys } from '@/hooks/use-keys'
import { useFolder } from '@/api/folders/folders'
import { ExplorerHeader } from './ExplorerHeader'
import { ItemsGrid } from '@/components/items/ItemsGrid'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { AlertCircle, ArrowLeftSquare } from 'lucide-react'
import { Link } from 'react-router'
import { Button } from '@/components/ui/button'
import { useSharedLink } from '@/api/share/share'

interface SharedExplorerProps {
    sharedLinkId: string
}

export function SharedExplorer({ sharedLinkId }: SharedExplorerProps) {
    const [currentFolderId, setCurrentFolderId] = useState<string | null>(null)
    const [folderTree, setFolderTree] = useState<Folder[]>([])

    function pushFolder(folder: Folder) {
        setFolderTree((prev) => {
            const idx = prev.findIndex((x) => x.id === folder.id)

            if (idx !== -1) return prev.slice(0, idx + 1)
            return [...prev, folder]
        })
    }

    const isInFolderPage = currentFolderId != null

    const sharedItems = useSharedLink({
        shareId: sharedLinkId,
        enabled: !isInFolderPage,
    })

    const folderQuery = useFolder({
        enabled: isInFolderPage,
        folderId: currentFolderId ?? '',
        shareId: sharedLinkId,
    })

    useEffect(() => {
        if (folderQuery.data) {
            pushFolder(folderQuery.data)
        }
    }, [folderQuery.data])

    useEffect(() => {
        if (currentFolderId == null) {
            setFolderTree([])
        }
    }, [currentFolderId])

    if (sharedItems.isError || folderQuery.isError) {
        return (
            <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2 mt-5">
                <div className="text-blue-400">
                    <AlertCircle className="h-4 w-4" />
                </div>
                <AlertDescription className="text-blue-300 text-sm">
                    An error occured while fetching the content shared with you,
                    try again later or verify if the shared link is still valid.
                    {isInFolderPage && (
                        <Button
                            variant={'primary'}
                            size={'sm'}
                            className="mt-1"
                            onClick={() =>
                                setCurrentFolderId(
                                    folderTree[folderTree.length - 1].id
                                )
                            }
                        >
                            Go back <ArrowLeftSquare />
                        </Button>
                    )}
                </AlertDescription>
            </Alert>
        )
    }

    return (
        <ExplorerContext.Provider
            value={{
                shareId: sharedLinkId,
                folderTree,
                pushFolder,
                currentFolderId,
                setCurrentFolderId,
                items: folderQuery.data?.children ?? sharedItems.data ?? [],
            }}
        >
            <ExplorerHeader />
            <ItemsGrid />
        </ExplorerContext.Provider>
    )
}

import type { FileItem, FolderItem } from '@/types/items'
import { ItemCard } from './ItemCard'

interface Props {
    items: (FileItem | FolderItem)[]
}
export function ItemsGrid({ items }: Props) {
    return (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {items.map((item) => (
                <ItemCard key={item.id} data={item} />
            ))}
        </div>
    )
}

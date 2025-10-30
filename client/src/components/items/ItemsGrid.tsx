import { ItemCard } from './ItemCard'
import { cn } from '@/lib/utils'
import { useExplorerContext } from '@/components/dashboard/explorer/ExplorerContext'

export function ItemsGrid({
    className,
    ...props
}: React.ComponentProps<'div'>) {
    const { items } = useExplorerContext()

    return (
        <div
            className={cn(
                'grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-4',
                className
            )}
            {...props}
        >
            {items.map((item) => (
                <ItemCard key={item.id} data={item} />
            ))}
        </div>
    )
}

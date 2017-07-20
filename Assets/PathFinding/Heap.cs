
public interface IHeapItemComparer<in T> {
    int Compare(T a, T b);
}

public interface IHeapItem<T> {
    int GetHeapIndex();
    void SetHeapIndex(int val);
}

public class Heap<T> where T : IHeapItem<T> {
	
    private readonly T[] items;
    private int currentItemCount;
    private IHeapItemComparer<T> itemComparer;
	
    public Heap(int maxHeapSize, IHeapItemComparer<T> itemComparer) {
        items = new T[maxHeapSize];
        this.itemComparer = itemComparer;
        currentItemCount = 0;
    }

    public void Clear() {
        for (int i = 0; i < items.Length; ++i) {
            items[i] = default(T);
        }
        currentItemCount = 0;
    }
	
    public void Add(T item) {
        item.SetHeapIndex(currentItemCount);
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst() {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].SetHeapIndex(0);
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item) {
        SortUp(item);
    }

    public int Count {
        get {
            return currentItemCount;
        }
    }

    public bool Contains(T item) {
        return Equals(items[item.GetHeapIndex()], item);
    }

    private void SortDown(T item) {
        while (true) {
            int childIndexLeft = item.GetHeapIndex() * 2 + 1;
            int childIndexRight = childIndexLeft + 1;

            if (childIndexLeft < currentItemCount) {
                int swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount) {
                    if (itemComparer.Compare(items[childIndexLeft], items[childIndexRight]) < 0) {
                        swapIndex = childIndexRight;
                    }
                }

                if (itemComparer.Compare(item, items[swapIndex]) < 0) {
                    Swap(item,items[swapIndex]);
                }
                else {
                    return;
                }
            }
            else {
                return;
            }

        }
    }
	
    private void SortUp(T item) {
        int parentIndex = (item.GetHeapIndex() - 1) / 2;
		
        while (true) {
            T parentItem = items[parentIndex];
            if (itemComparer.Compare(item, parentItem) > 0) {
                Swap(item, parentItem);
                parentIndex = (item.GetHeapIndex() - 1) / 2;
            } else {
                break;
            }
        }
    }
	
    private void Swap(T itemA, T itemB) {
        int itemAIndex = itemA.GetHeapIndex();
        int itemBIndex = itemB.GetHeapIndex();
        items[itemAIndex] = itemB;
        items[itemBIndex] = itemA;
        itemA.SetHeapIndex(itemBIndex);
        itemB.SetHeapIndex(itemAIndex);
    }
}

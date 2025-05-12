namespace TableMgmtApp;

public class RingBuffer<T> {
    public T[] circularArray;
    private int frontIndex;
    private int rearIndex;
    public int usageCount;

    public RingBuffer(int k) {
        circularArray = new T[k];
        frontIndex = rearIndex = 0;
        usageCount = 0;
    }

    public bool EnQueue(T value) {
        if (IsFull()) {
            DeQueue();
        }

        if (IsEmpty()) {
            circularArray[rearIndex] = value;
            usageCount++;
            return true;
        } 

        rearIndex = (rearIndex + 1) % circularArray.Length;
        circularArray[rearIndex] = value;
        usageCount++;
        return true;
    }

    private bool DeQueue() {
        if (IsEmpty()) {
            return false;
        }
        
        if (usageCount == 1) {
            rearIndex = (rearIndex + 1) % circularArray.Length;
        }

        frontIndex = (frontIndex + 1) % circularArray.Length;
        usageCount--;
        return true;
    }

    public T Front() {
        if (IsEmpty()) {
            return default!;
        }

        return circularArray[frontIndex];
    }

    public T Rear() {
        if (IsEmpty()) {
            return default!;
        }

        return circularArray[rearIndex];
    }

    public bool IsEmpty() {
        if (usageCount == 0) {
            return true;
        }

        return false;
    }

    public bool IsFull() {
        if (usageCount == circularArray.Length) {
            return true;
        }

        return false;
    }
}



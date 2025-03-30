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

/**
 * Your MyCircularQueue object will be instantiated and called as such:
 * MyCircularQueue obj = new MyCircularQueue(k);
 * bool param_1 = obj.EnQueue(value);
 * bool param_2 = obj.DeQueue();
 * int param_3 = obj.Front();
 * int param_4 = obj.Rear();
 * bool param_5 = obj.IsEmpty();
 * bool param_6 = obj.IsFull();
 */

/**
  MyCircularQueue(k) Initializes the object with the size of the queue to be k.
  int Front() Gets the front item from the queue. If the queue is empty, return -1.
  int Rear() Gets the last item from the queue. If the queue is empty, return -1.
  boolean enQueue(int value) Inserts an element into the circular queue. Return true if the operation is successful.
  boolean deQueue() Deletes an element from the circular queue. Return true if the operation is successful.
  boolean isEmpty() Checks whether the circular queue is empty or not.
  boolean isFull() Checks whether the circular queue is full or not.
  */





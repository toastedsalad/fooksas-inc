namespace TableMgmtApp.Test;

[Parallelizable(ParallelScope.All)]
public class RingBufferTest {

    [Test]
    public void RingBufferOverflowOverwritesOldestValue() {
        var ringBuffer = new RingBuffer<int>(3);

        ringBuffer.EnQueue(1);
        ringBuffer.EnQueue(2);
        ringBuffer.EnQueue(3);

        Assert.That(ringBuffer.Front(), Is.EqualTo(1));
        Assert.That(ringBuffer.Rear(), Is.EqualTo(3));

        ringBuffer.EnQueue(4);

        Assert.That(ringBuffer.Front(), Is.EqualTo(2));
        Assert.That(ringBuffer.Rear(), Is.EqualTo(4));
    }
}

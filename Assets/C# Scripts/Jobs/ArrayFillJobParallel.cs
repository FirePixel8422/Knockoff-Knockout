using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;


[BurstCompile(DisableSafetyChecks = true)]
public struct ArrayFillJobParallel<T> : IJobParallelFor where T : unmanaged
{
    [NativeDisableParallelForRestriction]
    [WriteOnly][NoAlias] public NativeArray<T> array;

    [ReadOnly][NoAlias] public T value;


    [BurstCompile(DisableSafetyChecks = true)]
    public void Execute(int index)
    {
        array[index] = value;
    }
}
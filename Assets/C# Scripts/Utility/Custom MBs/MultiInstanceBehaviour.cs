using UnityEngine;


public abstract class MultiInstanceBehaviour<T> : MonoBehaviour where T : MultiInstanceBehaviour<T>
{
    [SerializeField] private int id;
    public int Id => id;

#pragma warning disable UDR0001
    private static T[] instances;
#pragma warning restore UDR0001
    public static T[] Instances => instances;

    protected virtual void Awake()
    {
        if (Instances == null)
        {
            instances = new T[id + 1];
        }
        else if (id >= Instances.Length)
        {
            System.Array.Resize(ref instances, id + 1);
        }

        Instances[id] = (T)this;
    }
}

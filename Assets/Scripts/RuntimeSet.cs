using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    public List<T> items = new List<T>();

    public void AddItem(T item)
    {
        if(!items.Contains(item))
            items.Add(item);
    }

    public void RemoveItem(T item)
    {
        if (items.Contains(item))
            items.Remove(item);
    }

    public void Clear()
    {
        items.Clear();
    }
}

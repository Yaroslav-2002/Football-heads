using UnityEngine;

public class GameobjectsRuntimeSetMember : MonoBehaviour
{
    [SerializeField] private GameobjectsRuntimeSet runtimeSet;

    private void OnEnable()
    {
        if (runtimeSet == null)
            return;

        runtimeSet.AddItem(gameObject);
    }

    private void OnDisable()
    {
        if (runtimeSet == null)
            return;

        runtimeSet.RemoveItem(gameObject);
    }
}

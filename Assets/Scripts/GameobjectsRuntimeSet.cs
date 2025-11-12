using UnityEngine;

[CreateAssetMenu(
    fileName = "GameobjectsRuntimeSet",
    menuName = "GameobjectsRuntimeSet")]
public class GameobjectsRuntimeSet : RuntimeSet<GameObject>
{
    private void OnEnable()
    {
        items.Clear();
    }
}

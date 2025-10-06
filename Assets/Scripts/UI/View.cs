using UnityEngine;

public abstract class View : MonoBehaviour
{
    public abstract void Init();
    public void Show() => gameObject.SetActive(false);
    public void Hide() => gameObject.SetActive(false);
}

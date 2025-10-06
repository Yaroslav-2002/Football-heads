using UnityEngine;

public abstract class View : MonoBehaviour
{
    private bool _isInitialized;

    /// <summary>
    /// Ensures that the view is initialised once before it is shown.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        Init();
        _isInitialized = true;
    }

    public abstract void Init();

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Selectable))]
public class UIButtonHoldHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public event Action Pressed;
    public event Action Released;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable())
        {
            return;
        }

        Pressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable())
        {
            return;
        }

        Released?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable())
        {
            return;
        }

        if (eventData.pointerPress == gameObject)
        {
            Released?.Invoke();
        }
    }

    private bool IsInteractable()
    {
        if (TryGetComponent(out Selectable selectable))
        {
            return selectable.IsInteractable();
        }

        return true;
    }
}

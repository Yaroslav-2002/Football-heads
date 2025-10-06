using UnityEngine;
using System.Collections.Generic;

public class ViewManager : MonoBehaviour
{
    private static ViewManager instance;
    [SerializeField] private View startingView;
    [SerializeField] private View[] views;

    private View currentView;

    public Stack<View> history = new();

    private void Awake()
    {
        instance = this;
    }

    public static T GetView<T>() where T : View
    {
        foreach (var view in instance.views)
        {
            if (view is T) return (T)view;
        }

        Debug.Log($"No such view {typeof(T).Name}");
        return null;
    }

    public static void Show<T>(bool remember = true) where T : View
    {
        foreach (var view in instance.views)
        {
            if (view is not T)
                continue;

            if (instance.currentView != null)
            {
                if (remember)
                {
                    instance.history.Push(instance.currentView);
                }

                instance.currentView.Hide();

                view.Show();
                instance.currentView = view;
            }
        }

        Debug.Log($"No such view {typeof(T).Name}");
    }

    public static void Show(View view, bool remember = true)
    {
        if (instance.currentView != null)
        {
            if (remember)
            {
                instance.history.Push(instance.currentView);
            }

            instance.currentView.Hide();
        }

        view.Show();
        instance.currentView = view;
    }

    public static void ShowLast()
    {
        if(instance.history.Count != 0)
        {
            Show(instance.history.Pop(), false);
        }
    }
}
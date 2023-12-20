using UnityEngine;
using UnityEngine.Events;

sealed class UnityEventsListener : MonoBehaviour
{
    public bool IsEnabled { get; private set; }
    public readonly UnityEvent onEnable = new UnityEvent();
    public readonly UnityEvent onDisable = new UnityEvent();

    void OnEnable()
    {
        IsEnabled = true;
        onEnable.Invoke();
    }

    void OnDisable()
    {
        IsEnabled = false;
        onDisable.Invoke();
    }

    void OnDestroy()
    {
        IsEnabled = false;
        onEnable.RemoveAllListeners();
        onDisable.RemoveAllListeners();
    }
}

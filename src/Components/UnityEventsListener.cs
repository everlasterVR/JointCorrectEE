using UnityEngine;
using UnityEngine.Events;

public class UnityEventsListener : MonoBehaviour
{
    public readonly UnityEvent onDisable = new UnityEvent();
    public readonly UnityEvent onEnable = new UnityEvent();

    public void OnDisable()
    {
        onDisable.Invoke();
    }

    public void OnEnable()
    {
        onEnable.Invoke();
    }

    private void OnDestroy()
    {
        onDisable.RemoveAllListeners();
        onEnable.RemoveAllListeners();
    }
}

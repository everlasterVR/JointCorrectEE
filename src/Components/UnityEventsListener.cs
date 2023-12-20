using System;
using UnityEngine;

sealed class UnityEventsListener : MonoBehaviour
{
    internal bool IsEnabled { get; private set; }
    internal Action enabledHandlers;
    internal Action disabledHandlers;

    void OnEnable()
    {
        IsEnabled = true;
        enabledHandlers?.Invoke();
    }

    void OnDisable()
    {
        IsEnabled = false;
        disabledHandlers?.Invoke();
    }
}

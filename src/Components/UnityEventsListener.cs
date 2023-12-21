using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

sealed class UnityEventsListener : MonoBehaviour
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public bool IsEnabled { get; private set; }

    public Action enabledHandlers;
    public Action disabledHandlers;

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

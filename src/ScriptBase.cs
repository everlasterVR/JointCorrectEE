using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.UI;

class ScriptBase : MVRScript
{
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public bool IsInitialized { get; protected set; }

    UnityEventsListener _pluginUIEventsListener;

    public override bool ShouldIgnore() => true; // Prevent ScriptBase from showing up as a plugin in Plugins tab

    public override void InitUI()
    {
        base.InitUI();
        if(!UITransform || _pluginUIEventsListener)
        {
            return;
        }

        StartCoroutine(InitUICo());
    }

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual Action OnUIEnabled() => null;

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual Action OnUIDisabled() => null;

    IEnumerator InitUICo()
    {
        while(!IsInitialized)
        {
            yield return null;
        }

        _pluginUIEventsListener = UITransform.gameObject.AddComponent<UnityEventsListener>();
        _pluginUIEventsListener.onEnable.AddListener(() => StartCoroutine(OnUIEnabledCo(OnUIEnabled())));

        var onUIDisabled = OnUIDisabled();
        if(onUIDisabled != null)
        {
            _pluginUIEventsListener.onDisable.AddListener(() => StartCoroutine(OnUIDisabledCo(onUIDisabled)));
        }
    }

    bool _inEnabledCo;

    IEnumerator OnUIEnabledCo(Action callback = null)
    {
        if(_inEnabledCo)
        {
            /* When VAM UI is toggled back on with the plugin UI already active, onEnable gets called twice and onDisable once.
             * This ensures onEnable logic executes just once.
             */
            yield break;
        }

        _inEnabledCo = true;
        SetGrayBackground();

        if(callback != null)
        {
            yield return null;
            yield return null;
            yield return null;

            if(!IsInitialized)
            {
                yield break;
            }

            callback();
        }

        _inEnabledCo = false;
    }

    void SetGrayBackground()
    {
        var background = rightUIContent.parent.parent.parent.transform.GetComponent<Image>();
        background.color = Colors.backgroundGray;
    }

    IEnumerator OnUIDisabledCo(Action callback)
    {
        if(_inEnabledCo)
        {
            /* When VAM UI is toggled back on with the plugin UI already active, onEnable gets called twice and onDisable once.
             * This ensures only onEnable logic executes.
             */
            yield break;
        }

        callback();
    }

    protected void OnDestroy()
    {
        DestroyImmediate(_pluginUIEventsListener);
    }
}

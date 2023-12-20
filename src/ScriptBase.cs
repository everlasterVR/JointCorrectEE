using System;
using System.Collections;
using UnityEngine.UI;

class ScriptBase : MVRScript
{
    public override bool ShouldIgnore() => true;

    public const string VERSION = "0.0.0";
    internal readonly LogBuilder logBuilder = new LogBuilder();
    protected bool isInitialized;
    UnityEventsListener _pluginUIEventsListener;
    bool _inEnabledCo;
    bool _isUIBuilt;

    public override void InitUI()
    {
        if(ShouldIgnore())
        {
            return;
        }

        base.InitUI();
        if(!UITransform)
        {
            return;
        }

        SetGrayBackground();
        if(!_pluginUIEventsListener)
        {
            _pluginUIEventsListener = UITransform.gameObject.AddComponent<UnityEventsListener>();
            _pluginUIEventsListener.enabledHandlers += OnUIEnabled;
        }
    }

    void SetGrayBackground()
    {
        var background = rightUIContent.parent.parent.parent.transform.GetComponent<Image>();
        background.color = Colors.backgroundGray;
    }

    void OnUIEnabled() => StartCoroutine(OnUIEnabledCo());


    IEnumerator OnUIEnabledCo(Action callback = null)
    {
        if(_inEnabledCo)
        {
            /* When VAM UI is toggled back on with the plugin UI already active, onEnable gets called twice and onDisable once.
             * This ensures onEnable logic executes just once.
             */
            yield break;
        }

        while(!isInitialized)
        {
            yield return null;
        }

        _inEnabledCo = true;
        while(!isInitialized)
        {
            yield return null;
        }

        if(!_isUIBuilt)
        {
            BuildUI();
            _isUIBuilt = true;
        }

        _inEnabledCo = false;
    }

    protected virtual void BuildUI()
    {
    }

    protected void BaseDestroy()
    {
        DestroyImmediate(_pluginUIEventsListener);
        _pluginUIEventsListener = null;
    }
}

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

class ScriptBase : MVRScript
{
    public override bool ShouldIgnore() => true;

    public const string VERSION = "0.0.0";
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public readonly LogBuilder logBuilder = new LogBuilder();
    protected bool isInitialized;
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    protected bool isRestoringFromJSON;
    UnityEventsListener _parentObjectEventsListener;
    UnityEventsListener _pluginUIEventsListener;
    bool _inEnabledCo;
    bool _isUIOpened;
    UIDynamicTextField _postponedInfoField;
    Action _postponedActions;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public bool IsEnabledAndActive() => this.IsEnabledNullSafe() && gameObject.IsActiveInHierarchyNullSafe();

    void Start()
    {
        _postponedActions?.Invoke();
        _postponedActions = null;

        if(_parentObjectEventsListener == null)
        {
            _parentObjectEventsListener = transform.parent.gameObject.AddComponent<UnityEventsListener>(listener =>
            {
                listener.disabledHandlers += GoToInactiveWindow;
                listener.enabledHandlers += () =>
                {
                    if(_isUIOpened && isInitialized)
                    {
                        GoToMainWindow();
                    }
                };
            });
        }
    }

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

        if(gameObject.activeInHierarchy)
        {
            CreatePluginUIEventsListener();
        }
        else
        {
            _postponedInfoField = CreateTextField(new JSONStorableString("info", "<b>Enable the atom to initialize.</b>"));
            _postponedInfoField.backgroundColor = Color.clear;
            _postponedActions += () =>
            {
                if(_postponedInfoField != null)
                {
                    RemoveTextField(_postponedInfoField);
                }

                CreatePluginUIEventsListener();
            };
        }
    }

    void CreatePluginUIEventsListener()
    {
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

    void OnUIEnabled()
    {
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(OnUIEnabledCo());
        }
        else
        {
            GoToInactiveWindow();
        }
    }

    IEnumerator OnUIEnabledCo()
    {
        if(_inEnabledCo)
        {
            /* When VAM UI is toggled back on with the plugin UI already active, onEnable gets called twice and onDisable once.
             * This ensures onEnable logic executes just once.
             */
            yield break;
        }

        _inEnabledCo = true;
        while(!isInitialized)
        {
            yield return null;
        }

        if(!_isUIOpened)
        {
            GoToMainWindow();
            _isUIOpened = true;
        }

        _inEnabledCo = false;
    }

    protected virtual void GoToMainWindow()
    {
    }

    protected virtual void GoToInactiveWindow()
    {
    }

    protected void StartOrPostponeCoroutine(IEnumerator coroutine, Action onPostpone = null)
    {
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(coroutine);
        }
        else
        {
            onPostpone?.Invoke();
            _postponedActions += () => StartCoroutine(coroutine);
        }
    }

    protected void BaseDestroy()
    {
        DestroyImmediate(_pluginUIEventsListener);
        _pluginUIEventsListener = null;

        DestroyImmediate(_parentObjectEventsListener);
        _parentObjectEventsListener = null;
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class WindowBase
{
    protected readonly JointCorrectEE script;
    readonly string _id;
    public string GetId() => _id;

    readonly Dictionary<string, UIDynamic> _elements;

    readonly UnityAction _onReturnToParent;

    protected WindowBase(JointCorrectEE script, string id, UnityAction onReturnToParent = null)
    {
        this.script = script;
        _id = id;
        _onReturnToParent = onReturnToParent;
        _elements = new Dictionary<string, UIDynamic>();
    }

    protected void AddElement(Func<UIDynamic> createElement) => AddElement(Guid.NewGuid().ToString(), createElement);
    void AddElement(string key, Func<UIDynamic> createElement) => _elements[key] = createElement();

    void AddBackButton(bool rightSide, UnityAction onReturnToParent) => AddElement(
        () =>
        {
            var button = script.CreateButton("Return", rightSide);
            button.textColor = Color.white;
            var colors = button.button.colors;
            colors.normalColor = Colors.sliderGray;
            colors.highlightedColor = Color.grey;
            colors.pressedColor = Color.grey;
            button.button.colors = colors;
            button.AddListener(onReturnToParent);
            return button;
        });

    UIDynamicTextField CreateBasicTextField(string text, bool rightSide) =>
        script.CreateTextField(new JSONStorableString("text", text), rightSide);

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    protected UIDynamicTextField CreateHeaderTextField(
        string text,
        int fontSize,
        int height,
        bool rightSide
    )
    {
        var textField = CreateBasicTextField(text, rightSide);
        ModifyTextField(textField, fontSize, height);
        textField.UItext.alignment = TextAnchor.MiddleCenter;
        return textField;
    }

    static void ModifyTextField(UIDynamicTextField textField, int fontSize, int height)
    {
        textField.UItext.fontSize = fontSize;
        textField.backgroundColor = Color.clear;
        var layout = textField.GetComponent<LayoutElement>();
        layout.preferredHeight = height;
        layout.minHeight = height;
    }

    protected void AddInfoTextField(string text, bool rightSide, int height = 100, int fontSize = 26) => AddElement(
        () =>
        {
            var textField = CreateBasicTextField(text, rightSide);
            textField.UItext.fontSize = fontSize;
            textField.backgroundColor = Color.clear;
            var layout = textField.GetComponent<LayoutElement>();
            layout.preferredHeight = height;
            layout.minHeight = height;
            return textField;
        }
    );

    public void Build()
    {
        _elements.Clear();
        if(_onReturnToParent != null)
        {
            AddBackButton(false, _onReturnToParent);
        }

        OnBuild();
    }

    protected virtual void OnBuild()
    {
    }

    protected T GetElementAs<T>(string key)
    {
        if(_elements.ContainsKey(key))
        {
            var element = _elements[key];
            if(element is T)
            {
                return (T) Convert.ChangeType(element, typeof(T));
            }
        }

        return default(T);
    }

    public void ClosePopups()
    {
        ClosePopupsSelf();
    }

    public void Clear()
    {
        ClearSelf();
    }

    void ClearSelf()
    {
        ClosePopupsSelf();
        foreach(var pair in _elements)
        {
            script.RemoveElement(pair.Value);
        }
    }

    void ClosePopupsSelf()
    {
        foreach(var pair in _elements)
        {
            var uiDynamicPopup = pair.Value as UIDynamicPopup;
            if(uiDynamicPopup)
            {
                uiDynamicPopup.popup.visible = false;
            }
        }
    }
}

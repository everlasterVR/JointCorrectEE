using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static JointCorrectEE;

public class WindowBase : IWindow
{
    private readonly string _id;
    public string GetId() => _id;

    protected readonly Dictionary<string, UIDynamic> elements;
    protected readonly List<IWindow> nestedWindows;

    public IWindow GetActiveNestedWindow() => activeNestedWindow;
    protected IWindow activeNestedWindow;

    protected WindowBase(string id = "")
    {
        _id = id;
        elements = new Dictionary<string, UIDynamic>();
        nestedWindows = new List<IWindow>();
    }

    #region Common elements

    protected void AddSpacer(string name, int height, bool rightSide = false) =>
        elements[$"{name}Spacer"] = jointCorrectEE.NewSpacer(height, rightSide);

    protected void CreateTitleTextField(JSONStorableString storable, int fontSize, int height, bool rightSide, int spacing = 0)
    {
        AddSpacer(storable.name, spacing, rightSide);
        var textField = jointCorrectEE.CreateTextField(storable, rightSide);
        textField.UItext.fontSize = fontSize;
        textField.UItext.alignment = TextAnchor.MiddleCenter;
        textField.backgroundColor = Color.clear;
        var layout = textField.GetComponent<LayoutElement>();
        layout.preferredHeight = height;
        layout.minHeight = height;
        elements[storable.name] = textField;
    }

    protected void CreateHeaderTextField(JSONStorableString storable, bool rightSide = false, int fontSize = 30)
    {
        storable.val = "\n".Size(20) + storable.val.Bold();
        var textField = jointCorrectEE.CreateTextField(storable, rightSide);
        textField.UItext.fontSize = fontSize;
        textField.UItext.alignment = TextAnchor.LowerCenter;
        textField.backgroundColor = Color.clear;
        var layout = textField.GetComponent<LayoutElement>();
        layout.preferredHeight = 62;
        layout.minHeight = 62;
        elements[storable.name] = textField;
    }

    #endregion Common elements

    #region Life cycle

    public void Rebuild()
    {
        if(activeNestedWindow != null)
        {
            activeNestedWindow.Rebuild();
        }
        else
        {
            elements.Clear();
            OnBuild();
        }
    }

    protected virtual void OnBuild()
    {
    }

    protected virtual void OnClose()
    {
    }

    public void OnReturn()
    {
        activeNestedWindow.Clear();
        activeNestedWindow = null;
        OnBuild();
    }

    public List<UIDynamicSlider> GetSliders()
    {
        var sliders = new List<UIDynamicSlider>();
        if(elements.Any())
        {
            foreach(var element in elements)
            {
                var uiDynamicSlider = element.Value as UIDynamicSlider;
                if(uiDynamicSlider != null)
                {
                    sliders.Add(uiDynamicSlider);
                }
            }
        }

        foreach(var window in nestedWindows)
        {
            sliders.AddRange(window.GetSliders());
        }

        return sliders;
    }

    public void ClosePopups()
    {
        if(!elements.Any())
        {
            return;
        }

        foreach(var element in elements)
        {
            var uiDynamicPopup = element.Value as UIDynamicPopup;
            if(uiDynamicPopup != null)
            {
                uiDynamicPopup.popup.visible = false;
            }
        }
    }

    public void Clear()
    {
        if(activeNestedWindow != null)
        {
            activeNestedWindow.Clear();
        }
        else
        {
            ClearSelf();
        }

        OnClose();
    }

    protected void ClearSelf() =>
        elements.ToList().ForEach(element => jointCorrectEE.RemoveElement(element.Value));

    #endregion Life cycle
}

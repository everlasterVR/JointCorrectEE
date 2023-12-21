using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

static class AtomExtensions
{
    public static bool StorableExistsByRegexMatch(this Atom atom, Regex regex)
    {
        var storableIds = atom.GetStorableIDs();
        for(int i = 0; i < storableIds.Count; i++)
        {
            if(regex.IsMatch(storableIds[i]))
            {
                return true;
            }
        }

        return false;
    }
}

static class BehaviourExtensions
{
    public static bool IsEnabledNullSafe(this Behaviour behaviour) => behaviour && behaviour.enabled;
}

static class GameObjectExtensions
{
    public static bool IsActiveInHierarchyNullSafe(this GameObject gameObject) => gameObject && gameObject.activeInHierarchy;
}

static class MonoBehaviourExtensions
{
    public static T AddComponent<T>(this GameObject go, Action<T> callback) where T : MonoBehaviour
    {
        var component = go.AddComponent<T>();
        callback(component);
        return component;
    }
}

static class MVRScriptExtensions
{
    public static Transform InstantiateTextField(this MVRScript script, Transform parent = null) =>
        UnityEngine.Object.Instantiate(script.manager.configurableTextFieldPrefab, parent, false);

    public static UIDynamic NewSpacer(
        this MVRScript script,
        float height,
        bool rightSide = false
    )
    {
        if(height <= 0)
        {
            return null;
        }

        var spacer = script.CreateSpacer(rightSide);
        spacer.height = height;
        return spacer;
    }

    public static void RemoveElement(this MVRScript script, UIDynamic element)
    {
        try
        {
            if(element is UIDynamicTextField)
            {
                script.RemoveTextField((UIDynamicTextField) element);
            }
            else if(element is UIDynamicButton)
            {
                script.RemoveButton((UIDynamicButton) element);
            }
            else if(element is UIDynamicSlider)
            {
                script.RemoveSlider((UIDynamicSlider) element);
            }
            else if(element is UIDynamicToggle)
            {
                script.RemoveToggle((UIDynamicToggle) element);
            }
            else if(element is UIDynamicPopup)
            {
                script.RemovePopup((UIDynamicPopup) element);
            }
            else if(element is UIDynamicColorPicker)
            {
                script.RemoveColorPicker((UIDynamicColorPicker) element);
            }
            else
            {
                script.RemoveSpacer(element);
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"RemoveElement failed for {element.name}: {e}");
        }
    }
}

static class StringExtensions
{
    public static string Bold(this string str) => $"<b>{str}</b>";

    public static string Size(this string str, int size) => $"<size={size}>{str}</size>";
}

static class StringBuilderExtensions
{
    public static StringBuilder Clear(this StringBuilder sb)
    {
        sb.Length = 0;
        return sb;
    }
}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
static class UIDynamicExtensions
{
    public static void AddListener(this UIDynamic uiDynamic, UnityAction<bool> callback)
    {
        if(!uiDynamic)
        {
            return;
        }

        var uiDynamicToggle = uiDynamic as UIDynamicToggle;
        if(!uiDynamicToggle)
        {
            throw new ArgumentException($"UIDynamic {uiDynamic.name} was null or not an UIDynamicToggle");
        }

        uiDynamicToggle.toggle.onValueChanged.AddListener(callback);
    }

    public static void AddListener(this UIDynamic uiDynamic, UnityAction callback)
    {
        if(!uiDynamic)
        {
            return;
        }

        var uiDynamicButton = uiDynamic as UIDynamicButton;
        if(!uiDynamicButton)
        {
            throw new ArgumentException($"UIDynamic {uiDynamic.name} was null or not an UIDynamicButton");
        }

        uiDynamicButton.button.onClick.AddListener(callback);
    }

    public static void AddListener(this UIDynamic uiDynamic, UnityAction<float> callback)
    {
        if(!uiDynamic)
        {
            return;
        }

        var uiDynamicSlider = uiDynamic as UIDynamicSlider;
        if(!uiDynamicSlider)
        {
            throw new ArgumentException($"UIDynamic {uiDynamic.name} was null or not an UIDynamicSlider");
        }

        uiDynamicSlider.slider.onValueChanged.AddListener(callback);
    }

    public static void SetActiveStyle(this UIDynamic uiDynamic, bool isActive, bool setInteractable = false, bool highlightIneffective = false)
    {
        if(!uiDynamic)
        {
            return;
        }

        var color = isActive ? Color.black : Colors.inactive;
        if(uiDynamic is UIDynamicSlider)
        {
            var uiDynamicSlider = (UIDynamicSlider) uiDynamic;
            uiDynamicSlider.slider.interactable = !setInteractable || isActive;
            uiDynamicSlider.quickButtonsEnabled = !setInteractable || isActive;
            uiDynamicSlider.defaultButtonEnabled = !setInteractable || isActive;
            uiDynamicSlider.labelText.color = color;
        }
        else if(uiDynamic is UIDynamicToggle)
        {
            var uiDynamicToggle = (UIDynamicToggle) uiDynamic;
            uiDynamicToggle.toggle.interactable = !setInteractable || isActive;
            if(highlightIneffective && uiDynamicToggle.toggle.isOn && uiDynamicToggle.toggle.interactable)
            {
                color = isActive ? Color.black : Color.red;
            }

            uiDynamicToggle.labelText.color = color;
        }
        else if(uiDynamic is UIDynamicButton)
        {
            var uiDynamicButton = (UIDynamicButton) uiDynamic;
            uiDynamicButton.button.interactable = !setInteractable || isActive;
            var colors = uiDynamicButton.button.colors;
            colors.disabledColor = colors.normalColor;
            uiDynamicButton.button.colors = colors;
            uiDynamicButton.textColor = color;
        }
        else if(uiDynamic is UIDynamicPopup)
        {
            var uiDynamicPopup = (UIDynamicPopup) uiDynamic;
            uiDynamicPopup.SetInteractable(!setInteractable || isActive);
        }
        else if(uiDynamic is UIDynamicTextField)
        {
            var uiDynamicTextField = (UIDynamicTextField) uiDynamic;
            uiDynamicTextField.textColor = color;
        }
        else
        {
            throw new ArgumentException($"UIDynamic {uiDynamic.name} was null, or not an expected type");
        }
    }
}

static class UIDynamicPopupExtensions
{
    public static void SetInteractable(this UIDynamicPopup popup, bool interactable)
    {
        var slider = popup.GetComponentInChildren<Slider>();
        if(slider)
        {
            slider.interactable = interactable;
        }

        var button = popup.GetComponentInChildren<Button>();
        if(button)
        {
            button.interactable = interactable;
        }
    }
}

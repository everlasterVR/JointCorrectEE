using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;

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

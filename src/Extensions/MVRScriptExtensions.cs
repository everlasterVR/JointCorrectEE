using System.Diagnostics.CodeAnalysis;
using UnityEngine;

static class MVRScriptExtensions
{
    public static Transform InstantiateTextField(this MVRScript script, Transform parent = null) =>
        Object.Instantiate(script.manager.configurableTextFieldPrefab, parent, false);

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static JSONStorableString NewJSONStorableString(
        this MVRScript script,
        string paramName,
        string startingValue,
        bool shouldRegister = true
    )
    {
        var storable = new JSONStorableString(paramName, startingValue)
        {
            storeType = JSONStorableParam.StoreType.Full,
        };
        if(shouldRegister)
        {
            script.RegisterString(storable);
        }

        return storable;
    }

    public static JSONStorableBool NewJSONStorableBool(
        this MVRScript script,
        string paramName,
        bool startingValue,
        bool shouldRegister = true
    )
    {
        var storable = new JSONStorableBool(paramName, startingValue)
        {
            storeType = JSONStorableParam.StoreType.Full,
        };
        if(shouldRegister)
        {
            script.RegisterBool(storable);
        }

        return storable;
    }

    public static JSONStorableFloat NewJSONStorableFloat(
        this MVRScript script,
        string paramName,
        float startingValue,
        float minValue,
        float maxValue,
        bool shouldRegister = true
    )
    {
        var storable = new JSONStorableFloat(paramName, startingValue, minValue, maxValue);
        storable.storeType = JSONStorableParam.StoreType.Full;
        if(shouldRegister)
        {
            script.RegisterFloat(storable);
        }

        return storable;
    }

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
}

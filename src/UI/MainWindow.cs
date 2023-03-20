﻿using System.Linq;
using UnityEngine;

sealed class MainWindow : WindowBase
{
    public MainWindow() : base(JointCorrectEE.script, nameof(MainWindow))
    {
    }

    protected override void OnBuild()
    {
        BuildLeftSide();
        BuildRightSide();
    }

    void BuildLeftSide(bool rightSide = false)
    {
        AddSpacer(80, rightSide);

        foreach(var config in JointCorrectEE.boneConfigs.Take(7))
        {
            AddElement(() => MultiplierSlider(config.multiplierJsf, rightSide));
        }

        AddElement(
            () =>
            {
                var toggle = script.CreateToggle(JointCorrectEE.script.disableCollarBreastJsb, rightSide);
                toggle.label = "Disable Collar Bone Morphs Affecting Breast Vertices";
                toggle.height = 80;
                return toggle;
            }
        );
    }

    void BuildRightSide(bool rightSide = true)
    {
        AddInfoTextField(
            "\n".Size(12) +
            "Using morphs from package FallenDancer.JointCorrect.11.var.",
            rightSide,
            height: 80
        );

        foreach(var config in JointCorrectEE.boneConfigs.Skip(7))
        {
            AddElement(() => MultiplierSlider(config.multiplierJsf, rightSide));
        }

        /* Version text field */
        {
            var versionJss = new JSONStorableString("version", "");
            var versionTextField = CreateVersionTextField(versionJss);
            AddElement(versionTextField);
            JointCorrectEE.script.AddTextFieldToJss(versionTextField, versionJss);
        }
    }

    UIDynamicSlider MultiplierSlider(JSONStorableFloat storable, bool rightSide)
    {
        var slider = script.CreateSlider(storable, rightSide);
        slider.valueFormat = "F3";
        slider.label = storable.name;
        if(storable.name == "Genitals" && !JointCorrectEE.person.isFemale)
        {
            slider.label += " (disabled on Male)";
        }
        return slider;
    }

    public void UpdateGenitalsSlider()
    {
        var uiDynamicSlider = GetElementAs<UIDynamicSlider>("Genitals");
        if(uiDynamicSlider != null)
        {
            string label = "Genitals";
            if(!JointCorrectEE.person.isFemale)
            {
                label += " (disabled on Male)";
            }

            uiDynamicSlider.label = label;
        }
    }
}

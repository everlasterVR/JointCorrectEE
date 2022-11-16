﻿using UnityEngine;
using static JointCorrectEE;

public class MainWindow : WindowBase
{
    public MainWindow()
    {
    }

    protected override void OnBuild()
    {
        CreateTitleTextField(
            new JSONStorableString("title", "\n".Size(24) + $"{nameof(JointCorrectEE)}    v{VERSION}".Bold()),
            fontSize: 40,
            height: 100,
            rightSide: false
        );

        /* Locked toggle */
        {
            var storable = jointCorrectEE.locked;
            var toggle = jointCorrectEE.CreateToggle(storable, true);
            toggle.height = 52;
            toggle.label = "Locked";
            elements[storable.name] = toggle;
        }

        AddSpacer("lockedFiller", 57, true);
        AddSpacer("lockedFiller", 10);

        for(int i = 0; i < boneConfigs.Count; i++)
        {
            var config = boneConfigs[i];
            var storable = config.multiplierJsf;
            bool rightSide = i > 6;
            var slider = jointCorrectEE.CreateSlider(storable, rightSide);
            slider.valueFormat = "F3";
            slider.label = storable.name;
            elements[storable.name] = slider;
        }

        /* Info */
        {
            var storable = new JSONStorableString("infoText", "");
            storable.val = "\n".Size(12) + "Morphs from package FallenDancer.JointCorrect.11.var.";
            var textField = jointCorrectEE.CreateTextField(storable);
            textField.UItext.fontSize = 28;
            textField.backgroundColor = Color.clear;
            textField.height = 100;
        }
    }
}

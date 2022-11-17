using UnityEngine;
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

        /* Info */
        {
            var storable = new JSONStorableString("infoText", "");
            storable.val = "\n".Size(12) + "Using morphs from package FallenDancer.JointCorrect.11.var.";
            var textField = jointCorrectEE.CreateTextField(storable, true);
            textField.UItext.fontSize = 28;
            textField.backgroundColor = Color.clear;
            textField.height = 100;
            elements[storable.name] = textField;
        }

        AddSpacer("fillerLeft", 10);
        AddSpacer("fillerRight", 10, true);

        for(int i = 0; i < boneConfigs.Count; i++)
        {
            var config = boneConfigs[i];
            var storable = config.multiplierJsf;
            bool rightSide = i > 6;
            var slider = jointCorrectEE.CreateSlider(storable, rightSide);
            slider.valueFormat = "F3";
            slider.label = storable.name;
            elements[storable.name] = slider;

            if(storable.name == "Genitals")
            {
                if(geometry.selectedCharacter.isMale)
                {
                    slider.label += " (disabled on Male)";
                }
            }
        }
    }

    public void UpdateGenitalsSlider()
    {
        var uiDynamicSlider = elements["Genitals"] as UIDynamicSlider;
        if(uiDynamicSlider != null)
        {
            string label = "Genitals";
            if(geometry.selectedCharacter.isMale)
            {
                label += " (disabled on Male)";
            }

            uiDynamicSlider.label = label;
        }
    }
}

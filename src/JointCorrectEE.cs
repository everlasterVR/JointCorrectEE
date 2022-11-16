using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class JointCorrectEE : MVRScript
{
    public const string VERSION = "0.0.0";
    public static bool envIsDevelopment => VERSION.StartsWith("0.");

    private bool _initialized;

    public override void Init()
    {
        try
        {
            /* Used to store version in save JSON. */
            var versionJss = this.NewJSONStorableString("version", "");
            versionJss.val = $"{VERSION}";
            if(containingAtom.type != "Person")
            {
                Utils.LogError($"Add to Person atom, not {containingAtom.type}");
            }

            if(Utils.PluginIsDuplicate(containingAtom, storeId))
            {
                Utils.LogError($"Person already has an instance of {nameof(JointCorrectEE)}.");
                enabled = false;
                return;
            }

            StartCoroutine(DeferInit());
        }
        catch(Exception e)
        {
            Utils.LogError($"Init: {e}");
        }
    }

    private DAZCharacterSelector _geometry;
    private static Atom person { get; set; }
    public static GenerateDAZMorphsControlUI morphsControlUI { get; private set; }

    private List<BoneConfig> _boneConfigs;
    private JSONStorableBool _locked;

    private LineRenderer _line;

    private IEnumerator DeferInit()
    {
        yield return new WaitForEndOfFrame();
        while(SuperController.singleton.isLoading)
        {
            yield return null;
        }

        person = containingAtom;
        _geometry = (DAZCharacterSelector) person.GetStorableByID("geometry");
        morphsControlUI = _geometry.morphsControlUI;

        /* TODO UI */
        _locked = this.NewJSONStorableBool("Lock", false);
        CreateToggle(_locked, true);

        if(envIsDevelopment)
        {
            _line = gameObject.GetComponent<LineRenderer>();
            if(_line == null)
            {
                _line = gameObject.AddComponent<LineRenderer>();
            }

            _line.positionCount = 5;
            _line.startColor = Color.blue;
            _line.endColor = Color.blue;
            _line.startWidth = 0.04f;
            _line.endWidth = 0.00f;
        }

        InitMorphs();
        _initialized = true;
    }

    public void InitMorphs()
    {
        _boneConfigs = new List<BoneConfig>();

        /* Collars */
        {
            var left = GetBone("lCollar");
            var lCollarXn025 = NewMorphConfig("LCollarX-025");
            var lCollarXp015 = NewMorphConfig("LCollarX+015");
            var lCollarYn026 = NewMorphConfig("LCollarY-026");
            var lCollarYp017 = NewMorphConfig("LCollarY+017");
            var lCollarZn015 = NewMorphConfig("LCollarZ-015");
            var lCollarZp050 = NewMorphConfig("LCollarZ+050");

            var right = GetBone("rCollar");
            var rCollarXn025 = NewMorphConfig("RCollarX-025");
            var rCollarXp015 = NewMorphConfig("RCollarX+015");
            var rCollarYn017 = NewMorphConfig("RCollarY-017");
            var rCollarYp026 = NewMorphConfig("RCollarY+026");
            var rCollarZn050 = NewMorphConfig("RCollarZ-050");
            var rCollarZp015 = NewMorphConfig("RCollarZ+015");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lCollarXn025,
                    lCollarXp015,
                    lCollarYn026,
                    lCollarYp017,
                    lCollarZn015,
                    lCollarZp050,
                    rCollarXn025,
                    rCollarXp015,
                    rCollarYn017,
                    rCollarYp026,
                    rCollarZn050,
                    rCollarZp015,
                },
                multiplierJsf = this.NewJSONStorableFloat("Collars", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = left.GetAnglesDegrees();
                    lCollarXn025.Update(lAngles.x, 0, -25);
                    lCollarXp015.Update(lAngles.x, 0, 15);

                    // Note: Y Inverted
                    lCollarYn026.Update(lAngles.y, 0, 26);
                    lCollarYp017.Update(lAngles.y, 0, -17);

                    // Note: Rotation order not honored, Z Inverted
                    lCollarZn015.Update(lAngles.z, 0, 15);
                    lCollarZp050.Update(lAngles.z, 0, -50);

                    var rAngles = right.GetAnglesDegrees();
                    // Note: Rotation order not honored
                    rCollarXn025.Update(rAngles.x, 0, -25);
                    rCollarXp015.Update(rAngles.x, 0, 15);

                    // Note: Y Inverted
                    rCollarYn017.Update(rAngles.y, 0, 17);
                    rCollarYp026.Update(rAngles.y, 0, -26);

                    // Note: Rotation order not honored, Z Inverted
                    rCollarZn050.Update(rAngles.z, 0, 50);
                    rCollarZp015.Update(rAngles.z, 0, -15);
                },
            });
        }

        /* Feet */
        {
            var left = GetBone("lFoot");
            var lFootXp065 = NewMorphConfig("LFootX+065");
            var lFootXn040 = NewMorphConfig("LFootX-040");

            var right = GetBone("rFoot");
            var rFootXp065 = NewMorphConfig("RFootX+065");
            var rFootXn040 = NewMorphConfig("RFootX-040");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lFootXp065,
                    lFootXn040,
                    rFootXp065,
                    rFootXn040,
                },
                multiplierJsf = this.NewJSONStorableFloat("Feet", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngls = left.GetAnglesDegrees();
                    lFootXn040.Update(lAngls.x, 0, -40);
                    lFootXp065.Update(lAngls.x, 0, 65);

                    var rAngles = right.GetAnglesDegrees();
                    rFootXn040.Update(rAngles.x, 0, -40);
                    rFootXp065.Update(rAngles.x, 0, 65);
                },
            });
        }

        /* Forearms */
        {
            var lForearmBone = GetBone("lForeArm");
            var lForearmYn100 = NewMorphConfig("LForearmY-100");
            var lForearmYn130 = NewMorphConfig("LForearmY-130");

            var rForearmBone = GetBone("rForeArm");
            var rForearmYp100 = NewMorphConfig("RForearmY+100");
            var rForearmYp130 = NewMorphConfig("RForearmY+130");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lForearmYn100,
                    lForearmYn130,
                    rForearmYp100,
                    rForearmYp130,
                },
                multiplierJsf = this.NewJSONStorableFloat("Forearms", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = lForearmBone.GetAnglesDegrees();
                    // Note: Y Inverted
                    lForearmYn100.Update(lAngles.y, 0, 100);
                    lForearmYn130.Update(lAngles.y, 100, 130);

                    var rAngles = rForearmBone.GetAnglesDegrees();
                    // Note: Y Inverted
                    rForearmYp100.Update(rAngles.y, 0, -100);
                    rForearmYp130.Update(rAngles.y, -100, -130);
                },
            });
        }

        /* Hands */
        {
            var left = GetBone("lHand");
            var lHandZp080 = NewMorphConfig("LHandZ+080");

            var right = GetBone("rHand");
            var rHandZn080 = NewMorphConfig("RHandZ-080");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lHandZp080,
                    rHandZn080,
                },
                multiplierJsf = this.NewJSONStorableFloat("Hands", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = left.GetAnglesDegrees();
                    lHandZp080.Update(lAngles.z, 0, -80);

                    var rAngles = right.GetAnglesDegrees();
                    rHandZn080.Update(rAngles.z, 0, 80);
                },
            });
        }

        /* Shins */
        {
            var left = GetBone("lShin");
            var lShinXp085 = NewMorphConfig("LShinX+085");
            var lShinXp140 = NewMorphConfig("LShinX+140");

            var right = GetBone("rShin");
            var rShinXp085 = NewMorphConfig("RShinX+085");
            var rShinXp140 = NewMorphConfig("RShinX+140");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lShinXp085,
                    lShinXp140,
                    rShinXp085,
                    rShinXp140,
                },
                multiplierJsf = this.NewJSONStorableFloat("Shins", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = left.GetAnglesDegrees();
                    lShinXp085.Update(lAngles.x, 0, 85);
                    lShinXp140.Update(lAngles.x, 85, 140);

                    var rAngles = right.GetAnglesDegrees();
                    rShinXp085.Update(rAngles.x, 0, 85);
                    rShinXp140.Update(rAngles.x, 85, 140);
                },
            });
        }

        /* Shoulders */
        {
            var left = GetBone("lShldr");
            var lShldrZp035 = NewMorphConfig("LShldrZ+035");
            var lShldrZn060 = NewMorphConfig("LShldrZ-060");
            var lShldrZn075 = NewMorphConfig("LShldrZ-075");
            var lShldrYn095 = NewMorphConfig("LShldrY-095");
            var lShldrYp040 = NewMorphConfig("LShldrY+040");

            var right = GetBone("rShldr");
            var rShldrZn035 = NewMorphConfig("RShldrZ-035");
            var rShldrZp060 = NewMorphConfig("RShldrZ+060");
            var rShldrZp075 = NewMorphConfig("RShldrZ+075");
            var rShldrYn040 = NewMorphConfig("RShldrY-040");
            var rShldrYp095 = NewMorphConfig("RShldrY+095");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lShldrZp035,
                    lShldrZn060,
                    lShldrZn075,
                    lShldrYn095,
                    lShldrYp040,
                    rShldrZn035,
                    rShldrZp060,
                    rShldrZp075,
                    rShldrYn040,
                    rShldrYp095,
                },
                multiplierJsf = this.NewJSONStorableFloat("Shoulders", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = left.GetAnglesDegrees();
                    // Note: Y Inverted
                    lShldrYn095.Update(lAngles.y, 0, 95);
                    lShldrYp040.Update(lAngles.y, 0, -40);
                    // Remove -Z as Y approaches -40
                    float zcorLeft = lAngles.z * -Utils.NormalizeFloat(lAngles.y, 0, -40);
                    // Note: DAZ Z = VaM Z Inverted
                    lShldrZp035.Update(lAngles.z, 0, -35);
                    lShldrZn060.Update(lAngles.z + zcorLeft, 0, 60);
                    lShldrZn075.Update(lAngles.z + zcorLeft, 60, 75);

                    var rAngles = right.GetAnglesDegrees();
                    // Note: Y probably Inverted
                    rShldrYn040.Update(rAngles.y, 0, 40);
                    rShldrYp095.Update(rAngles.y, 0, -95);
                    // Remove +Z as Y approaches 40
                    float zcorRight = rAngles.z * -Utils.NormalizeFloat(rAngles.y, 0, 40);
                    // Note: DAZ Z = VaM Z Inverted
                    rShldrZn035.Update(rAngles.z, 0, 35);
                    rShldrZp060.Update(rAngles.z + zcorRight, 0, -60);
                    rShldrZp075.Update(rAngles.z + zcorRight, -60, -75);
                },
            });
        }

        /* Thighs */
        {
            var left = GetBone("lThigh");
            var lThighXp035 = NewMorphConfig("LThighX+035");
            var lThighXn055 = NewMorphConfig("LThighX-055");
            var lThighXn115 = NewMorphConfig("LThighX-115");
            var lThighYp075 = NewMorphConfig("LThighY+075");
            var lThighYn075 = NewMorphConfig("LThighY-075");
            var lThighZp085 = NewMorphConfig("LThighZ+085");
            var lThighZn015 = NewMorphConfig("LThighZ-015");

            var right = GetBone("rThigh");
            var rThighXp035 = NewMorphConfig("RThighX+035");
            var rThighXn055 = NewMorphConfig("RThighX-055");
            var rThighXn115 = NewMorphConfig("RThighX-115");
            var rThighYn075 = NewMorphConfig("RThighY-075");
            var rThighYp075 = NewMorphConfig("RThighY+075");
            var rThighZn085 = NewMorphConfig("RThighZ-085");
            var rThighZp015 = NewMorphConfig("RThighZ+015");

            var cThighZp180 = NewMorphConfig("CThighsZ+180");
            var cThighXn115 = NewMorphConfig("CThighsX-115");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lThighXp035,
                    lThighXn055,
                    lThighXn115,
                    lThighYp075,
                    lThighYn075,
                    lThighZp085,
                    lThighZn015,
                    rThighXp035,
                    rThighXn055,
                    rThighXn115,
                    rThighYn075,
                    rThighYp075,
                    rThighZn085,
                    rThighZp015,
                    cThighZp180,
                    cThighXn115,
                },
                multiplierJsf = this.NewJSONStorableFloat("Thighs", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = left.GetAnglesDegrees();
                    lThighXn115.Update(lAngles.x, -55, -115);
                    lThighXn055.Update(lAngles.x, 0, -55);
                    lThighXp035.Update(lAngles.x, 0, 35);

                    // Note: DAZ Y = VAM Y Inverted
                    lThighYn075.Update(lAngles.y, 0, 75);
                    lThighYp075.Update(lAngles.y, 0, -75);

                    // Note: DAZ Z = VAM Z Inverted
                    lThighZn015.Update(lAngles.z, 0, 15);
                    lThighZp085.Update(lAngles.z, 0, -85);

                    var rAngles = right.GetAnglesDegrees();
                    rThighXn115.Update(rAngles.x, -55, -115);
                    rThighXn055.Update(rAngles.x, 0, -55);
                    rThighXp035.Update(rAngles.x, 0, 35);

                    // Note: DAZ Y = VAM Y Inverted
                    rThighYn075.Update(rAngles.y, 0, 75);
                    rThighYp075.Update(rAngles.y, 0, -75);

                    // Note: DAZ Z = VAM Z Inverted
                    rThighZn085.Update(rAngles.z, 0, 85);
                    rThighZp015.Update(rAngles.z, 0, -15);

                    float thighZSeparation = rAngles.z - lAngles.z;
                    cThighZp180.Update(thighZSeparation, 0, 180);

                    float thighXCombination = (lAngles.x + rAngles.x) / 2;
                    cThighXn115.Update(thighXCombination, -55, -115);
                },
            });
        }

        /* Genitals */
        {
            var left = GetBone("lThigh");
            var lThighZp085Gens = NewMorphConfig("LThighZ+085.gens");
            // var lThighZn015gens = GetMorph("LThighZ-015.gens"); // unused

            var right = GetBone("rThigh");
            var rThighZn085Gens = NewMorphConfig("RThighZ-085.gens");

            // var cThighZ180gens = GetMorph("CThighsZ180.gens"); // unused
            var cThighZp180Gens = NewMorphConfig("CThighsZ+180.gens");
            var cThighZn030Gens = NewMorphConfig("CThighsZ-030.gens");
            var cThighXn115Gens = NewMorphConfig("CThighsX-115.gens");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    lThighZp085Gens,
                    rThighZn085Gens,
                    cThighZp180Gens,
                    cThighZn030Gens,
                    cThighXn115Gens,
                },
                multiplierJsf = this.NewJSONStorableFloat("Genitals", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = left.GetAnglesDegrees();
                    // Note: DAZ Z = VAM Z Inverted
                    lThighZp085Gens.Update(lAngles.z, 0, -85);

                    var rAngles = right.GetAnglesDegrees();
                    // Note: DAZ Z = VAM Z Inverted
                    rThighZn085Gens.Update(rAngles.z, 0, 85);

                    float thighZSeparation = rAngles.z - lAngles.z;
                    cThighZp180Gens.Update(thighZSeparation, 0, 180);
                    cThighZn030Gens.Update(thighZSeparation, 0, -30);

                    float thighXCombination = (lAngles.x + rAngles.x) / 2;
                    cThighXn115Gens.Update(thighXCombination, -55, -115);
                },
            });
        }

        /* Pelvis */
        {
            var bone = GetBone("pelvis");
            var pelvisXp030 = NewMorphConfig("TPelvisX+030");
            var pelvisXn015 = NewMorphConfig("TPelvisX-015");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    pelvisXp030,
                    pelvisXn015,
                },
                multiplierJsf = this.NewJSONStorableFloat("Pelvis", 1, 0, 2),
                driver = multiplier =>
                {
                    var angles = bone.GetAnglesDegrees();
                    pelvisXn015.Update(angles.x, 0, -15);
                    pelvisXp030.Update(angles.x, 0, 30);
                },
            });
        }

        /* Abdomen */
        {
            var bone = GetBone("abdomen");
            var abdomenXn020 = NewMorphConfig("TAbdomenX-020");
            var abdomenXp030 = NewMorphConfig("TAbdomenX+030");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    abdomenXn020,
                    abdomenXp030,
                },
                multiplierJsf = this.NewJSONStorableFloat("Abdomen", 1, 0, 2),
                driver = multiplier =>
                {
                    var angles = bone.GetAnglesDegrees();
                    abdomenXn020.Update(angles.x, 0, -20);
                    abdomenXp030.Update(angles.x, 0, 30);
                },
            });
        }

        /* Abdomen2 */
        {
            var bone = GetBone("abdomen2");
            var abdomen2Xn020 = NewMorphConfig("TAbdomen2X-020");
            var abdomen2Xp030 = NewMorphConfig("TAbdomen2X+030");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    abdomen2Xn020,
                    abdomen2Xp030,
                },
                multiplierJsf = this.NewJSONStorableFloat("Abdomen2", 1, 0, 2),
                driver = multiplier =>
                {
                    var angles = bone.GetAnglesDegrees();
                    abdomen2Xn020.Update(angles.x, 0, -20);
                    abdomen2Xp030.Update(angles.x, 0, 30);
                },
            });
        }

        /* Chest */
        {
            var bone = GetBone("chest");
            var chestXp020 = NewMorphConfig("TChestX+020");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    chestXp020,
                },
                multiplierJsf = this.NewJSONStorableFloat("Chest", 1, 0, 2),
                driver = multiplier =>
                {
                    var angles = bone.GetAnglesDegrees();
                    chestXp020.Update(angles.x, 0, 20);
                },
            });
        }

        /* Neck */
        {
            var bone = GetBone("neck");
            var neckXn030 = NewMorphConfig("TNeckX-030");
            var neckYp035 = NewMorphConfig("TNeckY+035");
            var neckYn035 = NewMorphConfig("TNeckY-035");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    neckXn030,
                    neckYp035,
                    neckYn035,
                },
                multiplierJsf = this.NewJSONStorableFloat("Neck", 1, 0, 2),
                driver = multiplier =>
                {
                    var angles = bone.GetAnglesDegrees();
                    neckXn030.Update(angles.x, 0, -30);
                    neckYp035.Update(angles.y, 0, -35);
                    neckYn035.Update(angles.y, 0, 35);
                },
            });
        }

        /* Head */
        {
            var bone = GetBone("head");
            var headXn045 = NewMorphConfig("THeadX-045");
            var headXp035 = NewMorphConfig("THeadX+035");

            _boneConfigs.Add(new BoneConfig
            {
                morphConfigs = new List<MorphConfig>
                {
                    headXn045,
                    headXp035,
                },
                multiplierJsf = this.NewJSONStorableFloat("Head", 1, 0, 2),
                driver = multiplier =>
                {
                    var angles = bone.GetAnglesDegrees();
                    headXn045.Update(angles.x, 0, -45);
                    headXp035.Update(angles.x, 0, 35);
                },
            });
        }

        foreach(var config in _boneConfigs)
        {
            config.SetGroupMultiplierReferences();
        }
    }

    private static MorphConfig NewMorphConfig(string morphName) =>
        new MorphConfig(morphName, new JSONStorableFloat($"{morphName}", 1, 0, 2));

    public void Update()
    {
        if(!_initialized || _locked.val)
        {
            return;
        }

        try
        {
            foreach(var config in _boneConfigs)
            {
                config.Update();
            }
        }
        catch(Exception e)
        {
            Utils.LogError($"Update: {e}");
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void DrawAxes(Quaternion rotation, Vector3 position)
    {
        const float length = 0.25f;

        _line.positionCount = 5;

        _line.startColor = Color.green;
        _line.endColor = Color.blue;

        _line.SetPosition(0, position + (rotation * Vector3.up).normalized * length);
        _line.SetPosition(1, position);
        _line.SetPosition(2, position + (rotation * Vector3.right).normalized * length);
        _line.SetPosition(3, position);
        _line.SetPosition(4, position + (rotation * Vector3.forward).normalized * length);
    }

    private DAZBone GetBone(string jointName) =>
        containingAtom.GetStorableByID(jointName).transform.GetComponent<DAZBone>();

    public override void RestoreFromJSON(
        JSONClass jsonClass,
        bool restorePhysical = true,
        bool restoreAppearance = true,
        JSONArray presetAtoms = null,
        bool setMissingToDefault = true
    )
    {
        if(jsonClass.HasKey("version"))
        {
            jsonClass["version"] = $"{VERSION}";
        }

        StartCoroutine(DeferRestoreFromJSON(
            jsonClass,
            restorePhysical,
            restoreAppearance,
            presetAtoms,
            setMissingToDefault
        ));
    }

    private IEnumerator DeferRestoreFromJSON(
        JSONClass jsonClass,
        bool restorePhysical,
        bool restoreAppearance,
        JSONArray presetAtoms,
        bool setMissingToDefault
    )
    {
        while(!_initialized)
        {
            yield return null;
        }

        base.RestoreFromJSON(jsonClass, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);
    }

    public void OnEnable()
    {
        if(!_initialized)
        {
            return;
        }

        try
        {
        }
        catch(Exception e)
        {
            Utils.LogError($"OnEnable: {e}");
        }
    }

    public void OnDisable()
    {
        try
        {
            foreach(var config in _boneConfigs)
            {
                config.Reset();
            }
        }
        catch(Exception e)
        {
            if(_initialized)
            {
                Utils.LogError($"OnDisable: {e}");
            }
            else if(envIsDevelopment)
            {
                Utils.Log($"OnDisable: {e}");
            }
        }
    }

    public void OnDestroy()
    {
        try
        {
            person = null;
            morphsControlUI = null;
        }
        catch(Exception e)
        {
            if(_initialized)
            {
                Utils.LogError($"OnDestroy: {e}");
            }
            else if(envIsDevelopment)
            {
                Utils.Log($"OnDestroy: {e}");
            }
        }
    }
}

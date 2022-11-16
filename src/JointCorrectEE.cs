using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;
using UnityEngine;

public class JointCorrectEE : MVRScript
{
    public const string VERSION = "0.0.0";
    public static bool envIsDevelopment => VERSION.StartsWith("0.");

    private MorphConfig
        _lFootXp065,
        _lFootXn040,
        _rFootXp065,
        _rFootXn040,
        _lShinXp085,
        _lShinXp140,
        _rShinXp085,
        _rShinXp140,
        _lThighXp035,
        _lThighXn055,
        _lThighXn115,
        _lThighYp075,
        _lThighYn075,
        _lThighZp085,
        _lThighZp085Gens,
        _lThighZn015,
        // _lThighZn015Gens,
        _rThighXp035,
        _rThighXn055,
        _rThighXn115,
        _rThighYn075,
        _rThighYp075,
        _rThighZn085,
        _rThighZn085Gens,
        _rThighZp015,
        _lHandZp080,
        _rHandZn080,
        _lForearmYn100,
        _lForearmYn130,
        _rForearmYp100,
        _rForearmYp130,
        _lShldrYn095,
        _lShldrYp040,
        _lShldrZn075,
        _lShldrZn060,
        _lShldrZp035,
        _rShldrYp095,
        _rShldrYn040,
        _rShldrZp075,
        _rShldrZp060,
        _rShldrZn035,
        _lCollarXn025,
        _lCollarXp015,
        _rCollarXn025,
        _rCollarXp015,
        _lCollarYn026,
        _lCollarYp017,
        _rCollarYn017,
        _rCollarYp026,
        _lCollarZn015,
        _lCollarZp050,
        _rCollarZn050,
        _rCollarZp015,
        _pelvisXp030,
        _pelvisXn015,
        _abdomenXn020,
        _abdomenXp030,
        _abdomen2Xn020,
        _abdomen2Xp030,
        _chestXp020,
        _neckXn030,
        _neckYp035,
        _neckYn035,
        _headXn045,
        _headXp035,
        // _cThighZ180Gens,
        _cThighXn115,
        _cThighXn115Gens,
        _cThighZp180,
        _cThighZp180Gens,
        _cThighZn030Gens;

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
    public static GenerateDAZMorphsControlUI morphsControlUI { get; private set; }

    private readonly List<MorphConfig> _morphConfigs = new List<MorphConfig>();
    private readonly List<Action> _drivers = new List<Action>();

    private readonly StringBuilder _rLog = new StringBuilder();
    private readonly StringBuilder _mLog = new StringBuilder();
    private JSONStorableString _morphOut;
    private JSONStorableString _jointOut;
    private JSONStorableBool _locked;

    private LineRenderer _line;

    private IEnumerator DeferInit()
    {
        yield return new WaitForEndOfFrame();
        while(SuperController.singleton.isLoading)
        {
            yield return null;
        }

        _geometry = (DAZCharacterSelector) containingAtom.GetStorableByID("geometry");
        morphsControlUI = _geometry.morphsControlUI;

        /* TODO UI */

        InitMorphs();
        _initialized = true;
    }

    public void InitMorphs()
    {
        _locked = this.NewJSONStorableBool("Lock", false);
        CreateToggle(_locked, true);

        if(envIsDevelopment)
        {
            _morphOut = new JSONStorableString("morphout", "");
            var dbugField = CreateTextField(_morphOut);
            dbugField.height *= 4;

            _jointOut = new JSONStorableString("jointout", "");
            var joutField = CreateTextField(_jointOut, true);
            joutField.height *= 4;
        }

        #region Collar

        var lCollarBone = GetJoint("lCollar").GetComponent<DAZBone>();
        _lCollarXn025 = GetMorph("LCollarX-025");
        _lCollarXp015 = GetMorph("LCollarX+015");
        _lCollarYn026 = GetMorph("LCollarY-026");
        _lCollarYp017 = GetMorph("LCollarY+017");
        _lCollarZn015 = GetMorph("LCollarZ-015");
        _lCollarZp050 = GetMorph("LCollarZ+050");
        _drivers.Add(() =>
        {
            var angles = lCollarBone.GetAnglesDegrees();

            SetMorph(_lCollarXn025, angles.x, 0, -25);
            SetMorph(_lCollarXp015, angles.x, 0, 15);

            // Note: Y Inverted
            SetMorph(_lCollarYn026, angles.y, 0, 26);
            SetMorph(_lCollarYp017, angles.y, 0, -17);

            // Note: Rotation order not honored, Z Inverted
            SetMorph(_lCollarZn015, angles.z, 0, 15);
            SetMorph(_lCollarZp050, angles.z, 0, -50);
        });

        var rCollarBone = GetJoint("rCollar").GetComponent<DAZBone>();
        _rCollarXn025 = GetMorph("RCollarX-025");
        _rCollarXp015 = GetMorph("RCollarX+015");
        _rCollarYn017 = GetMorph("RCollarY-017");
        _rCollarYp026 = GetMorph("RCollarY+026");
        _rCollarZn050 = GetMorph("RCollarZ-050");
        _rCollarZp015 = GetMorph("RCollarZ+015");
        _drivers.Add(() =>
        {
            var angles = rCollarBone.GetAnglesDegrees();

            // Note: Rotation order not honored
            SetMorph(_rCollarXn025, angles.x, 0, -25);
            SetMorph(_rCollarXp015, angles.x, 0, 15);

            // Note: Y Inverted
            SetMorph(_rCollarYn017, angles.y, 0, 17);
            SetMorph(_rCollarYp026, angles.y, 0, -26);

            // Note: Rotation order not honored, Z Inverted
            SetMorph(_rCollarZn050, angles.z, 0, 50);
            SetMorph(_rCollarZp015, angles.z, 0, -15);
        });

        #endregion

        #region Foot

        var lFootBone = GetJoint("lFoot").GetComponent<DAZBone>();
        _lFootXp065 = GetMorph("LFootX+065");
        _lFootXn040 = GetMorph("LFootX-040");
        _drivers.Add(() =>
        {
            var angles = lFootBone.GetAnglesDegrees();
            SetMorph(_lFootXn040, angles.x, 0, -40);
            SetMorph(_lFootXp065, angles.x, 0, 65);
        });

        var rFootBone = GetJoint("rFoot").GetComponent<DAZBone>();
        _rFootXp065 = GetMorph("RFootX+065");
        _rFootXn040 = GetMorph("RFootX-040");
        _drivers.Add(() =>
        {
            var angles = rFootBone.GetAnglesDegrees();
            SetMorph(_rFootXn040, angles.x, 0, -40);
            SetMorph(_rFootXp065, angles.x, 0, 65);
        });

        #endregion

        #region Forearm

        var lForearmBone = GetJoint("lForeArm").GetComponent<DAZBone>();
        _lForearmYn100 = GetMorph("LForearmY-100");
        _lForearmYn130 = GetMorph("LForearmY-130");
        _drivers.Add(() =>
        {
            var angles = lForearmBone.GetAnglesDegrees();
            // Note: Y Inverted
            SetMorph(_lForearmYn100, angles.y, 0, 100);
            SetMorph(_lForearmYn130, angles.y, 100, 130);
        });

        var rForearmBone = GetJoint("rForeArm").GetComponent<DAZBone>();
        _rForearmYp100 = GetMorph("RForearmY+100");
        _rForearmYp130 = GetMorph("RForearmY+130");
        _drivers.Add(() =>
        {
            var angles = rForearmBone.GetAnglesDegrees();
            // Note: Y Inverted
            SetMorph(_rForearmYp100, angles.y, 0, -100);
            SetMorph(_rForearmYp130, angles.y, -100, -130);
        });

        #endregion

        #region Hand

        var lHandBone = GetJoint("lHand").GetComponent<DAZBone>();
        var rHandBone = GetJoint("rHand").GetComponent<DAZBone>();
        _lHandZp080 = GetMorph("LHandZ+080");
        _rHandZn080 = GetMorph("RHandZ-080");
        _drivers.Add(() =>
        {
            var langles = lHandBone.GetAnglesDegrees();
            var rangles = rHandBone.GetAnglesDegrees();
            SetMorph(_lHandZp080, langles.z, 0, -80);
            SetMorph(_rHandZn080, rangles.z, 0, 80);
        });

        #endregion

        #region Shin

        var lShinBone = GetJoint("lShin").GetComponent<DAZBone>();
        _lShinXp085 = GetMorph("LShinX+085");
        _lShinXp140 = GetMorph("LShinX+140");
        _drivers.Add(() =>
        {
            var angles = lShinBone.GetAnglesDegrees();
            SetMorph(_lShinXp085, angles.x, 0, 85);
            SetMorph(_lShinXp140, angles.x, 85, 140);
        });

        var rShinBone = GetJoint("rShin").GetComponent<DAZBone>();
        _rShinXp085 = GetMorph("RShinX+085");
        _rShinXp140 = GetMorph("RShinX+140");
        _drivers.Add(() =>
        {
            var angles = rShinBone.GetAnglesDegrees();
            SetMorph(_rShinXp085, angles.x, 0, 85);
            SetMorph(_rShinXp140, angles.x, 85, 140);
        });

        #endregion

        #region Shoulder

        var lShldrBone = GetJoint("lShldr").GetComponent<DAZBone>();
        _lShldrZp035 = GetMorph("LShldrZ+035");
        _lShldrZn060 = GetMorph("LShldrZ-060");
        _lShldrZn075 = GetMorph("LShldrZ-075");
        _lShldrYn095 = GetMorph("LShldrY-095");
        _lShldrYp040 = GetMorph("LShldrY+040");
        _drivers.Add(() =>
        {
            var angles = lShldrBone.GetAnglesDegrees();
            // Note: Y Inverted
            SetMorph(_lShldrYn095, angles.y, 0, 95);
            SetMorph(_lShldrYp040, angles.y, 0, -40);
            // Remove -Z as Y approaches -40
            float zcor = angles.z * -NormalizeFloat(angles.y, 0, -40);
            // Note: DAZ Z = VaM Z Inverted
            SetMorph(_lShldrZp035, angles.z, 0, -35);
            SetMorph(_lShldrZn060, angles.z + zcor, 0, 60);
            SetMorph(_lShldrZn075, angles.z + zcor, 60, 75);
        });

        var rShldrBone = GetJoint("rShldr").GetComponent<DAZBone>();
        _rShldrZn035 = GetMorph("RShldrZ-035");
        _rShldrZp060 = GetMorph("RShldrZ+060");
        _rShldrZp075 = GetMorph("RShldrZ+075");
        _rShldrYn040 = GetMorph("RShldrY-040");
        _rShldrYp095 = GetMorph("RShldrY+095");
        _drivers.Add(() =>
        {
            var angles = rShldrBone.GetAnglesDegrees();
            // Note: Y probably Inverted
            SetMorph(_rShldrYn040, angles.y, 0, 40);
            SetMorph(_rShldrYp095, angles.y, 0, -95);
            // Remove +Z as Y approaches 40
            float zcor = angles.z * -NormalizeFloat(angles.y, 0, 40);
            // Note: DAZ Z = VaM Z Inverted
            SetMorph(_rShldrZn035, angles.z, 0, 35);
            SetMorph(_rShldrZp060, angles.z + zcor, 0, -60);
            SetMorph(_rShldrZp075, angles.z + zcor, -60, -75);
        });

        #endregion

        #region Thigh

        _lThighXp035 = GetMorph("LThighX+035");
        _lThighXn055 = GetMorph("LThighX-055");
        _lThighXn115 = GetMorph("LThighX-115");
        _lThighYp075 = GetMorph("LThighY+075");
        _lThighYn075 = GetMorph("LThighY-075");
        _lThighZp085 = GetMorph("LThighZ+085");
        _lThighZp085Gens = GetMorph("LThighZ+085.gens");
        _lThighZn015 = GetMorph("LThighZ-015");
        // _lThighZn015gens = GetMorph("LThighZ-015.gens"); // unused

        _rThighXp035 = GetMorph("RThighX+035");
        _rThighXn055 = GetMorph("RThighX-055");
        _rThighXn115 = GetMorph("RThighX-115");
        _rThighYn075 = GetMorph("RThighY-075");
        _rThighYp075 = GetMorph("RThighY+075");
        _rThighZn085 = GetMorph("RThighZ-085");
        _rThighZn085Gens = GetMorph("RThighZ-085.gens");
        _rThighZp015 = GetMorph("RThighZ+015");

        // _cThighZ180gens = GetMorph("CThighsZ180.gens"); // unused
        _cThighZp180 = GetMorph("CThighsZ+180");
        _cThighZp180Gens = GetMorph("CThighsZ+180.gens");
        _cThighZn030Gens = GetMorph("CThighsZ-030.gens");
        _cThighXn115 = GetMorph("CThighsX-115");
        _cThighXn115Gens = GetMorph("CThighsX-115.gens");

        var lThighBone = GetJoint("lThigh").GetComponent<DAZBone>();
        var rThighBone = GetJoint("rThigh").GetComponent<DAZBone>();
        _drivers.Add(() =>
        {
            var lAngles = lThighBone.GetAnglesDegrees();
            var rAngles = rThighBone.GetAnglesDegrees();

            SetMorph(_lThighXn115, lAngles.x, -55, -115);
            SetMorph(_lThighXn055, lAngles.x, 0, -55);
            SetMorph(_lThighXp035, lAngles.x, 0, 35);

            SetMorph(_rThighXn115, rAngles.x, -55, -115);
            SetMorph(_rThighXn055, rAngles.x, 0, -55);
            SetMorph(_rThighXp035, rAngles.x, 0, 35);

            // Note: DAZ Y = VAM Y Inverted
            SetMorph(_lThighYn075, lAngles.y, 0, 75);
            SetMorph(_lThighYp075, lAngles.y, 0, -75);

            // Note: DAZ Y = VAM Y Inverted
            SetMorph(_rThighYn075, rAngles.y, 0, 75);
            SetMorph(_rThighYp075, rAngles.y, 0, -75);

            // Note: DAZ Z = VAM Z Inverted
            SetMorph(_lThighZn015, lAngles.z, 0, 15);
            SetMorph(_lThighZp085, lAngles.z, 0, -85);
            SetMorph(_lThighZp085Gens, lAngles.z, 0, -85);

            // Note: DAZ Z = VAM Z Inverteda
            SetMorph(_rThighZn085, rAngles.z, 0, 85);
            SetMorph(_rThighZn085Gens, rAngles.z, 0, 85);
            SetMorph(_rThighZp015, rAngles.z, 0, -15);

            float thighZSeparation = rAngles.z - lAngles.z;
            SetMorph(_cThighZp180, thighZSeparation, 0, 180);
            SetMorph(_cThighZp180Gens, thighZSeparation, 0, 180);
            SetMorph(_cThighZn030Gens, thighZSeparation, 0, -30);

            float thighXCombination = (lAngles.x + rAngles.x) / 2;
            SetMorph(_cThighXn115, thighXCombination, -55, -115);
            SetMorph(_cThighXn115Gens, thighXCombination, -55, -115);
        });

        #endregion

        #region Trunk

        var pelvisBone = GetJoint("pelvis").GetComponent<DAZBone>();
        _pelvisXp030 = GetMorph("TPelvisX+030");
        _pelvisXn015 = GetMorph("TPelvisX-015");
        _drivers.Add(() =>
        {
            var angles = pelvisBone.GetAnglesDegrees();
            SetMorph(_pelvisXn015, angles.x, 0, -15);
            SetMorph(_pelvisXp030, angles.x, 0, 30);
        });

        var abdomenBone = GetJoint("abdomen").GetComponent<DAZBone>();
        _abdomenXn020 = GetMorph("TAbdomenX-020");
        _abdomenXp030 = GetMorph("TAbdomenX+030");
        _drivers.Add(() =>
        {
            var angles = abdomenBone.GetAnglesDegrees();
            SetMorph(_abdomenXn020, angles.x, 0, -20);
            SetMorph(_abdomenXp030, angles.x, 0, 30);
        });

        var abdomen2Bone = GetJoint("abdomen2").GetComponent<DAZBone>();
        _abdomen2Xn020 = GetMorph("TAbdomen2X-020");
        _abdomen2Xp030 = GetMorph("TAbdomen2X+030");
        _drivers.Add(() =>
        {
            var angles = abdomen2Bone.GetAnglesDegrees();
            SetMorph(_abdomen2Xn020, angles.x, 0, -20);
            SetMorph(_abdomen2Xp030, angles.x, 0, 30);
        });

        var chestBone = GetJoint("chest").GetComponent<DAZBone>();
        _chestXp020 = GetMorph("TChestX+020");
        _drivers.Add(() =>
        {
            var angles = chestBone.GetAnglesDegrees();
            SetMorph(_chestXp020, angles.x, 0, 20);
        });

        var neckBone = GetJoint("neck").GetComponent<DAZBone>();
        _neckXn030 = GetMorph("TNeckX-030");
        _neckYp035 = GetMorph("TNeckY+035");
        _neckYn035 = GetMorph("TNeckY-035");
        _drivers.Add(() =>
        {
            var angles = neckBone.GetAnglesDegrees();
            SetMorph(_neckXn030, angles.x, 0, -30);
            SetMorph(_neckYp035, angles.y, 0, -35);
            SetMorph(_neckYn035, angles.y, 0, 35);
        });

        var headBone = GetJoint("head").GetComponent<DAZBone>();
        _headXn045 = GetMorph("THeadX-045");
        _headXp035 = GetMorph("THeadX+035");
        _drivers.Add(() =>
        {
            var angles = headBone.GetAnglesDegrees();
            SetMorph(_headXn045, angles.x, 0, -45);
            SetMorph(_headXp035, angles.x, 0, 35);
        });

        #endregion

        if(envIsDevelopment)
        {
            /* Debug bones */
            _drivers.Add(() =>
            {
                PrintBone(lCollarBone);
                PrintBone(rCollarBone);
                PrintBone(lFootBone);
                PrintBone(rFootBone);
                PrintBone(lForearmBone);
                PrintBone(rForearmBone);
                PrintBone(lHandBone);
                PrintBone(rHandBone);
                PrintBone(lShinBone);
                PrintBone(rShinBone);
                PrintBone(lShldrBone);
                PrintBone(rShldrBone);
                PrintBone(lThighBone);
                PrintBone(rThighBone);
                PrintBone(pelvisBone);
                PrintBone(abdomenBone);
                PrintBone(abdomen2Bone);
                PrintBone(chestBone);
                PrintBone(neckBone);
            });

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

        _locked.val = false;
    }

    public void Update()
    {
        if(!_initialized || _locked.val)
        {
            return;
        }

        try
        {
            foreach(var driver in _drivers)
            {
                driver.Invoke();
            }

            if(envIsDevelopment)
            {
                _mLog.Remove(0, _mLog.Length);
                _rLog.Remove(0, _rLog.Length);
                _morphOut.valNoCallback = _mLog.ToString();
                _jointOut.valNoCallback = _rLog.ToString();
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

    private void SetMorph(MorphConfig morphConfig, float value, float starting, float ending)
    {
        value = NormalizeFloat(value, starting, ending);
        if(envIsDevelopment)
        {
            _mLog.AppendLine($"{morphConfig.morph.displayName} <- {value:##0.00}");
        }

        morphConfig.morph.morphValue = morphConfig.multiplier * value;
    }

    private MorphConfig GetMorph(string morphName)
    {
        var config = new MorphConfig(morphName, 1);
        _morphConfigs.Add(config);
        return config;
    }

    private Transform GetJoint(string jointName) =>
        containingAtom.GetStorableByID(jointName).transform;

    private void PrintBone(DAZBone bone)
    {
        var angles = bone.GetAnglesDegrees();
        _rLog.AppendLine($"{angles.x: 000;-000}, {angles.y: 000;-000}, {angles.z: 000;-000} {bone.name} {bone.rotationOrder}");
    }

    /* TODO InverseLerp? */
    private static float NormalizeFloat(float value, float start, float end) =>
        Mathf.Clamp((value - start) / (end - start), 0, 1);

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
            foreach(var morphConfig in _morphConfigs)
            {
                morphConfig.morph.morphValue = 0;
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

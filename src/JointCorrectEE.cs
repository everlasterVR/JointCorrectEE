#define ENV_DEVELOPMENT
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

sealed class JointCorrectEE : ScriptBase
{
    public override bool ShouldIgnore() => false;

    public override void Init()
    {
        try
        {
            this.NewJSONStorableString(Strings.VERSION, VERSION);
            if(containingAtom.type != "Person")
            {
                logBuilder.ErrorNoReport("Add to a Person atom, not {0}.", containingAtom.type);
                return;
            }

            if(containingAtom.StorableExistsByRegexMatch(Utils.NewRegex($@"^plugin#\d+_{nameof(JointCorrectEE)}")))
            {
                logBuilder.ErrorNoReport("An instance of {0} is already added.", nameof(JointCorrectEE));
                return;
            }

            StartOrPostponeCoroutine(InitCo());
        }
        catch(Exception e)
        {
            logBuilder.Error("{0}: {1}", nameof(Init), e);
        }
    }

    public Person Person { get; private set; }
    public BoneConfig[] BoneConfigs { get; private set; }
    public JSONStorableBool DisableCollarBreastJsb { get; private set; }

    IWindow _currentWindow;
    IWindow _mainWindow;
    IWindow _inactiveWindow;

    IEnumerator InitCo()
    {
        yield return new WaitForEndOfFrame();
        while(SuperController.singleton.isLoading)
        {
            yield return null;
        }

        yield return SetupPerson();

        try
        {
            InitMorphs();
            DisableCollarBreastJsb = this.NewJSONStorableBool("disableCollarBreastMorphs", true);
            _mainWindow = new MainWindow(this);
            isInitialized = true;
        }
        catch(Exception e)
        {
            logBuilder.Error("{0}: {1}", nameof(InitCo), e);
        }
    }

    IEnumerator SetupPerson()
    {
        Person = new Person(containingAtom);
        const int limit = 15;
        yield return Person.WaitForGeometryReady(limit);
        if(!Person.GeometryReady)
        {
            logBuilder.Error("Selected character {0} was not ready after {1} seconds of waiting", Person.Geometry.selectedCharacter.name, limit);
            yield break;
        }

        try
        {
            Person.Setup();
        }
        catch(Exception e)
        {
            logBuilder.Error("{0}: {1}", nameof(SetupPerson), e);
        }
    }

    protected override void GoToMainWindow()
    {
        if(_currentWindow == _mainWindow)
        {
            return;
        }

        _inactiveWindow?.Clear();
        _mainWindow.Build();
        _currentWindow = _mainWindow;
    }

    protected override void GoToInactiveWindow()
    {
        if(_currentWindow == _inactiveWindow)
        {
            return;
        }

        if(_inactiveWindow == null)
        {
            _inactiveWindow = new InactiveWindow(this);
        }

        _mainWindow?.Clear();
        _inactiveWindow.Build();
        _currentWindow = _inactiveWindow;
    }

    void InitMorphs()
    {
        BoneConfig collarsConfig;
        BoneConfig feetConfig;
        BoneConfig forearmsConfig;
        BoneConfig handsConfig;
        BoneConfig shinsConfig;
        BoneConfig shouldersConfig;
        BoneConfig thighsConfig;
        BoneConfig genitalsConfig;
        BoneConfig pelvisConfig;
        BoneConfig abdomenConfig;
        BoneConfig abdomen2Config;
        BoneConfig chestConfig;
        BoneConfig neckConfig;
        BoneConfig headConfig;

        /* Collar Bones */
        {
            var left = Person.GetBone("lCollar");
            var lCollarXn025 = new MorphConfig(Person.GetMorph("LCollarX-025"));
            var lCollarXp015 = new MorphConfig(Person.GetMorph("LCollarX+015"));
            var lCollarYn026 = new MorphConfig(Person.GetMorph("LCollarY-026"));
            var lCollarYp017 = new MorphConfig(Person.GetMorph("LCollarY+017"));
            var lCollarZn015 = new MorphConfig(Person.GetMorph("LCollarZ-015"));
            var lCollarZp050 = new MorphConfig(Person.GetMorph("LCollarZ+050"));

            var right = Person.GetBone("rCollar");
            var rCollarXn025 = new MorphConfig(Person.GetMorph("RCollarX-025"));
            var rCollarXp015 = new MorphConfig(Person.GetMorph("RCollarX+015"));
            var rCollarYn017 = new MorphConfig(Person.GetMorph("RCollarY-017"));
            var rCollarYp026 = new MorphConfig(Person.GetMorph("RCollarY+026"));
            var rCollarZn050 = new MorphConfig(Person.GetMorph("RCollarZ-050"));
            var rCollarZp015 = new MorphConfig(Person.GetMorph("RCollarZ+015"));

            collarsConfig = new BoneConfig
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
                multiplierJsf = this.NewJSONStorableFloat("Collar Bones", 1, 0, 2),
                driver = multiplier =>
                {
                    var lAngles = left.GetAnglesDegrees();
                    lCollarXn025.Update(lAngles.x, 0, -25);
                    lCollarXp015.Update(lAngles.x, 0, 15);

                    // Note: Y Inverted
                    lCollarYp017.Update(lAngles.y, 0, -17);

                    // Note: Rotation order not honored, Z Inverted
                    lCollarZn015.Update(lAngles.z, 0, 15);

                    var rAngles = right.GetAnglesDegrees();
                    // Note: Rotation order not honored
                    rCollarXn025.Update(rAngles.x, 0, -25);
                    rCollarXp015.Update(rAngles.x, 0, 15);

                    // Note: Y Inverted
                    rCollarYn017.Update(rAngles.y, 0, 17);

                    // Note: Rotation order not honored, Z Inverted
                    rCollarZp015.Update(rAngles.z, 0, -15);

                    /* These morphs visibly affect breast vertices that have soft physics joints */
                    if(!DisableCollarBreastJsb.val)
                    {
                        // Note: Y Inverted
                        lCollarYn026.Update(lAngles.y, 0, 26); // problematic

                        // Note: Rotation order not honored, Z Inverted
                        lCollarZp050.Update(lAngles.z, 0, -50); // problematic

                        // Note: Y Inverted
                        rCollarYp026.Update(rAngles.y, 0, -26); // problematic

                        // Note: Rotation order not honored, Z Inverted
                        rCollarZn050.Update(rAngles.z, 0, 50); // problematic
                    }
                    else
                    {
                        lCollarYn026.Reset();
                        lCollarZp050.Reset();
                        rCollarYp026.Reset();
                        rCollarZn050.Reset();
                    }
                },
            };
        }

        /* Feet */
        {
            var left = Person.GetBone("lFoot");
            var lFootXp065 = new MorphConfig(Person.GetMorph("LFootX+065"));
            var lFootXn040 = new MorphConfig(Person.GetMorph("LFootX-040"));

            var right = Person.GetBone("rFoot");
            var rFootXp065 = new MorphConfig(Person.GetMorph("RFootX+065"));
            var rFootXn040 = new MorphConfig(Person.GetMorph("RFootX-040"));

            feetConfig = new BoneConfig
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
            };
        }

        /* Forearms */
        {
            var lForearmBone = Person.GetBone("lForeArm");
            var lForearmYn100 = new MorphConfig(Person.GetMorph("LForearmY-100"));
            var lForearmYn130 = new MorphConfig(Person.GetMorph("LForearmY-130"));

            var rForearmBone = Person.GetBone("rForeArm");
            var rForearmYp100 = new MorphConfig(Person.GetMorph("RForearmY+100"));
            var rForearmYp130 = new MorphConfig(Person.GetMorph("RForearmY+130"));

            forearmsConfig = new BoneConfig
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
            };
        }

        /* Hands */
        {
            var left = Person.GetBone("lHand");
            var lHandZp080 = new MorphConfig(Person.GetMorph("LHandZ+080"));

            var right = Person.GetBone("rHand");
            var rHandZn080 = new MorphConfig(Person.GetMorph("RHandZ-080"));

            handsConfig = new BoneConfig
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
            };
        }

        /* Shins */
        {
            var left = Person.GetBone("lShin");
            var lShinXp085 = new MorphConfig(Person.GetMorph("LShinX+085"));
            var lShinXp140 = new MorphConfig(Person.GetMorph("LShinX+140"));

            var right = Person.GetBone("rShin");
            var rShinXp085 = new MorphConfig(Person.GetMorph("RShinX+085"));
            var rShinXp140 = new MorphConfig(Person.GetMorph("RShinX+140"));

            shinsConfig = new BoneConfig
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
            };
        }

        /* Shoulders */
        {
            var left = Person.GetBone("lShldr");
            var lShldrZp035 = new MorphConfig(Person.GetMorph("LShldrZ+035"));
            var lShldrZn060 = new MorphConfig(Person.GetMorph("LShldrZ-060"));
            var lShldrZn075 = new MorphConfig(Person.GetMorph("LShldrZ-075"));
            var lShldrYn095 = new MorphConfig(Person.GetMorph("LShldrY-095"));
            var lShldrYp040 = new MorphConfig(Person.GetMorph("LShldrY+040"));

            var right = Person.GetBone("rShldr");
            var rShldrZn035 = new MorphConfig(Person.GetMorph("RShldrZ-035"));
            var rShldrZp060 = new MorphConfig(Person.GetMorph("RShldrZ+060"));
            var rShldrZp075 = new MorphConfig(Person.GetMorph("RShldrZ+075"));
            var rShldrYn040 = new MorphConfig(Person.GetMorph("RShldrY-040"));
            var rShldrYp095 = new MorphConfig(Person.GetMorph("RShldrY+095"));

            shouldersConfig = new BoneConfig
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
                    float zcorLeft = lAngles.z * -Calc.NormalizeFloat(lAngles.y, 0, -40);
                    // Note: DAZ Z = VaM Z Inverted
                    lShldrZp035.Update(lAngles.z, 0, -35);
                    lShldrZn060.Update(lAngles.z + zcorLeft, 0, 60);
                    lShldrZn075.Update(lAngles.z + zcorLeft, 60, 75);

                    var rAngles = right.GetAnglesDegrees();
                    // Note: Y probably Inverted
                    rShldrYn040.Update(rAngles.y, 0, 40);
                    rShldrYp095.Update(rAngles.y, 0, -95);
                    // Remove +Z as Y approaches 40
                    float zcorRight = rAngles.z * -Calc.NormalizeFloat(rAngles.y, 0, 40);
                    // Note: DAZ Z = VaM Z Inverted
                    rShldrZn035.Update(rAngles.z, 0, 35);
                    rShldrZp060.Update(rAngles.z + zcorRight, 0, -60);
                    rShldrZp075.Update(rAngles.z + zcorRight, -60, -75);
                },
            };
        }

        /* Thighs */
        {
            var left = Person.GetBone("lThigh");
            var lThighXp035 = new MorphConfig(Person.GetMorph("LThighX+035"));
            var lThighXn055 = new MorphConfig(Person.GetMorph("LThighX-055"));
            var lThighXn115 = new MorphConfig(Person.GetMorph("LThighX-115"));
            var lThighYp075 = new MorphConfig(Person.GetMorph("LThighY+075"));
            var lThighYn075 = new MorphConfig(Person.GetMorph("LThighY-075"));
            var lThighZp085 = new MorphConfig(Person.GetMorph("LThighZ+085"));
            var lThighZn015 = new MorphConfig(Person.GetMorph("LThighZ-015"));

            var right = Person.GetBone("rThigh");
            var rThighXp035 = new MorphConfig(Person.GetMorph("RThighX+035"));
            var rThighXn055 = new MorphConfig(Person.GetMorph("RThighX-055"));
            var rThighXn115 = new MorphConfig(Person.GetMorph("RThighX-115"));
            var rThighYn075 = new MorphConfig(Person.GetMorph("RThighY-075"));
            var rThighYp075 = new MorphConfig(Person.GetMorph("RThighY+075"));
            var rThighZn085 = new MorphConfig(Person.GetMorph("RThighZ-085"));
            var rThighZp015 = new MorphConfig(Person.GetMorph("RThighZ+015"));

            var cThighZp180 = new MorphConfig(Person.GetMorph("CThighsZ+180"));
            var cThighXn115 = new MorphConfig(Person.GetMorph("CThighsX-115"));

            thighsConfig = new BoneConfig
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
            };
        }

        /* Genitals */
        {
            var left = Person.GetBone("lThigh");
            var lThighZp085Gens = new MorphConfig(Person.GetMorph("LThighZ+085.gens"));
            // var lThighZn015gens = GetMorph("LThighZ-015.gens"); // unused

            var right = Person.GetBone("rThigh");
            var rThighZn085Gens = new MorphConfig(Person.GetMorph("RThighZ-085.gens"));

            // var cThighZ180gens = GetMorph("CThighsZ180.gens"); // unused
            var cThighZp180Gens = new MorphConfig(Person.GetMorph("CThighsZ+180.gens"));
            var cThighZn030Gens = new MorphConfig(Person.GetMorph("CThighsZ-030.gens"));
            var cThighXn115Gens = new MorphConfig(Person.GetMorph("CThighsX-115.gens"));

            genitalsConfig = new BoneConfig
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
            };
        }

        /* Pelvis */
        {
            var bone = Person.GetBone("pelvis");
            var pelvisXp030 = new MorphConfig(Person.GetMorph("TPelvisX+030"));
            var pelvisXn015 = new MorphConfig(Person.GetMorph("TPelvisX-015"));

            pelvisConfig = new BoneConfig
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
            };
        }

        /* Abdomen */
        {
            var bone = Person.GetBone("abdomen");
            var abdomenXn020 = new MorphConfig(Person.GetMorph("TAbdomenX-020"));
            var abdomenXp030 = new MorphConfig(Person.GetMorph("TAbdomenX+030"));

            abdomenConfig = new BoneConfig
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
            };
        }

        /* Abdomen2 */
        {
            var bone = Person.GetBone("abdomen2");
            var abdomen2Xn020 = new MorphConfig(Person.GetMorph("TAbdomen2X-020"));
            var abdomen2Xp030 = new MorphConfig(Person.GetMorph("TAbdomen2X+030"));

            abdomen2Config = new BoneConfig
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
            };
        }

        /* Chest */
        {
            var bone = Person.GetBone("chest");
            var chestXp020 = new MorphConfig(Person.GetMorph("TChestX+020"));

            chestConfig = new BoneConfig
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
            };
        }

        /* Neck */
        {
            var bone = Person.GetBone("neck");
            var neckXn030 = new MorphConfig(Person.GetMorph("TNeckX-030"));
            var neckYp035 = new MorphConfig(Person.GetMorph("TNeckY+035"));
            var neckYn035 = new MorphConfig(Person.GetMorph("TNeckY-035"));

            neckConfig = new BoneConfig
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
            };
        }

        /* Head */
        {
            var bone = Person.GetBone("head");
            var headXn045 = new MorphConfig(Person.GetMorph("THeadX-045"));
            var headXp035 = new MorphConfig(Person.GetMorph("THeadX+035"));

            headConfig = new BoneConfig
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
            };
        }

        /* Order matters for UI */
        BoneConfigs = new []
        {
            headConfig,
            neckConfig,
            collarsConfig,
            chestConfig,
            shouldersConfig,
            forearmsConfig,
            handsConfig,
            abdomenConfig,
            abdomen2Config,
            pelvisConfig,
            genitalsConfig,
            thighsConfig,
            shinsConfig,
            feetConfig,
        };

        for(int i = 0; i < BoneConfigs.Length; i++)
        {
            BoneConfigs[i].SetGroupMultiplierReferences();
        }
    }

    void Update()
    {
        if(!isInitialized)
        {
            return;
        }

        try
        {
            for(int i = 0; i < BoneConfigs.Length; i++)
            {
                var config = BoneConfigs[i];
                config.Update();
            }
        }
        catch(Exception e)
        {
            logBuilder.Error("{0}: {1}", nameof(Update), e);
        }
    }

    public override void RestoreFromJSON(
        JSONClass jc,
        bool restorePhysical = true,
        bool restoreAppearance = true,
        JSONArray presetAtoms = null,
        bool setMissingToDefault = true
    )
    {
        isRestoringFromJSON = true;

        /* Disable early to allow correct enabled value to be used during Init */
        if(jc.HasKey("enabled") && !jc["enabled"].AsBool)
        {
            enabledJSON.val = false;
        }

        /* Prevent overriding versionJss.val from JSON. Version stored in JSON just for information,
         * but could be intercepted here and used to save a "loadedFromVersion" value.
         */
        if(jc.HasKey(Strings.VERSION))
        {
            jc[Strings.VERSION] = VERSION;
        }

        StartOrPostponeCoroutine(RestoreFromJSONCo(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault));
    }

    IEnumerator RestoreFromJSONCo(
        JSONClass jsonClass,
        bool restorePhysical,
        bool restoreAppearance,
        JSONArray presetAtoms,
        bool setMissingToDefault
    )
    {
        while(!isInitialized)
        {
            yield return null;
        }

        base.RestoreFromJSON(jsonClass, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);
        isRestoringFromJSON = false;
    }

    public void AddTextFieldToJss(UIDynamicTextField textField, JSONStorableString jss)
    {
        jss.dynamicText = textField;
        textFieldToJSONStorableString.Add(textField, jss);
    }

    public void AddToggleToJsb(UIDynamicToggle toggle, JSONStorableBool jsb)
    {
        jsb.toggle = toggle.toggle;
        toggleToJSONStorableBool.Add(toggle, jsb);
    }

    public void AddSliderToJsf(UIDynamicSlider slider, JSONStorableFloat jsf)
    {
        jsf.slider = slider.slider;
        sliderToJSONStorableFloat.Add(slider, jsf);
    }

    void OnDisable()
    {
        if(!isInitialized)
        {
            return;
        }

        try
        {
            for(int i = 0; i < BoneConfigs.Length; i++)
            {
                BoneConfigs[i].Reset();
            }
        }
        catch(Exception e)
        {
            logBuilder.Error("{0}: {1}", nameof(OnDisable), e);
        }
    }

    void OnDestroy()
    {
        try
        {
            BaseDestroy();
        }
        catch(Exception e)
        {
            if(isInitialized)
            {
                SuperController.LogError($"{nameof(OnDestroy)}: {e}");
            }
            else
            {
                Debug.LogError($"{nameof(OnDestroy)}: {e}");
            }
        }
    }
}

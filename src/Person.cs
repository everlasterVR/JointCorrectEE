using System.Collections;
using UnityEngine;

sealed class Person
{
    readonly Atom _atom;
    public DAZCharacterSelector Geometry { get; private set; }
    public bool IsFemale { get; private set; }
    GenerateDAZMorphsControlUI _morphsControlUI;

    const string MORPHS_PATH = "FallenDancer.JointCorrect.11:/Custom/Atom/Person/Morphs/female/FallenDancer/JCM";
    const string GEN_MORPHS_PATH = "FallenDancer.JointCorrect.11:/Custom/Atom/Person/Morphs/female_genitalia/FallenDancer/JCM";

    public Person(Atom atom)
    {
        _atom = atom;
    }

    public bool GeometryReady { get; private set; }

    public IEnumerator WaitForGeometryReady(int limit)
    {
        float timeout = Time.unscaledTime + limit;
        while(!GeometryReady && Time.unscaledTime < timeout)
        {
            Geometry = (DAZCharacterSelector) _atom.GetStorableByID("geometry");
            GeometryReady = Geometry.selectedCharacter.ready;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void Setup()
    {
        Geometry = (DAZCharacterSelector) _atom.GetStorableByID("geometry");
        IsFemale = !Geometry.selectedCharacter.isMale;
        _morphsControlUI = IsFemale ? Geometry.morphsControlUI : Geometry.morphsControlUIOtherGender;
    }

    public DAZMorph GetMorph(string filename)
    {
        bool isGen = filename.EndsWith(".gens");
        string uid = $"{(isGen ? GEN_MORPHS_PATH : MORPHS_PATH)}/{filename}.vmi";
        var dazMorph = _morphsControlUI.GetMorphByUid(uid);
        if(dazMorph == null)
        {
            new LogBuilder().Error("Morph with uid '{0}' not found", uid);
        }

        return dazMorph;
    }

    public DAZBone GetBone(string jointName) =>
        _atom.GetStorableByID(jointName).transform.GetComponent<DAZBone>();
}

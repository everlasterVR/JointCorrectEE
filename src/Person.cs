using System.Collections;
using UnityEngine;

sealed class Person
{
    readonly Atom _atom;
    public DAZCharacterSelector geometry { get; private set; }
    public bool isFemale { get; private set; }
    public GenerateDAZMorphsControlUI morphsControlUI { get; private set; }

    const string MORPHS_PATH = "FallenDancer.JointCorrect.11:/Custom/Atom/Person/Morphs/female/FallenDancer/JCM";
    const string GEN_MORPHS_PATH = "FallenDancer.JointCorrect.11:/Custom/Atom/Person/Morphs/female_genitalia/FallenDancer/JCM";

    public Person(Atom atom)
    {
        _atom = atom;
    }

    public bool geometryReady { get; private set; }

    public IEnumerator WaitForGeometryReady(int limit)
    {
        float timeout = Time.unscaledTime + limit;
        while(!geometryReady && Time.unscaledTime < timeout)
        {
            geometry = (DAZCharacterSelector) _atom.GetStorableByID("geometry");
            geometryReady = geometry.selectedCharacter.ready;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void Setup()
    {
        geometry = (DAZCharacterSelector) _atom.GetStorableByID("geometry");
        isFemale = !geometry.selectedCharacter.isMale;
        morphsControlUI = isFemale ? geometry.morphsControlUI : geometry.morphsControlUIOtherGender;
    }

    public DAZMorph GetMorph(string filename)
    {
        bool isGen = filename.EndsWith(".gens");
        string uid = $"{(isGen ? GEN_MORPHS_PATH : MORPHS_PATH)}/{filename}.vmi";
        var dazMorph = morphsControlUI.GetMorphByUid(uid);
        if(dazMorph == null)
        {
            new LogBuilder().Error("Morph with uid '{0}' not found", uid);
        }

        return dazMorph;
    }

    public DAZBone GetBone(string jointName) =>
        _atom.GetStorableByID(jointName).transform.GetComponent<DAZBone>();
}

// ReSharper disable UnusedMember.Global
using UnityEngine;
using static JointCorrectEE;

public static class Utils
{
    public static void Log(string message)
    {
        if(envIsDevelopment)
        {
            Debug.Log(message);
        }
    }

    public static void LogError(string message) =>
        SuperController.LogError(Format(message));

    public static void LogMessage(string message) =>
        SuperController.LogMessage(Format(message));

    private static string Format(string message) =>
        $"{nameof(JointCorrectEE)} {VERSION}: {message}";

    private const string MORPHS_PATH = "FallenDancer.JointCorrect.11:/Custom/Atom/Person/Morphs/female/FallenDancer/JCM";
    private const string GEN_MORPHS_PATH = "FallenDancer.JointCorrect.11:/Custom/Atom/Person/Morphs/female_genitalia/FallenDancer/JCM";

    public static DAZMorph GetMorph(string filename)
    {
        bool isGen = filename.EndsWith(".gens");
        string uid = $"{(isGen ? GEN_MORPHS_PATH : MORPHS_PATH)}/{filename}.vmi";
        var dazMorph = morphsControlUI.GetMorphByUid(uid);
        if(dazMorph == null)
        {
            LogError($"Morph with uid '{uid}' not found!");
        }

        return dazMorph;
    }
}

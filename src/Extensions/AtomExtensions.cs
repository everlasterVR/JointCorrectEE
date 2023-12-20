using System.Text.RegularExpressions;

static class AtomExtensions
{
    public static bool StorableExistsByRegexMatch(this Atom atom, Regex regex)
    {
        var storableIds = atom.GetStorableIDs();
        for(int i = 0; i < storableIds.Count; i++)
        {
            if(regex.IsMatch(storableIds[i]))
            {
                return true;
            }
        }

        return false;
    }
}

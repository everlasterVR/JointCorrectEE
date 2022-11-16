using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class AtomExtensions
{
    public static List<JSONStorable> FindStorablesByRegexMatch(this Atom atom, Regex regex) =>
        atom.GetStorableIDs()
            .Where(id => regex.IsMatch(id))
            .Select(atom.GetStorableByID)
            .ToList();
}

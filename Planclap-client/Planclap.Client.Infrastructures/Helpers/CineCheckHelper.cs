using Planclap.Client.Domains.core;

namespace Planclap.Client.Infrastructures.Helpers;

public static class CineCheckHelper
{
    public static (string age, IList<string> others) SplitAgeAndOther(IList<string> input)
    {
        for (var i = 0; i < input.Count; i++)
        {
            if (!CineCheckAge.IsValid(input[i]))
            {
                continue;
            }

            var age = input[i];
            var others = input.Where((_, index) => index != i).ToList();
            return (age, others);
        }

        throw new ArgumentException("Input string was not in a correct format.");
    }
}

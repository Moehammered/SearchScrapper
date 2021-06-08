using System.Linq;

namespace SearchQueryViewModels.Utility
{
    public static class PlaceFormat
    {
        public static string NumberToPlace(int value)
        {
            if (value >= 10 && value <= 20)
                return $"{value}th";
            else
            {
                var endingDigit = value.ToString().Last();
                switch (endingDigit)
                {
                    case '1':
                        return $"{value}st";

                    case '2':
                        return $"{value}nd";

                    case '3':
                        return $"{value}rd";

                    default:
                        return $"{value}th";
                }
            }
        }
    }
}

using System.ComponentModel;
using GameStore.Common.Utils;

namespace GameStore.Common.Constants;

[TypeConverter(typeof(FlexibleEnumTypeConverter<SortingOptions>))]
public enum SortingOptions
{
    MostPopular,
    MostCommented,
    PriceAscending,
    PriceDescending,
    New
}
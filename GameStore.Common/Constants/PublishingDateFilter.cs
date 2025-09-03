using System.ComponentModel;
using GameStore.Common.Utils;

namespace GameStore.Common.Constants;

[TypeConverter(typeof(FlexibleEnumTypeConverter<PublishingDateFilter>))]
public enum PublishingDateFilter
{
    LastWeek,
    LastMonth,
    LastYear,
    TwoYears,
    ThreeYears,
}
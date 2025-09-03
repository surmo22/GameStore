using GameStore.Common.Constants;

namespace GameStore.Common.Exceptions;

public class InvalidBanDurationException(BanDuration duration) : Exception($"Invalid ban duration: {duration.ToString()}.");
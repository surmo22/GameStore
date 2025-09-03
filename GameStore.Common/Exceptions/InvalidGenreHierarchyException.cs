namespace GameStore.Common.Exceptions;

public class InvalidGenreHierarchyException() : Exception("Genre id cannot be the same as Parent genre Id.");

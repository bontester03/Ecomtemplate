namespace KhanHomeFloralLine.Application.Common;

public class AppValidationException(string message) : Exception(message);
public class AppNotFoundException(string message) : Exception(message);


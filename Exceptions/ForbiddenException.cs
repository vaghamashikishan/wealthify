namespace wealthify.Exceptions;

public class ForbiddenException(string message) : CustomException(message, StatusCodes.Status403Forbidden);
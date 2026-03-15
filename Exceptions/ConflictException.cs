namespace wealthify.Exceptions;

public class ConflictException(string message) : CustomException(message, StatusCodes.Status409Conflict);

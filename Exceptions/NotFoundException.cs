namespace wealthify.Exceptions;

public class NotFoundException(string message) : CustomException(message, StatusCodes.Status404NotFound);

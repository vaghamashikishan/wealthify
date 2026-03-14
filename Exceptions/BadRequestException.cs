namespace wealthify.Exceptions;

public class BadRequestException(string message) : CustomException(message, StatusCodes.Status400BadRequest);



namespace iBartender.Core.Exceptions
{
    public class AlreadyExistsException(string message) : Exception(message);

    public class NotFoundException(string message) : Exception(message);

    public class InvalidCredentialsException(string message) : Exception(message);

    public class InvalidFileException(string message) : Exception(message);

    public class InvalidPasswordException(string message) : Exception(message);
}

using System.Linq;

namespace Helpers;

internal static class PasswordValidator
{
    internal static void ValidateOrThrow(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new BO.BlInvalidIdException("Password cannot be empty", null);

        if (password.Length < 6)
            throw new BO.BlInvalidIdException("Password must be at least 6 characters long", null);

        if (!password.Any(char.IsLetter))
            throw new BO.BlInvalidIdException("Password must contain at least one letter", null);

        if (!password.Any(char.IsDigit))
            throw new BO.BlInvalidIdException("Password must contain at least one number", null);
    }
}

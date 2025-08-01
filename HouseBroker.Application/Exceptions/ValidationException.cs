﻿namespace HouseBroker.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }

    public IEnumerable<ValidationError> Errors { get; }
}

public sealed record ValidationError(string PropertyName, string ErrorMessage);
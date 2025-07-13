using System.Diagnostics.CodeAnalysis;
using HouseBroker.Application.CommonDto;


public class Response
{
    protected Response(bool isSuccess, Error error)
    {
        switch (isSuccess)
        {
            case true when error != Error.None:
                throw new InvalidOperationException();
            case false when error == Error.None:
                throw new InvalidOperationException();
            default:
                IsSuccess = isSuccess;
                Error = IsSuccess? null:error;
                break;
        }
    }

    public bool IsSuccess { get; }

    public Error? Error { get; }

    public static Response Success() => new(true, Error.None);

    public static Response Failure(Error error) => new(false, error);

    public static Response<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Response<TValue> Failure<TValue>(Error error) => new(default, false, error);


    protected static Response<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.None);

    public bool Equals(Response? other)
    {
        throw new NotImplementedException();
    }
}

public class Response<TValue> : Response
{
    private readonly TValue? _value;

    protected internal Response(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Result => _value!;

    public static implicit operator Response<TValue>(TValue? value) => Create(value);
}
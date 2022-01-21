

using WeCoreCommon.Behaviours;

namespace WeCoreCommon.Validation;

public interface IValidationHandler { }

public interface IValidationHandler<T> : IValidationHandler
{
    Task<HandlerResponse> Validate(T request);
}


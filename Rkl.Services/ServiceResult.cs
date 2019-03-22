using System;

namespace Rkl.Services.Abstractions
{
    /// <summary>
    /// Инкапсулирует логику для возврата результата выполнения метода или возврата ошибки
    /// </summary>
    /// <typeparam name="TError">Тип ошибки</typeparam>
    public class ServiceResult<TError> : IServiceResult<TError>
    {
        protected ServiceResult(bool isSuccess) {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }

        public bool IsFaulted => !IsSuccess;

        TError[] _errors;

        /// <summary>
        /// Ошибки, которые возникли в результате работы.
        /// Если ошибок нет вернёт пустой массив.
        /// </summary>
        public TError[] Errors
        {
            get
            {
                return _errors ?? new TError[0];
            }

            protected set
            {
                _errors = value;
            }

        }

        /// <summary>
        /// Создаст не успешный ServiceResult
        /// </summary>
        /// <typeparam name="TDestination">Тип ServiceResult</typeparam>
        /// <returns></returns>
        public IServiceResult<TError, TDestination> CastToFaultedResult<TDestination>()
        {
            return ServiceResult<TError, TDestination>.Fault(Exception, Errors);
        }

        /// <summary>
        /// Иcключение, которое привело к созданию ошибки
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// Устнавливает Exception.
        /// Если исключение уже установленно, кидает ошибку (throw new Exception),
        /// что бы не потерять информацию об "Оригинальном" Exception, которое действительно привело к ошибке
        /// </summary>
        /// <param name="e"></param>
        public void SetException(Exception e)
        {
            if (Exception == null)
            {
                Exception = e;
            }
            throw new Exception($"{nameof(ServiceResult<TError>)}.{nameof(Exception)} already set.");
        }

        public static IServiceResult<TError> Success()
        {
            return new ServiceResult<TError>(true);
        }

        public static IServiceResult<TError, TResult> Success<TResult>(TResult data)
        {
            return ServiceResult<TError, TResult>.Success(data);
        }

        public static IServiceResult<TError> Fault(Exception e, params TError[] errors)
        {
            return new ServiceResult<TError>(false) { Exception = e, Errors =  errors };
        }

    }

    /// <summary>
    /// Инкапсулирует логику для возврата результата выполнения метода или возврата ошибки
    /// </summary>
    /// <typeparam name="TError">Тип ошибки</typeparam>
    /// <typeparam name="TResult">Тип результата</typeparam>
    public class ServiceResult<TError, TResult> : ServiceResult<TError>, IServiceResult<TError, TResult>
    {
        private ServiceResult(TResult data) : base(true)
        {
            Data = data;
        }

        protected ServiceResult() : base(false) { }


        [Obsolete("Use Data")]
        public TResult Result => Data;

        public TResult Data { get; }

        public bool IsFaultedOrNullResult => IsFaulted || Data == null;

        public bool IsSuccessAndNotNullResult => IsSuccess && Data != null;

        
        public static IServiceResult<TError, TResult> Success(TResult result)
        {
            return new ServiceResult<TError, TResult>(result);
        }

        public new static IServiceResult<TError, TResult> Fault(Exception e, params TError[] errors)
        {
            return new ServiceResult<TError, TResult>() { Exception = e, Errors = errors };
        }

        /// <summary>
        /// Создаст не успешный ServiceResult
        /// </summary>
        /// <typeparam name="TDestination">Тип ServiceResult</typeparam>
        /// <returns></returns>
        public IServiceResult<TError, TDestination> CastToFaultedResult<TDestination>()
        {
            return ServiceResult<TError, TDestination>.Fault(Exception, Errors);
        }

    }
}

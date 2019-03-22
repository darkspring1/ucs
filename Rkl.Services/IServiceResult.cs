using System;

namespace Rkl.Services.Abstractions
{
    /// <summary>
    /// Инкапсулирует логику для возврата результата выполнения метода или возврата ошибки
    /// </summary>
    /// <typeparam name="TError">Тип ошибки</typeparam>
    public interface IServiceResult<out TError>
    {
        bool IsSuccess { get; }

        bool IsFaulted { get; }

        /// <summary>
        /// Ошибки, которые возникли в результате работы.
        /// Если ошибок нет вернёт пустой массив.
        /// </summary>
        TError[] Errors { get; }

        /// <summary>
        /// Иcключение, которое привело к созданию ошибки
        /// </summary>
        Exception Exception { get; }


        /// <summary>
        /// Создаст не успешный ServiceResult
        /// </summary>
        /// <typeparam name="TDestination">Тип ServiceResult</typeparam>
        /// <returns></returns>
        IServiceResult<TError, TDestination> CastToFaultedResult<TDestination>();

        /// <summary>
        /// Устнавливает Exception.
        /// Если исключение уже установленно, кидает ошибку (throw new Exception),
        /// что бы не потерять информацию об "Оригинальном" Exception, которое действительно привело к ошибке
        /// </summary>
        /// <param name="e"></param>
        void SetException(Exception e);
    }

    /// <summary>
    /// Инкапсулирует логику для возврата результата выполнения метода или возврата ошибки
    /// </summary>
    /// <typeparam name="TError">Тип ошибки</typeparam>
    /// <typeparam name="TResult">Тип результата</typeparam>
    public interface IServiceResult<out TError, out TResult> : IServiceResult<TError>
    {
        /// <summary>
        /// Создаст не успешный IServiceResult
        /// </summary>
        /// <typeparam name="TDestination">Тип результата IServiceResult</typeparam>
        /// <returns></returns>
        IServiceResult<TError, TDestination> CastToFaultedResult<TDestination>();

        /// <summary>
        /// Результат работы сервиса
        /// </summary>
        [Obsolete("Use Data")]
        TResult Result { get; }

        /// <summary>
        /// Результат работы сервиса
        /// </summary>
        TResult Data { get; }

        bool IsFaultedOrNullResult { get; }

        bool IsSuccessAndNotNullResult { get; }
    }
}

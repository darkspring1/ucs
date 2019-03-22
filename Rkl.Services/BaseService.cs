using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rkl.Services.Abstractions
{
    /// <summary>
    /// Базовый класс для сервисов
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// Логгер
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Базовый класс для сервисов
        /// </summary>
        /// <param name="logger">Логгер для логирвание ошибок</param>
        public BaseService(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Базовый класс для сервисов. Логирование ошибок не будет производиться
        /// </summary>
        public BaseService()
        {
        }

        /// <summary>
        /// Запускает Action, логирует исключения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        protected IServiceResult<TError, T> Run<TError, T>(Func<T> action)
        {
            try
            {
                return ServiceResult<TError, T>.Success(action());
            }

            catch (Exception e)
            {
                Logger?.LogError(e, e.Message);
                return ServiceResult<TError, T>.Fault(e);
            }
        }

        /// <summary>
        /// Запускает Action, логирует исключения
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected IServiceResult<TError> Run<TError>(Action action)   
        {
            try
            {
                action();
                return ServiceResult<TError>.Success();
            }

            catch (Exception e)
            {
                Logger?.LogError(e, e.Message);
                return ServiceResult<TError>.Fault(e);
            }
        }

        /// <summary>
        /// Асинхронный запуск задачи
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="errorAction">обработка ошибки, проставление кодов, статусов и тд</param>
        /// <returns></returns>
        protected async Task<IServiceResult<TError, T>> RunAsync<TError, T>(Func<Task<IServiceResult<TError, T>>> action, Func<Exception, IServiceResult<TError, T>> errorAction)
        {
            try
            {
                return await action();
            }

            catch (Exception e)
            {
                Logger?.LogError(e, e.Message);

                if (errorAction == null)
                {
                    return ServiceResult<TError, T>.Fault(e);
                }
                return errorAction(e);
            }
        }

        /// <summary>
        /// Асинхронный запуск задачи
        /// </summary>
        /// <param name="action"></param>
        /// <param name="errorAction">обработка ошибки, проставление кодов, статусов и тд</param>
        /// <returns></returns>
        protected async Task<IServiceResult<TError, T>>
            RunAsync<TError, T>(Func<Task<IServiceResult<TError, T>>> action, Func<Exception, Task<IServiceResult<TError, T>>> errorAction)
            
        {
            try
            {
                return await action();
            }

            catch (Exception e)
            {
                Logger?.LogError(e, e.Message);

                if (errorAction == null)
                {
                    return ServiceResult<TError, T>.Fault(e);
                }
                return await errorAction(e);
            }
        }

        /// <summary>
        /// Асинхронный запуск задачи
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected Task<IServiceResult<TError, T>> RunAsync<TError, T>(Func<Task<IServiceResult<TError, T>>> action)
            
        {
            Func<Exception, Task<IServiceResult<TError, T>>> errorAction = null;
            return RunAsync(action, errorAction);
        }

        /// <summary>
        /// Асинхронный запуск задачи
        /// </summary>
        /// <param name="action"></param>
        /// <param name="errorAction">обработка ошибки, проставление кодов, статусов и тд</param>
        /// <returns></returns>
        protected async Task<IServiceResult<TError>>
            RunAsync<TError>(Func<Task<IServiceResult<TError>>> action, Func<Exception, Task<IServiceResult<TError>>> errorAction)
            
        {
            try
            {
                return await action();
            }

            catch (Exception e)
            {
                Logger?.LogError(e, e.Message);
                if (errorAction == null)
                {
                    return ServiceResult<TError>.Fault(e);
                }
                return await errorAction(e);
            }
        }

        /// <summary>
        /// Асинхронный запуск задачи
        /// </summary>
        /// <param name="action"></param>
        /// <param name="errorAction">обработка ошибки, проставление кодов, статусов и тд</param>
        /// <returns></returns>
        protected async Task<IServiceResult<TError>> RunAsync<TError>(Func<Task<IServiceResult<TError>>> action, Func<Exception, IServiceResult<TError>> errorAction)
            
        {
            try
            {
                return await action();
            }

            catch (Exception e)
            {
                Logger?.LogError(e, e.Message);
                if (errorAction == null)
                {
                    return ServiceResult<TError>.Fault(e);
                }
                return errorAction(e);
            }
        }

        /// <summary>
        /// Асинхронный запуск задачи
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected Task<IServiceResult<TError>> RunAsync<TError>(Func<Task<IServiceResult<TError>>> action)
            
        {
            return RunAsync(action, (Func<Exception, IServiceResult<TError>>)null);
        }

        /// <summary>
        /// синхронный запуск задачи
        /// </summary>
        /// <param name="action"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected Task<IServiceResult<TError>> RunAsync<TError>(Func<Task<IServiceResult<TError>>> action, TError error)
            
        {
            return RunAsync(action, exception => ServiceResult<TError>.Fault(exception, error));
        }

        /// <summary>
        /// синхронный запуск задачи
        /// </summary>
        /// <param name="action"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected Task<IServiceResult<TError, T>> RunAsync<TError, T>(Func<Task<IServiceResult<TError, T>>> action, TError error)
            
        {
            return RunAsync(action, exception => ServiceResult<TError, T>.Fault(exception, error));
        }

    }
}
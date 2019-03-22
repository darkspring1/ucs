namespace Rkl.DAL.Abstractions
{
    /// <summary>
    /// Страница результата запроса
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResult<T>
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="items"></param>
        /// <param name="totalCount"></param>
        public PagingResult(T[] items, int totalCount)
        {
            if (items == null)
            {
                items = new T[0];
            }
            Items = items;
            TotalCount = totalCount;
            if (items != null)
            {
                Selected = items.Length;
            }
            else
            {
                Selected = 0;
            }
        }
        /// <summary>
        /// Результат запроса (массив объектов)
        /// </summary>
        public T[] Items { get; }
        /// <summary>
        /// Обзщее количество объектов
        /// </summary>
        public int TotalCount { get; }
        /// <summary>
        /// Количество выбранных объектов
        /// </summary>
        public int Selected { get; }
    }
}

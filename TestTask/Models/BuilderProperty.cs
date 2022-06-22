namespace TestTask.Models
{
    /// <summary>
    /// Свойство строителя.
    /// Хранит информацию о том, было ли задано значение свойству.
    /// </summary>
    /// <typeparam name="T">Тип свойства.</typeparam>
    public struct BuilderProperty<T>
    {
        #region Private Fields

        /// <summary>
        /// Значение свойства.
        /// </summary>
        private T _value;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Было ли задано значение.
        /// </summary>
        public bool IsValueSetted { get; private set; }

        /// <inheritdoc cref="_value"/>
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                IsValueSetted = true;
            }
        }

        #endregion Public Properties
    }
}
namespace TestTask.Data.English
{
    /// <summary>
    /// Содержит список букв каждого типа: гласные и согласные.
    /// </summary>
    internal struct ListOfCharsTypes
    {
        #region Public Fields

        /// <summary>
        /// Список согласных букв.
        /// </summary>
        public static readonly string ConsonantsChars = "BCDFGHJKLMNPQRSTVWXZbcdfghjklmnpqrstvwxz";

        /// <summary>
        /// Список гласных букв.
        /// </summary>
        public static readonly string VovelChars = "AEIOUYaeiouy";

        #endregion Public Fields
    }
}
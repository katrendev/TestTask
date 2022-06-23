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

        /// <summary>
        /// Список всех букв.
        /// </summary>
        public static string All => ConsonantsChars + VovelChars;

        #endregion Public Fields
    }
}
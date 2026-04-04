namespace GerenteAcademico.Domain.Exceptions
{
    /// <summary>
    /// Excepción base para todas las excepciones de la aplicación.
    /// Permite diferenciar errores de negocio de errores técnicos.
    /// </summary>
    public abstract class BaseApplicationException : Exception
    {
        /// <summary>
        /// Código de error único para identificar el tipo de excepción.
        /// Útil para APIs y logging.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Detalles adicionales sobre el error.
        /// </summary>
        public string? Details { get; }

        protected BaseApplicationException(
            string message,
            string errorCode,
            string? details = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Details = details;
        }
    }
}

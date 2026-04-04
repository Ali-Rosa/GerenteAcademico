namespace GerenteAcademico.Domain.Exceptions
{
    /// <summary>
    /// Se lanza cuando una academia existe y está activa, pero le faltan datos importantes.
    /// La app requiere que ciertos campos tengan valores para poder renderizar correctamente.
    /// </summary>
    public class AcademiaIncompleteDataException : BaseApplicationException
    {
        /// <summary>
        /// Lista de campos que están faltando o vacíos.
        /// </summary>
        public string[] MissingFields { get; }

        public AcademiaIncompleteDataException(string codigo, params string[] missingFields)
            : base(
                message: $"La academia '{codigo}' existe pero le faltan datos obligatorios.",
                errorCode: "ACADEMIA_INCOMPLETE_DATA",
                details: $"Campos faltantes: {string.Join(", ", missingFields)}")
        {
            MissingFields = missingFields;
        }
    }
}

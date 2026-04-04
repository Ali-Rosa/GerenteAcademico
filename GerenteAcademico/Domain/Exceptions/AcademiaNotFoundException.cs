namespace GerenteAcademico.Domain.Exceptions
{
    /// <summary>
    /// Se lanza cuando se intenta acceder a una academia que no existe.
    /// </summary>
    public class AcademiaNotFoundException : BaseApplicationException
    {
        public AcademiaNotFoundException(int academiaId)
            : base(
                message: $"La academia con ID {academiaId} no fue encontrada.",
                errorCode: "ACADEMIA_NOT_FOUND",
                details: $"Attempted to access academia with ID: {academiaId}")
        {
        }

        public AcademiaNotFoundException(string codigo)
            : base(
                message: $"La academia con código '{codigo}' no fue encontrada.",
                errorCode: "ACADEMIA_CODE_NOT_FOUND",
                details: $"Attempted to access academia with code: {codigo}")
        {
        }
    }
}

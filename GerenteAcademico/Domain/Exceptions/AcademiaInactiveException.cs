namespace GerenteAcademico.Domain.Exceptions
{
    /// <summary>
    /// Se lanza cuando se intenta acceder a una academia que está desactivada.
    /// Útil en el login y acceso a datos.
    /// </summary>
    public class AcademiaInactiveException : BaseApplicationException
    {
        public AcademiaInactiveException(string codigo)
            : base(
                message: $"La academia '{codigo}' no está activa en este momento.",
                errorCode: "ACADEMIA_INACTIVE",
                details: $"Academia {codigo} is inactive. Check 'Activo' property in database.")
        {
        }

        public AcademiaInactiveException(int academiaId)
            : base(
                message: $"La academia con ID {academiaId} no está activa.",
                errorCode: "ACADEMIA_INACTIVE",
                details: $"Academia ID {academiaId} is inactive.")
        {
        }
    }
}

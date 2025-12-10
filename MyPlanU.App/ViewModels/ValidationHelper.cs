using System.Text.RegularExpressions;

namespace MyPlanU.App.ViewModels;

public static class ValidationHelper
{
    static readonly Regex s_emailRegex = new(@"^(?<local>[^@\s]+)@(?<domain>[A-Za-z0-9.-]+\.[A-Za-z]{2,})$", RegexOptions.Compiled);

    public static string ValidarCorreo(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return "Correo requerido.";

        var m = s_emailRegex.Match(valor);
        if (!m.Success)
            return "Formato de correo inválido.";

        return string.Empty;
    }

    public static string ValidarContrasena(string valor)
    {
        if (string.IsNullOrEmpty(valor))
            return "Contraseña requerida.";

        if (valor.Length < 8)
            return "La contraseña debe tener al menos 8 caracteres.";

        bool tieneMayus = valor.Any(char.IsUpper);
        bool tieneMinus = valor.Any(char.IsLower);
        bool tieneNumero = valor.Any(char.IsDigit);
        bool tieneSimbolo = valor.Any(ch => !char.IsLetterOrDigit(ch));
        
        if (tieneMayus && tieneMinus && tieneNumero && tieneSimbolo)
            return string.Empty;

        return "Debe incluir mayúscula, minúscula, número y un símbolo.";
    }
}

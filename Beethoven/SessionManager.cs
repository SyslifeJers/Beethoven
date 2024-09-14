using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beethoven
{
    public static class SessionManager
    {
        // Propiedad estática para almacenar el idioma actual (por defecto inglés)
        public static string CurrentLanguage { get; set; } = "es-ES"; // Idioma predeterminado

        // Método opcional para cambiar el idioma, si necesitas lógica adicional
        public static void ChangeLanguage(string newLanguage)
        {
            if (!string.IsNullOrEmpty(newLanguage))
            {
                CurrentLanguage = newLanguage;
                Console.WriteLine("Idioma cambiado a: " + CurrentLanguage);
            }
        }
    }
}

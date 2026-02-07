# Permisos, Licencias y Riesgos – Explorador de Archivos Cifrados

## Permisos y autorizaciones necesarias

- **.NET 8 y WPF:**  
  Tecnologías de Microsoft de uso gratuito para desarrollo y distribución.

- **Librerías de cifrado (System.Security.Cryptography):**  
  Incluidas en .NET, no requieren licencias adicionales.

- **GitHub:**  
  Uso gratuito para repositorios públicos o privados con cuenta personal.

- **Sistema Windows:**  
  La aplicación se ejecuta en entorno Windows, sin permisos especiales más allá del acceso a archivos del usuario.

---

## Identificación de riesgos

### Riesgos técnicos
- Fallos en el sistema de cifrado.
- Pérdida de datos si ocurre un error al guardar el archivo contenedor.
- Problemas de rendimiento con archivos grandes.

### Riesgos organizativos
- Falta de tiempo para completar todas las funcionalidades.
- Errores por trabajar en solitario sin revisión externa.

---

## 🛡️ Plan básico de prevención y medidas correctoras

| Riesgo                          | Prevención                          | Medida correctora                     |
|---------------------------------|-------------------------------------|---------------------------------------|
| Error en cifrado                | Pruebas unitarias y validación      | Restaurar desde copia de seguridad    |
| Pérdida de datos                | Guardado con versiones temporales   | Recuperar versión anterior            |
| Falta de tiempo                 | Priorizar funciones clave           | Reducir alcance                       |
| Errores por trabajo individual  | Revisiones frecuentes               | Refactorizar y documentar             |


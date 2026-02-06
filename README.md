# Explorador de Archivos Cifrados (.NET 8 WPF)

## Descripción del proyecto

### Idea general de la aplicación  
La aplicación es un explorador de archivos para Windows desarrollado con .NET 8 y WPF en C#. Permite trabajar con un único archivo cifrado que actúa como un contenedor seguro. El usuario puede abrir ese archivo, descifrar su contenido y visualizarlo como si fuera un explorador de archivos tradicional (carpetas y ficheros), pudiendo añadir, modificar y eliminar elementos.  
Todos los cambios se guardan nuevamente cifrados dentro del mismo archivo.

### Problema que resuelve  
En muchos casos se necesitan almacenar archivos de forma segura sin depender de servicios externos o carpetas visibles en el sistema.  
Esta aplicación permite:
- Proteger información sensible.
- Centralizar muchos archivos en un único contenedor cifrado.
- Evitar accesos no autorizados a los datos.

### Público objetivo  
- Usuarios que necesitan proteger documentos personales.
- Estudiantes o profesionales que gestionan información sensible.
- Cualquier persona que quiera un “explorador privado” cifrado en su PC.

---

## Funcionalidades principales

- Apertura de un archivo contenedor cifrado.
- Descifrado del contenido mediante clave.
- Visualización de archivos y carpetas en formato explorador.
- Creación, edición y eliminación de archivos/carpetas.
- Guardado automático del contenido cifrado.
- Interfaz gráfica en WPF para uso intuitivo.

---

## Tecnologías utilizadas

- **C# (.NET 8):** Lenguaje principal de desarrollo.
- **WPF:** Interfaz gráfica para Windows.
- **Criptografía (System.Security.Cryptography):** Para cifrar y descifrar el contenedor.
- **GitHub:** Repositorio y control de versiones.

---

## Estado actual del proyecto

🔹 Versión inicial (PoC):  
- Apertura de archivo cifrado.  
- Descifrado básico.  
- Interfaz preliminar tipo explorador.  
- Funciones mínimas para probar el concepto.


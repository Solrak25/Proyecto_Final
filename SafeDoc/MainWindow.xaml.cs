using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SafeDoc
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedUICommand CompartirCommand = new RoutedUICommand(
            "Compartir", "Compartir", typeof(MainWindow));

        private Carpeta raiz = null!;
        private Carpeta carpetaActual = null!;
        private List<Elemento> clipboardElementos = new List<Elemento>();
        private string passwordActual;
        private string usuarioActual;

        public MainWindow(string username, string password)
        {
            InitializeComponent();

            usuarioActual = username;
            passwordActual = password;

            try
            {
                raiz = GestorArchivos.CargarCarpetaUsuario(username, password);
            }
            catch
            {
                MessageBox.Show("Error al cargar perfil.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            carpetaActual = raiz;

            // Inicialización de muestra por primera vez
            if (raiz.Contenido.Count == 0)
            {
                var carpeta1 = new Carpeta("Mis Documentos", raiz);
                var archivo1 = new Archivo("ClavesSecretas.txt", raiz);

                raiz.Contenido.Add(carpeta1);
                raiz.Contenido.Add(archivo1);
            }

            Abrir(carpetaActual);
        }

        // ---------------------------
        // 📂 NAVEGACIÓN Y REFRESCO
        // ---------------------------
        public void Abrir(Carpeta carpeta)
        {
            carpetaActual = carpeta;
            RefrescarVista();
        }

        private void RefrescarVista()
        {
            // Forzar actualización del enlace de datos en WPF
            WPExploradorItems.ItemsSource = null;
            WPExploradorItems.ItemsSource = carpetaActual.Contenido;
            
            // Construir visualizador de la ruta (Breadcrumbs)
            string rutaStr = carpetaActual.Nombre;
            Carpeta? c = carpetaActual.CarpetaMadre;
            while(c != null)
            {
                rutaStr = c.Nombre + " > " + rutaStr;
                c = c.CarpetaMadre;
            }
            txtRuta.Text = rutaStr;
        }

        // Escuchar doble click para abrir carpetas
        private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (item.Content is Carpeta carpeta)
                {
                    Abrir(carpeta);
                }
                else if (item.Content is Archivo archivo)
                {
                    EditorArchivoWindow editor = new EditorArchivoWindow(archivo);
                    editor.Owner = this;
                    editor.ShowDialog();
                }
            }
        }

        // Navegar atrás
        private void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (carpetaActual.CarpetaMadre != null)
                Abrir(carpetaActual.CarpetaMadre);
        }

        // ---------------------------
        // ✂️ COMANDOS DE PORTAPAPELES
        // ---------------------------
        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WPExploradorItems.SelectedItems.Count > 0)
            {
                clipboardElementos.Clear();
                var aCortar = WPExploradorItems.SelectedItems.Cast<Elemento>().ToList();
                foreach (var elem in aCortar)
                {
                    clipboardElementos.Add(elem);
                    carpetaActual.Contenido.Remove(elem);
                }
                RefrescarVista();
            }
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WPExploradorItems.SelectedItems.Count > 0)
            {
                clipboardElementos.Clear();
                foreach (Elemento elem in WPExploradorItems.SelectedItems)
                {
                    clipboardElementos.Add(elem);
                }
            }
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (clipboardElementos.Count == 0) return;

            foreach (var clipElem in clipboardElementos)
            {
                if (clipElem is Archivo a)
                {
                    var nuevo = new Archivo(a.Nombre + " - Copia", carpetaActual)
                    {
                        ContenidoTexto = a.ContenidoTexto
                    };
                    carpetaActual.Contenido.Add(nuevo);
                }
                else if (clipElem is Carpeta c)
                {
                    var copia = CopiarCarpeta(c, carpetaActual);
                    copia.Nombre += " - Copia";
                    carpetaActual.Contenido.Add(copia);
                }
            }

            RefrescarVista();
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WPExploradorItems.SelectedItems.Count > 0)
            {
                string msg = WPExploradorItems.SelectedItems.Count == 1 
                    ? $"¿Estás seguro de que deseas eliminar permanentemente '{((Elemento)WPExploradorItems.SelectedItems[0]).Nombre}'?"
                    : $"¿Estás seguro de que deseas eliminar permanentemente los {WPExploradorItems.SelectedItems.Count} elementos seleccionados?";

                var confirmacion = MessageBox.Show(msg,
                                                   "Confirmar Eliminación", 
                                                   MessageBoxButton.YesNo, 
                                                   MessageBoxImage.Warning);
                
                if (confirmacion == MessageBoxResult.Yes)
                {
                    var aBorrar = WPExploradorItems.SelectedItems.Cast<Elemento>().ToList();
                    foreach (var elem in aBorrar)
                    {
                        carpetaActual.Contenido.Remove(elem);
                    }
                    RefrescarVista();
                }
            }
        }

        private Carpeta CopiarCarpeta(Carpeta original, Carpeta madre)
        {
            Carpeta copia = new Carpeta(original.Nombre, madre);

            foreach (var elem in original.Contenido)
            {
                if (elem is Archivo a)
                {
                    var archivoCopia = new Archivo(a.Nombre, copia);
                    archivoCopia.ContenidoTexto = a.ContenidoTexto;
                    copia.Contenido.Add(archivoCopia);
                }
                else if (elem is Carpeta c)
                {
                    copia.Contenido.Add(CopiarCarpeta(c, copia));
                }
            }
            return copia;
        }

        // ---------------------------
        // 🆕 ACCIONES EXTRA
        // ---------------------------
        private void BtnNuevaCarpeta_Click(object sender, RoutedEventArgs e)
        {
            InputDialog dialog = new InputDialog("Ingresa un nombre para la nueva carpeta:");
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Respuesta))
            {
                carpetaActual.Contenido.Add(new Carpeta(dialog.Respuesta, carpetaActual));
                RefrescarVista();
            }
        }

        private void BtnNuevoArchivo_Click(object sender, RoutedEventArgs e)
        {
            InputDialog dialog = new InputDialog("Ingresa un nombre para el nuevo archivo:");
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Respuesta))
            {
                string nombre = dialog.Respuesta;
                if (!nombre.Contains(".")) nombre += ".txt"; // Agregamos extensión por limpieza visual

                carpetaActual.Contenido.Add(new Archivo(nombre, carpetaActual));
                RefrescarVista();
            }
        }

        private void BtnCompartir_Click(object sender, ExecutedRoutedEventArgs e)
        {
            if (WPExploradorItems.SelectedItem is Carpeta carpeta)
            {
                try
                {
                    string codigo = GestorArchivos.CompartirCarpeta(carpeta);
                    MessageBox.Show($"Carpeta compartida!\nEnvía este código:\n\n{codigo}", "Compartir", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al compartir: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Selecciona una carpeta para compartir.");
            }
        }

        private void BtnRecibir_Click(object sender, RoutedEventArgs e)
        {
            InputDialog dialog = new InputDialog("Ingresa el código compartido:");
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Respuesta))
            {
                try
                {
                    Carpeta recibida = GestorArchivos.ImportarCarpeta(dialog.Respuesta);
                    recibida.CarpetaMadre = carpetaActual;
                    carpetaActual.Contenido.Add(recibida);
                    RefrescarVista();
                    MessageBox.Show("Carpeta recibida con éxito!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al recibir: " + ex.Message);
                }
            }
        }

        // ---------------------------
        // 💾 AUTO-GUARDADO
        // ---------------------------
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            GestorArchivos.GuardarCarpetaUsuario(usuarioActual, passwordActual, raiz);
        }

        // ---------------------------
        // 🖱️ SELECCIÓN POR ARRASTRE
        // ---------------------------
        private bool isSelecting = false;
        private Point startPoint;

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source && FindVisualParent<ListBoxItem>(source) == null)
            {
                isSelecting = true;
                startPoint = e.GetPosition(selectionCanvas);
                
                selectionBox.Width = 0;
                selectionBox.Height = 0;
                Canvas.SetLeft(selectionBox, startPoint.X);
                Canvas.SetTop(selectionBox, startPoint.Y);
                selectionBox.Visibility = Visibility.Visible;
                
                if ((Keyboard.Modifiers & ModifierKeys.Control) == 0 && (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                {
                    WPExploradorItems.SelectedItems.Clear();
                }

                WPExploradorItems.CaptureMouse();
                e.Handled = true; 
            }
        }

        private void List_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                Point currentPoint = e.GetPosition(selectionCanvas);

                double x = Math.Min(currentPoint.X, startPoint.X);
                double y = Math.Min(currentPoint.Y, startPoint.Y);
                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);

                Canvas.SetLeft(selectionBox, x);
                Canvas.SetTop(selectionBox, y);
                selectionBox.Width = width;
                selectionBox.Height = height;

                Rect selectionRect = new Rect(x, y, width, height);

                foreach (var item in WPExploradorItems.Items)
                {
                    if (WPExploradorItems.ItemContainerGenerator.ContainerFromItem(item) is ListBoxItem listBoxItem)
                    {
                        Point itemPos = listBoxItem.TranslatePoint(new Point(0, 0), selectionCanvas);
                        Rect itemRect = new Rect(itemPos.X, itemPos.Y, listBoxItem.ActualWidth, listBoxItem.ActualHeight);

                        if (selectionRect.IntersectsWith(itemRect))
                        {
                            listBoxItem.IsSelected = true;
                        }
                        else if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                        {
                            listBoxItem.IsSelected = false;
                        }
                    }
                }
            }
        }

        private void List_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting)
            {
                isSelecting = false;
                selectionBox.Visibility = Visibility.Collapsed;
                WPExploradorItems.ReleaseMouseCapture();
            }
        }

        private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindVisualParent<T>(parentObject);
        }
    }
}
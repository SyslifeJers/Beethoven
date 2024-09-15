using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Threading;


namespace Beethoven
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Musica GetMusica;
        List<string> tagsList = new List<string>
{
    "BonnPage", "Clases", "Musico", "Independiente", "Testamento",
    "Heroica", "Apogeo", "Ultimos", "Sobrino", "amigos",
    "BethoveenyGoet", "Cuaderno", "Casa", "Obras",
    "Literatura", "FuenteImagenes"
};
        private MediaPlayer mediaPlayer;
        private string[] audioFiles;
        private int currentFileIndex = 0;
        private DispatcherTimer _inactivityTimer;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;

            mediaPlayer = new MediaPlayer();
            GetMusica = new Musica();
            // Reproducir el archivo menu.mp3 al iniciar la aplicación
            PlayMenuAudio();
            SessionManager.CurrentLanguage = "es-ES";
            LoadLanguage(SessionManager.CurrentLanguage);
           
            _inactivityTimer = new DispatcherTimer();
            _inactivityTimer.Interval = TimeSpan.FromMinutes(5); // 5 minutos de inactividad
            _inactivityTimer.Tick += InactivityTimer_Tick;

            // Suscribirse a los eventos de entrada del usuario
            this.MouseMove += ResetInactivityTimer;
            this.KeyDown += ResetInactivityTimer;

            // Iniciar el temporizador
            _inactivityTimer.Start();
        }
        // Método que se ejecuta cuando el temporizador alcanza los 5 minutos de inactividad
        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            // Ejecuta el método que desees
            EjecutarMetodoPorInactividad();

            // Reinicia el temporizador para seguir monitoreando
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }

        // Restablece el temporizador cuando hay actividad del usuario
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop(); // Detener el temporizador
            _inactivityTimer.Start(); // Reiniciar el temporizador
        }

        // Método que se ejecuta cuando se detecta inactividad
        private void EjecutarMetodoPorInactividad()
        {
            ContentFrame.Content = null;
            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Recursos/saver.jpg")));
            // Aquí puedes colocar el código que deseas ejecutar
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Si es posible retroceder, ejecuta el comando de navegación hacia atrás
            if (ContentFrame.CanGoBack)
            {
                GetMusica.StopMenuAudio();
                ContentFrame.GoBack();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var currentPage = ContentFrame.Content as Page;

            // Si es posible avanzar, ejecuta el comando de navegación hacia adelante
            if (ContentFrame.CanGoForward)
            {
                ContentFrame.GoForward();
            }else
            {
                if (currentPage == null)
                {
                    goNavi("BonnPage");
                }
                else
                {
                    string currentTag = GetCurrentPageTag(currentPage);

                    // Encontrar el índice del tag actual en la lista
                    int currentIndex = tagsList.IndexOf(currentTag);

                    // Si se encontró el tag actual en la lista y no es el último, navegar al siguiente
                    if (currentIndex >= 0 && currentIndex < tagsList.Count - 1)
                    {
                        string nextTag = tagsList[currentIndex + 1];
                        goNavi(nextTag);  // Navega a la siguiente página usando su tag
                    }
                    else
                    {
                        
                        ContentFrame.Content = null;
                        this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Recursos/saver.jpg")));
                        CloseAllExpanders();
                        PlayMenuAudio();
                    }
                }
            }

        }
        private string GetCurrentPageTag(Page currentPage)
        {
            // Obtener el nombre del tipo de la página actual, por ejemplo, "BonnPage"
            return currentPage.GetType().Name;
        }
        private void FakeClick(object sender, MouseButtonEventArgs e)
        {
            // Verifica si el clic fue en un TreeViewItem
            var clickedElement = e.OriginalSource as DependencyObject;
            while (clickedElement != null && !(clickedElement is TreeViewItem))
            {
                clickedElement = VisualTreeHelper.GetParent(clickedElement);
            }

            if (clickedElement != null)
            {
                var item = (TreeViewItem)clickedElement;

                // Si el item no está seleccionado, selecciónalo
                if (!item.IsSelected)
                {
                    item.IsSelected = true;
                }
            }
        }


        private void LoadLanguage(string cultureCode)
        {
            var cultureInfo = new CultureInfo(cultureCode);

            // Cambiar el idioma del hilo actual
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // Limpiar los diccionarios de recursos existentes

            // Cargar el diccionario de recursos correspondiente al idioma seleccionado
            var dict = new ResourceDictionary();
            switch (cultureCode)
            {
                case "es-ES":
                    dict.Source = new Uri("Recursos\\idioma\\Menu.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Recursos\\idioma\\MenuEn.xaml", UriKind.Relative);
                    break;
            }
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(dict);
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = e.OriginalSource as TreeViewItem;

            if (selectedItem != null)
            {
                // Obtener el valor del Tag
                string tagValue = selectedItem.Tag as string;

                if (!string.IsNullOrEmpty(tagValue))
                {
                    // Llamar al método de manejo de selección
                    goNavi(tagValue);
                }
                selectedItem.IsSelected = false;

                // Evitar que el evento se propague a otros elementos
                e.Handled = true;
            }
        }

        private void PlayMenuAudio()
        {
            GetMusica.StopMenuAudio();
            string audio = "menu.mp3";
            if (SessionManager.CurrentLanguage != "es-ES")
            {
                audio = "menuIn.mp3";
            }
            // Ruta del recurso menu.mp3 en el proyecto
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;


            // Ruta completa al archivo menu.mp3 (asumiendo que está en Recursos/audio/Spanisch relativo al ejecutable)
            string audioFilePath = Path.Combine(exeDirectory, @"Recursos\" + audio);

            // Cargar y reproducir el archivo de audio
            mediaPlayer.Open(new Uri(audioFilePath, UriKind.Absolute));
            mediaPlayer.Play();
        }

        public void goNavi(string tag)
        {

            // Verificar si mediaPlayer está en reproducción de manera segura
            if (mediaPlayer.Position != TimeSpan.Zero &&
                mediaPlayer.NaturalDuration.HasTimeSpan && // Verificar si la duración es válida
                mediaPlayer.Position < mediaPlayer.NaturalDuration.TimeSpan)
            {
                mediaPlayer.Stop();
            }
            GetMusica.Pagina(tag);
            // Crear la nueva imagen de fondo
            var newBackground = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Recursos/fon2.jpg")));

            // Comprobar si la imagen de fondo actual es diferente antes de cambiarla
            if (!(this.Background is ImageBrush currentBackground) || currentBackground.ImageSource.ToString() != newBackground.ImageSource.ToString())
            {
                this.Background = newBackground;
            }

            // Navegar a la nueva página
            string pageName = tag;
            ContentFrame.Navigate(new Uri($"Vistas/{pageName}.xaml", UriKind.Relative));
        }
        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            // Asegurarse de que el sender es un Button
            if (sender is Button clickedButton)
            {
                // Obtener el Tag del botón
                string pageTag = clickedButton.Tag as string;

                if (!string.IsNullOrEmpty(pageTag))
                {
                    // Llamar a la función GoNavi para navegar a la página correspondiente
                    goNavi(pageTag);
                }
            }
        }
        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListBox.SelectedItem is ListBoxItem selectedItem)
            {
                // Obtén el Tag del elemento seleccionado
                var tag = selectedItem.Tag;

                // Llama al método goNavi pasándole el tag
                goNavi(tag.ToString());
                MenuListBox.SelectedItem = null;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.F12)
            {
                // Mostrar el mensaje de confirmación
                MessageBoxResult resultado = MessageBox.Show(
                    "¿Está seguro de que desea salir?", // Mensaje del diálogo
                    "Confirmar salida",                 // Título del diálogo
                    MessageBoxButton.YesNo,             // Botones de Sí y No
                    MessageBoxImage.Question);          // Icono de pregunta

                // Si el usuario selecciona "Sí", cerrar la aplicación
                if (resultado == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            
            if (SessionManager.CurrentLanguage == "es-ES")
            {
                SessionManager.CurrentLanguage = "en-US";
            }
            else
            {
                SessionManager.ChangeLanguage("es-ES");
            }
            LoadLanguage(SessionManager.CurrentLanguage);
            ContentFrame.Content = null;
            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Recursos/saver.jpg")));
            PlayMenuAudio();
            CloseAllExpanders();
        }

        private void Expander_Expanded1(object sender, RoutedEventArgs e)
        {
            if (Expander1.IsExpanded)
            {
                goNavi("BonnPage");
            }
        }
        private void Expander_Expanded2(object sender, RoutedEventArgs e)
        {
            if (Expander2.IsExpanded)
            {
                goNavi("Independiente");
            }
        }
        private void Expander_Expanded3(object sender, RoutedEventArgs e)
        {
            if (Expander3.IsExpanded)
            {
                goNavi("Sobrino");
            }
        }
        private void Expander_Expanded4(object sender, RoutedEventArgs e)
        {
            if (Expander4.IsExpanded)
            {
                goNavi("Cuaderno");
            }
        }
        private void CloseAllExpanders()
        {
            // Cerrar cada Expander estableciendo IsExpanded en false
            Expander1.IsExpanded = false;
            Expander2.IsExpanded = false;
            Expander3.IsExpanded = false;
            Expander4.IsExpanded = false;
        }

       
    }
}
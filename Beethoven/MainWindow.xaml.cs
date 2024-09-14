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


namespace Beethoven
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer;
        private string[] audioFiles;
        private int currentFileIndex = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;
           // this.Topmost = true;

            mediaPlayer = new MediaPlayer();

            // Reproducir el archivo menu.mp3 al iniciar la aplicación
            PlayMenuAudio();
            FamilyChildhoodItem.Selected += TreeViewItem_Selected;
            TuitionNeefeItem.Selected += TreeViewItem_Selected;
            MusicianBonnItem.Selected += TreeViewItem_Selected;

            SessionManager.CurrentLanguage = "es-ES";
            LoadLanguage(SessionManager.CurrentLanguage);
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

                // Evitar que el evento se propague a otros elementos
                e.Handled = true;
            }
        }

        private void PlayMenuAudio()
        {
            // Ruta del recurso menu.mp3 en el proyecto
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Ruta completa al archivo menu.mp3 (asumiendo que está en Recursos/audio/Spanisch relativo al ejecutable)
            string audioFilePath = Path.Combine(exeDirectory, @"Recursos\menu.mp3");

            // Cargar y reproducir el archivo de audio
            mediaPlayer.Open(new Uri(audioFilePath, UriKind.Absolute));
            mediaPlayer.Play();
        }


        public void goNavi(string tag)
        {
            mediaPlayer.Stop();

            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Recursos/fon2.jpg")));
            string pageName = tag;
            ContentFrame.Navigate(new Uri($"Vistas/{pageName}.xaml", UriKind.Relative));
        }
        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
        }
    }
}
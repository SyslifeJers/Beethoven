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
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Si es posible retroceder, ejecuta el comando de navegación hacia atrás
            if (ContentFrame.CanGoBack)
            {
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
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;

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
                selectedItem.IsSelected = false;

                // Evitar que el evento se propague a otros elementos
                e.Handled = true;
            }
        }

        private void PlayMenuAudio()
        {
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
            mediaPlayer.Stop();

            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Recursos/fon2.jpg")));
            string pageName = tag;
            ContentFrame.Navigate(new Uri($"Vistas/{pageName}.xaml", UriKind.Relative));
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
        }

        private void MyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = sender as TreeView;

            if (treeView != null)
            {
                // Obtener el elemento seleccionado

                    // Aquí no llamamos a goNavi ni realizamos ninguna acción adicional

                    // Después de procesar, deselecciona el item
                    treeView.SelectedItemChanged -= MyTreeView_SelectedItemChanged; // Desconectar temporalmente el evento
             
                    treeView.SelectedItemChanged += MyTreeView_SelectedItemChanged; // Reconectar el evento

            }
        }
    }
}
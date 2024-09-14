using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Beethoven.Vistas
{
    /// <summary>
    /// Lógica de interacción para FuenteImagenes.xaml
    /// </summary>
    public partial class FuenteImagenes : Page
    {
        // Tamaños originales de las imágenes
        private const double OriginalWidth = 200;
        private const double OriginalHeight = 250;

        // Tamaño al que se agranda la imagen
        private const double EnlargedWidth = 400;
        private const double EnlargedHeight = 500;

        private const double OriginalMargin = 50;
        public FuenteImagenes()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;

            // Verificamos el tamaño actual de la imagen para saber si debe agrandarse o reducirse
            if (image.Width == OriginalWidth && image.Height == OriginalHeight)
            {
                // Animar el tamaño para agrandarlo y reducir el margen
                AnimateImageSizeAndMargin(image, EnlargedWidth, EnlargedHeight, 0);
            }
            else
            {
                // Animar el tamaño para reducirlo y restaurar el margen original
                AnimateImageSizeAndMargin(image, OriginalWidth, OriginalHeight, OriginalMargin);
            }
        }

        private void AnimateImageSizeAndMargin(Image image, double newWidth, double newHeight, double newMargin)
        {
            // Animar el ancho de la imagen
            var widthAnimation = new DoubleAnimation
            {
                From = image.Width,
                To = newWidth,
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };

            // Animar el alto de la imagen
            var heightAnimation = new DoubleAnimation
            {
                From = image.Height,
                To = newHeight,
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };

            // Animar el margen (se anima el Left y Right simultáneamente para reducir el margen a 0)
            var marginAnimation = new ThicknessAnimation
            {
                From = image.Margin,
                To = new Thickness(newMargin),
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };

            // Aplicar las animaciones a las propiedades Width, Height y Margin de la imagen
            image.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation);
            image.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
            image.BeginAnimation(FrameworkElement.MarginProperty, marginAnimation);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Beethoven
{
    public class Musica
    {
    //        "BonnPage", "Clases", "Musico", "Independiente", "Testamento",
    //"Heroica", "Apogeo", "Ultimos", "Sobrino", "amigos",
    //"BethoveenyGoet", "Cuaderno", "Casa", "Obras",
    //"Literatura", "FuenteImagenes"
        MediaPlayer mediaPlayer;
        public Musica()
        {
            mediaPlayer = new MediaPlayer();
        }
        public void Pagina(string tag)
        {
            switch (tag)
            {
                case "BonnPage":
                    PlayMenuAudio("nines");
                    break;
                case "Clases":
                    PlayMenuAudio("viertesjahr");
                    break;
                case "Musico":
                    PlayMenuAudio("freyheit");
                    break;    
                case "Independiente":
                    PlayMenuAudio("vonwem");
                    break;  
                case "Testamento":
                    PlayMenuAudio("schicksal");
                    break;     
                case "Heroica":
                    PlayMenuAudio("schweine");
                    break;     
                case "Apogeo":
                    PlayMenuAudio("freiheit");
                    break;     
                case "Ultimos":
                    PlayMenuAudio("letztejahre");
                    break;     
                case "Sobrino":
                    PlayMenuAudio("leiblich");
                    break;     
                case "BethoveenyGoet":
                    PlayMenuAudio("zusammengerafft");
                    break;
                default:
                    mediaPlayer.Stop();
                    break;

            }
        }
        public void StopMenuAudio()
        {
            mediaPlayer.Stop();
        }

            private void PlayMenuAudio(string audio)
        {
            mediaPlayer.Stop();
            
            if (SessionManager.CurrentLanguage != "es-ES")
            {
                audio = $"{audio}_en.mp3";
            }
            else
            {
                audio = $"{audio}.mp3";
            }
            // Ruta del recurso menu.mp3 en el proyecto
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;


            // Ruta completa al archivo menu.mp3 (asumiendo que está en Recursos/audio/Spanisch relativo al ejecutable)
            string audioFilePath = Path.Combine(exeDirectory, @"Recursos\audio\" + audio);

            // Cargar y reproducir el archivo de audio
            mediaPlayer.Open(new Uri(audioFilePath, UriKind.Absolute));
            mediaPlayer.Play();
        }
    }

}


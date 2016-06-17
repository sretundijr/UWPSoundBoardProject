using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPSoundBoard.Model
{
    public class SoundManager
    {

        public static void GetAllSounds(ObservableCollection<Sound> sounds)
        {
            var allSounds = getSounds();

            sounds.Clear();

            allSounds.ForEach(p => sounds.Add(p));
        }

        public static void GetSoundsByName(ObservableCollection<Sound> sounds, string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                var allSounds = getSounds();
                //eliminates the case issue when searching
                StringBuilder sb = new StringBuilder(name);
                sb[0] = char.ToUpper(sb[0]);
                name = sb.ToString();

                var filteredSounds = allSounds.Where(p => p.Name == name).ToList();
                sounds.Clear();
                filteredSounds.ForEach(p => sounds.Add(p));
            }
           
        }

        public static void GetSoundsByCategory(ObservableCollection<Sound> sounds, SoundCategory soundCategory)
        {
            var allSounds = getSounds();

            var filteredResults = allSounds.Where(p => p.Category == soundCategory).ToList();
            sounds.Clear();
            filteredResults.ForEach(p => sounds.Add(p));
        }

        private static List<Sound> getSounds()
        {
            var sounds = new List<Sound>();

            sounds.Add(new Sound("Cow", SoundCategory.Animals));
            sounds.Add(new Sound("Cat", SoundCategory.Animals));

            sounds.Add(new Sound("Gun", SoundCategory.Cartoons));
            sounds.Add(new Sound("Spring", SoundCategory.Cartoons));

            sounds.Add(new Sound("Clock", SoundCategory.Taunts));
            sounds.Add(new Sound("LOL", SoundCategory.Taunts));

            sounds.Add(new Sound("Ship", SoundCategory.Warnings));
            sounds.Add(new Sound("Siren", SoundCategory.Warnings));

            return sounds;
        }
    }
}

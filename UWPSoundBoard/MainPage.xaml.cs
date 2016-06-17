using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UWPSoundBoard.Model;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPSoundBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<Sound> Sounds;
        private List<string> Suggestions;
        private List<MenuItem> MenuItems;

        public MainPage()
        {
            this.InitializeComponent();
            Sounds = new ObservableCollection<Sound>();
            SoundManager.GetAllSounds(Sounds);

            MenuItems = new List<MenuItem>();
            MenuItems.Add(new MenuItem { IconFile = "Assets/Icons/animals.png", Category = SoundCategory.Animals });
            MenuItems.Add(new MenuItem { IconFile = "Assets/Icons/cartoon.png", Category = SoundCategory.Cartoons });
            MenuItems.Add(new MenuItem { IconFile = "Assets/Icons/taunt.png", Category = SoundCategory.Taunts });
            MenuItems.Add(new MenuItem { IconFile = "Assets/Icons/warning.png", Category = SoundCategory.Warnings });

            BackButton.Visibility = Visibility.Collapsed;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageSplitView.IsPaneOpen = !MainPageSplitView.IsPaneOpen;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        //this code populates the suggestions drop down in the search box
        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

            if (String.IsNullOrEmpty(sender.Text))
            {
                GoBack();
            }             
            else
            {
                SoundManager.GetAllSounds(Sounds);

                //eliminates the case issue when searching
                StringBuilder sb = new StringBuilder(sender.Text.ToString());
                sb[0] = char.ToUpper(sb[0]);
                sender.Text = sb.ToString();

                Suggestions = Sounds.Where(p => p.Name.StartsWith(sender.Text))
                    .Select(p => p.Name)
                    .ToList();
                SearchBox.ItemsSource = Suggestions;
            }

            
        }

        private void GoBack()
        {
            SoundManager.GetAllSounds(Sounds);
            MenuItemsListView.SelectedItem = null;
            BackButton.Visibility = Visibility.Collapsed;
            CategoryTextBlock.Text = "All Sounds";
            SearchBox.Text = string.Empty;
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SoundManager.GetSoundsByName(Sounds, sender.Text);
            MenuItemsListView.SelectedItem = null;
            BackButton.Visibility = Visibility.Visible;
            CategoryTextBlock.Text = sender.Text;
        }

        private void MenuItemsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItems = (MenuItem)e.ClickedItem;

            //filtered results by category
            CategoryTextBlock.Text = menuItems.Category.ToString();
            SoundManager.GetSoundsByCategory(Sounds, menuItems.Category);
            BackButton.Visibility = Visibility.Visible;
        }

        private void SoundGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var sound = (Sound)e.ClickedItem;

            //base uri gives the root of the project, sound.audiofile specifies the path
            MyMediaElement.Source = new Uri(this.BaseUri, sound.AudioFile);
        }

        //adds the drop feature over the grid to allow the user to drag and drop an audio file
        private async void SoundGridView_Drop(object sender, DragEventArgs e)
        {
            if(e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();

                if(items.Any())
                {
                    var storageFile = items[0] as StorageFile;
                    var contentType = storageFile.ContentType;

                    StorageFolder folder = ApplicationData.Current.LocalFolder;

                    if(contentType == "audio/wav" || contentType == "audio/mgeg")
                    {
                        StorageFile newFile = await storageFile.CopyAsync(folder, 
                            storageFile.Name, NameCollisionOption.GenerateUniqueName);

                        MyMediaElement.SetSource(await storageFile.OpenAsync(FileAccessMode.Read), contentType);
                        MyMediaElement.Play();
                    }

                }
            }
        }

        //shows a caption when the item dragged is over the grid
        private void SoundGridView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;

            e.DragUIOverride.Caption = "drop to create a custom sound and tile";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }
    }
}

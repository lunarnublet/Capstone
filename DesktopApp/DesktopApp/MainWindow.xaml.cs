using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp
{
     /// <summary>
     /// Interaction logic for MainWindow.xaml
     /// </summary>
     public partial class MainWindow : Window
     {
          Server server;
          public bool NewImage;
          public Dictionary<Image, string> imageDictionary;


          public MainWindow()
          {
               InitializeComponent();
               this.Focusable = true;
               server = new Server(this);
               imageDictionary = new Dictionary<Image, string>();
               server.OnStart();
               DisplayFileImages();
               NewImage = true;
               ListDirectories.SelectionChanged += ListDirectories_SelectionChanged; ;
               //ListDirectories.MouseDown += ListDirectories_MouseDown;
               ListDirectories.ItemsSource = server.ExistingDirs;
               ListDirectories.SelectedIndex = 0;

               CompositionTarget.Rendering += CompositionTarget_Rendering;
               FilePathTextBox.GotFocus += FilePathTextBox_GotFocus;
               FilePathTextBox.LostFocus += FilePathTextBox_LostFocus;
               FilePathTextBox.KeyDown += FilePathTextBox_KeyDown;



               Code.Content = server.Code;


               //System.Windows.Data.Binding b = new System.Windows.Data.Binding("M");
               //b.Source = eventListView;
               //b.Path = new PropertyPath(cell.Width);
               //b.Converter = new ListViewWidthConverter();
               //b.Mode = BindingMode.OneWay;
               //cell.SetBinding(ListView.ActualWidthProperty, b);
          }

          private void ToggleCodeClick(object sender, RoutedEventArgs e)
          {
               if (Code.Visibility == Visibility.Hidden)
               {
                    Code.Visibility = Visibility.Visible;
               }
               else
               {
                    Code.Visibility = Visibility.Hidden;
               }
          }

          private void ListDirectories_SelectionChanged(object sender, SelectionChangedEventArgs e)
          {
               int index = 0;
               if (ListDirectories.SelectedIndex > 0) 
               {
                    index = ListDirectories.SelectedIndex;
               }
               string s = server.ExistingDirs[index];
               ChangeDirectory(s);
          }

          //private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
          //{
          //     e.CanExecute = true;
          //}

          //private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
          //{
          //     string s = "";
          //}

          private void CompositionTarget_Rendering(object sender, EventArgs e)
          {
               if (NewImage)
               {
                    Refresh();
                    NewImage = false;
               }
          }

          private void Server_newPhoto(string filepath)
          {
               AddImageToImagePanel(new Uri(filepath));
          }

          private void FilePathTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
          {
               if (e.Key == Key.Enter)
               {
                    ChangeDirectory(FilePathTextBox.Text);
               }
          }

          private void FilePathTextBox_LostFocus(object sender, RoutedEventArgs e)
          {
               if (FilePathTextBox.Text == "")
               {
                    FilePathTextBox.Text = "Ex. 'D:\\YourFolderName'";
               }
          }

          private void FilePathTextBox_GotFocus(object sender, RoutedEventArgs e)
          {
               if (FilePathTextBox.Text == "Ex. 'D:\\YourFolderName'")
               {
                    FilePathTextBox.Text = "";
               }
          }

          protected override void OnClosed(EventArgs e)
          {
               base.OnClosed(e);
               server.EndCalls();
          }

          private void ChangeDirectoryClick(object sender, RoutedEventArgs e)
          {
               string filepath = FilePathTextBox.Text;
               ChangeDirectory(filepath);
          }

          private void ChangeDirectory(string directory)
          {
               if (Directory.Exists(directory))
               {
                    //ErrorLabel.Visibility = Visibility.Hidden;
                    server.SavePath = directory;
                    server.AddDirectory(directory);
                    Refresh();
                    int index = server.GetIndexOf(directory);
                    ListDirectories.SelectedIndex = index == -1 ? 0 : index;
               }
               else
               {
                    //ErrorLabel.Visibility = Visibility.Visible;
                    //ErrorLabel.Content = "\"" + filepath + "\" is an invalid directory.";
               }
          }

          private void SelectFolderClick(object sender, RoutedEventArgs e)
          {
               using (var fbd = new FolderBrowserDialog())
               {
                    DialogResult result = fbd.ShowDialog();

                    if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                         FilePathTextBox.Text = fbd.SelectedPath;
                         FilePathTextBox.Focus();
                         ChangeDirectory(FilePathTextBox.Text);
                    }
               }
          }

          private void RefreshClick(object sender, RoutedEventArgs e)
          {
               Refresh();
          }

          private void Refresh()
          {
               ImagePanel.Children.Clear();
               imageDictionary.Clear();
               DisplayFileImages();
          }

          private void DisplayFileImages()
          {
               string searchpattern = "*.*";
               string path = server.SavePath;
               var files = Directory.GetFiles(path, searchpattern, SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png"));
               foreach (string filename in files)
               {
                    AddImageToImagePanel(new Uri(filename));
               }
          }

          private void AddImageToImagePanel(Uri uri)
          {
               Image image = new Image();
               image.Width = 100;
               image.Height = 100;
               image.Margin = new Thickness(5);
               image.Source = new BitmapImage(uri);
               image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
               ImagePanel.Children.Add(image);
               imageDictionary.Add(image, uri.AbsolutePath);
          }

          private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
          {
               if (e.ClickCount == 2) 
               {
                    Image image = (Image)sender;
                    string path = "";
                    imageDictionary.TryGetValue(image, out path);
                    if (path != "") 
                    {
                         try
                         {
                              System.Diagnostics.Process.Start(path);

                         }
                         catch (Exception ex) 
                         {
                         
                         }
                    }
               }

          }

     }
}

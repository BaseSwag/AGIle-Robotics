using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageClassifier
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if(!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            Reset();
            remainingFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.jpg").ToList();
            if(remainingFiles.Count > 0)
            {
                todo = true;
                LoadNextImage();
                CycleImages();
                LoadNextImage();
            }
        }

        string outputPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Output");
        bool isDragging = false;
        Point point1 = new Point();
        Point point2 = new Point();
        Point imagePoint1 = new Point();
        Point imagePoint2 = new Point();
        (double, double) rectSize = (-1, -1);
        string currentFile;
        BitmapImage currentImage;
        string nextFile;
        BitmapImage nextImage;
        List<string> remainingFiles;
        bool todo = false;

        private void LearningPicture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            point1 = e.GetPosition(DrawingPlane);
            imagePoint1 = e.GetPosition(LearningPicture);
            isDragging = true;
        }

        private void LearningPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                ClassifierRect.Visibility = Visibility.Visible;
                point2 = e.GetPosition(DrawingPlane);
                imagePoint2 = e.GetPosition(LearningPicture);
                ClassifierRect.SetValue(Canvas.LeftProperty, Math.Min(point1.X, point2.X));
                ClassifierRect.SetValue(Canvas.TopProperty, Math.Min(point1.Y, point2.Y));

                rectSize = (Math.Abs(point2.X - point1.X), Math.Abs(point2.Y - point1.Y));
                ClassifierRect.Width = rectSize.Item1;
                ClassifierRect.Height = rectSize.Item2;
            }
            var xy = e.GetPosition(LearningPicture);
            Console.WriteLine($"X:{xy.X} | Y:{xy.Y}");
        }

        private void LearningPicture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void LearningPicture_MouseLeave(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(LearningPicture);
            if (p.X < 0 || p.Y < 0 || p.X > LearningPicture.Width || p.Y > LearningPicture.Height)
            {
                isDragging = false;
            }
        }

        private void DrawingPlane_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                e.Handled = true;
                Next();
            }
        }

        void Next()
        {
            try
            {
                var oldImage = currentImage;
                Point topleft = new Point(Math.Min(imagePoint1.X, imagePoint2.X), Math.Min(imagePoint1.Y, imagePoint2.Y));
                var size = LearningPicture.RenderSize;
                topleft.X /= size.Width;
                topleft.Y /= size.Height;
                double width = rectSize.Item1 / size.Width;
                double height = rectSize.Item2 / size.Height;

                string classification = $"AGIBot {topleft.X} {topleft.Y} {width} {height}";
                string path = currentFile;
                string newPath = System.IO.Path.Combine(outputPath, System.IO.Path.GetFileName(path));
                File.Move(path, newPath);
                newPath = System.IO.Path.ChangeExtension(newPath, ".txt");
                File.WriteAllText(newPath, classification);

                Reset();

                if (todo)
                {
                    CycleImages();
                    if(!LoadNextImage())
                    {
                        todo = false;
                        MessageBox.Show("LastImage!");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void Reset()
        {
            rectSize = (-1, -1);
            point1 = new Point(-1, -1);
            point2 = new Point(-1, -1);
            imagePoint1 = new Point(-1, -1);
            imagePoint2 = new Point(-1, -1);
            ClassifierRect.Visibility = Visibility.Collapsed;
        }

        void CycleImages()
        {
            currentImage = nextImage;
            currentFile = nextFile;
            LearningPicture.Source = currentImage;
        }

        bool LoadNextImage()
        {
            if(remainingFiles.Count > 0)
            {
                nextFile = System.IO.Path.GetFullPath(remainingFiles[0]);
                var uri = new Uri(nextFile, UriKind.RelativeOrAbsolute);

                var src = new BitmapImage();
                src.BeginInit();
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.CreateOptions = BitmapCreateOptions.None;
                src.DownloadFailed += delegate
                {
                    Console.WriteLine("Failed");
                };

                src.DownloadProgress += delegate
                {
                    Console.WriteLine("Progress");
                };

                src.DownloadCompleted += delegate
                {
                    Console.WriteLine("Completed");
                };

                src.UriSource = uri;
                src.EndInit();
                nextImage = src;

                remainingFiles.RemoveAt(0);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

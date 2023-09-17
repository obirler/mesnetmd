using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MesnetMD.Classes.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MesnetMD.Classes.IO;
using MesnetMD.Classes.Ui.Som;
using MesnetMD.Xaml.Pages;
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.CloseLogger();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string[] arguments = System.Environment.GetCommandLineArgs();
            if (arguments.GetLength(0) > 1)
            {
                if (arguments[1].EndsWith(".mnt"))
                {
                    AssociationPath = arguments[1];
                }
            }
            Logger.InitializeLogger();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private void MinSize(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black);
            textBlock.Width = formattedText.Width;
            textBlock.Height = formattedText.Height;
        }

        private void RotateAround(TextBlock textBlock, Beam beam)
        {
            var rotate = new RotateTransform();
            rotate.CenterX = textBlock.Width / 2;
            rotate.CenterY = textBlock.Height / 2;
            rotate.Angle = -beam.Angle;
            textBlock.RenderTransform = rotate;
        }

        public static string AssociationPath = null;
    }
}

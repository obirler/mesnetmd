using System.Windows;
using System.Windows.Input;
using MesnetMD.Classes;

namespace MesnetMD.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for MessagePrompt.xaml
    /// </summary>
    public partial class MessagePrompt : Window
    {
        public MessagePrompt(string message)
        {
            InitializeComponent();
            Message.Text = message;
        }

        public Global.DialogResult Result = Global.DialogResult.None;

        private void yesbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Yes;
            DialogResult = true;
        }

        private void nobtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.No;
            DialogResult = true;
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Result = Global.DialogResult.Cancel;
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Result = Global.DialogResult.Yes;
                DialogResult = true;
            }
        }
    }
}

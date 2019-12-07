using System;
using System.Windows;
using MesnetMD.Classes.Tools;
using MesnetMD.Classes.Ui.Base;

namespace MesnetMD.Xaml.Pages
{
    /// <summary>
    /// Interaction logic for DirectLoadPrompt.xaml
    /// </summary>
    public partial class DirectLoadPrompt : Window
    {
        public DirectLoadPrompt()
        {
            InitializeComponent();
        }

        public DirectLoadPrompt(FreeSupportItem support)
        {
            InitializeComponent();
        }

        public DirectLoad _loads;

        public DirectLoad Loads
        {
            get { return _loads; }
        }

        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void finishbtn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }   
    }
}

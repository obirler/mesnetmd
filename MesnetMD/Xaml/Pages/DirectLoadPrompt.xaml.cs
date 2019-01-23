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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

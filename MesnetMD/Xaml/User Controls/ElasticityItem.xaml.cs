using System.Windows.Controls;

namespace MesnetMD.Xaml.User_Controls
{
    /// <summary>
    /// Interaction logic for ElasticityItem.xaml
    /// </summary>
    public partial class ElasticityItem : UserControl
    {
        public ElasticityItem(string name)
        {
            InitializeComponent();
            elasticity.Text = name;
        }
    }
}

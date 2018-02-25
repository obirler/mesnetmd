using System.Windows.Controls;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes.Ui
{
    public class TreeViewBeamItem : TreeViewItem
    {
        public TreeViewBeamItem(Beam beam)
        {
            _beam = beam;
        }

        private Beam _beam;

        public Beam Beam
        {
            get { return _beam; }
        }
    }
}

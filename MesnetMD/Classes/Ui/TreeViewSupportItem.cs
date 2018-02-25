using System.Windows.Controls;

namespace MesnetMD.Classes.Ui
{
    public class TreeViewSupportItem:TreeViewItem
    {
        public TreeViewSupportItem(object support)
        {
            _support = support;
        }

        private object _support;

        public object Support
        {
            get { return _support; }
        }
    }
}

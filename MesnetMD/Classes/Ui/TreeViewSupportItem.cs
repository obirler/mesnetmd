using System.Windows.Controls;

namespace MesnetMD.Classes.Ui
{
    public class TreeViewSupportItem:TreeViewItem
    {
        public TreeViewSupportItem(SupportItem support)
        {
            _support = support;
        }

        private SupportItem _support;

        public SupportItem Support
        {
            get { return _support; }
        }
    }
}

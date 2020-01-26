using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace MesnetMD.Classes.Ui.Base
{
    public abstract class GraphicItem : UiItem
    {
        protected GraphicItem()
        {
        }

        protected void MinSize(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black);
            textBlock.Width = formattedText.Width;
            textBlock.Height = formattedText.Height;
        }

        protected void RotateAround(TextBlock textBlock, double angle)
        {
            var rotate = new RotateTransform();
            rotate.CenterX = textBlock.Width / 2;
            rotate.CenterY = textBlock.Height / 2;
            rotate.Angle = -angle;
            textBlock.RenderTransform = rotate;
        }

        protected TextBlock createtextblock()
        {
            var tbl = new TextBlock();
            var reverse = new ScaleTransform();
            reverse.CenterY = 0.5;
            reverse.CenterY = 0.5;
            reverse.ScaleX = 1;
            reverse.ScaleY = -1;
            tbl.LayoutTransform = reverse;
            return tbl;
        }
    }
}

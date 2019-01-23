using System.Windows;
using System.Windows.Controls;
using MesnetMD.Classes.Ui.Som;

namespace MesnetMD.Classes
{
    public interface IGraphic
    {
        void Draw(int c);

        void Show();

        void Hide();

        void RemoveLabels();
    }

    public interface ISomItem
    {
        void BindEvents();

        void Select();

        void UnSelect();

        void ResetSolution();
    }

    public interface IRealSupportItem
    {
        void Add(Canvas canvas, double leftpos, double toppos);

        void UpdatePosition(Beam beam);

        void SetPosition(double x, double y);

        void SetPosition(Point point);

        void SetAngle(double angle);
    }

    public interface IFictionalSupportItem
    {
        void AddBeam(Beam beam, Global.Direction direction);

        void RemoveBeam(Beam beam);
    }

    public interface IFixedSupportItem
    {
        void AddBeam(Beam beam);
    }

    public abstract class ISimpsonIntegrator
    {
        public abstract void AddData(double data);
        public abstract void Calculate();
        private double Result;
    }
}
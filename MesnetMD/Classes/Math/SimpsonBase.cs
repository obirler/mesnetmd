using System.Collections.Generic;

namespace MesnetMD.Classes.Math
{
    public abstract class SimpsonBase
    {
        public SimpsonBase(Global.SimpsonIntegrationType type, double deltax)
        {
            Type = type;
            datas=new List<double>();
            _h = deltax;
        }

        public abstract void Calculate();

        public void AddData(double data)
        {
            datas.Add(data);
        }

        public Global.SimpsonIntegrationType Type;

        protected double _result;

        protected double _error;

        protected List<double> datas;

        protected double _h;

        protected double _sum;

        public double Result
        {
            get { return _result; }
        }

        public double Error
        {
            get { return _error; }
        }
    }
}

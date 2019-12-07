namespace MesnetMD.Classes.Math
{
    public class SimpsonsSecondIntegrator : SimpsonBase
    {
        public SimpsonsSecondIntegrator(double deltax) : base(Global.SimpsonIntegrationType.Second, deltax)
        {
        }

        public override void Calculate()
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (i == 0)
                {
                    _sum += datas[i];
                }
                else if (i == datas.Count - 1)
                {
                    _sum += datas[i];
                }
                else if (i % 3 == 0)
                {
                    _sum += 2 * datas[i];
                }
                else
                {
                    _sum += 3 * datas[i];
                }
            }
            _result = 3 * _h / 8 * _sum;

            _error = datas.Count * System.Math.Pow(_h, 5) / 6480;
        }
    }
}

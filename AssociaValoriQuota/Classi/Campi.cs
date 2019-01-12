using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociaValoriQuota.Classi
{
    public class Campi
    {
        private double m_campoEst;

        public double CampoEst
        {
            get { return m_campoEst; }
            set { m_campoEst = value; }
        }

        private double m_campoNord;

        public double CampoNord
        {
            get { return m_campoNord; }
            set { m_campoNord = value; }
        }

        private double m_campoQuota;

        public double CampoQuota
        {
            get { return m_campoQuota; }
            set { m_campoQuota = value; }
        }


        public Campi(double campoEst, double campoNord, double campoQuota)
        {
            CampoEst = campoEst;
            CampoNord = campoNord;
            CampoQuota = campoQuota;
        }

        public Campi()
        {

        }

        public string getCoordinatesWithSeparator(char separatore)
        {
            return (string.Format("{0:#0.000}", CampoEst) + separatore +
                        string.Format("{0:#0.000}", CampoNord) + separatore +
                        string.Format("{0:#0.000}", CampoQuota) + separatore);
        }
    }
}

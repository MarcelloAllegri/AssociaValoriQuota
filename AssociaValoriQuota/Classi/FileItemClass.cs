using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociaValoriQuota.Classi
{
    public class FileItemClass
    {
        public FileItemClass()
        {

        }

        private string m_Path;

        public string Path
        {
            get { return m_Path; }
            set { m_Path = value; }
        }

        private char m_FileDelimiter;

        public char FileDelimiter
        {
            get { return m_FileDelimiter; }
            set { m_FileDelimiter = value; }
        }

        private int m_CampoEst;

        public int CampoEst
        {
            get { return m_CampoEst; }
            set { m_CampoEst = value; }
        }

        private int m_CampoNord;

        public int CampoNord
        {
            get { return m_CampoNord; }
            set { m_CampoNord = value; }
        }

        private int m_CampoQuota;

        public int CampoQuota
        {
            get { return m_CampoQuota; }
            set { m_CampoQuota = value; }
        }


    }
}

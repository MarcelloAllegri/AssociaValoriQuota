using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssociaValoriQuota.Classi
{
    class CustomFileDescriptor
    {

        public CustomFileDescriptor()
        {
            
        }

        private char  m_frameDelimiter;
        private string m_fileName;
        private ComboBox[] m_comboBoxes;

        public ComboBox[] ComboBoxes
        {
            get { return m_comboBoxes; }
            set { m_comboBoxes = value; }
        }

        public string FileName
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }

        public char FrameDelimiter
        {
            get { return m_frameDelimiter; }
            set { m_frameDelimiter = value; }
        }

    }
}

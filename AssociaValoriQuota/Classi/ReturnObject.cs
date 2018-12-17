using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociaValoriQuota.Classi
{
    public class ReturnObject
    {
        private string m_Quota;

        public string Quota
        {
            get { return m_Quota; }
            set { m_Quota = value; }
        }

        private string m_itemToRemove;

        public string ItemToRemove
        {
            get { return m_itemToRemove; }
            set { m_itemToRemove = value; }
        }

        public ReturnObject(string quota,string itemToRemove)
        {
            this.Quota = quota;
            this.ItemToRemove = itemToRemove;
        }

    }
}

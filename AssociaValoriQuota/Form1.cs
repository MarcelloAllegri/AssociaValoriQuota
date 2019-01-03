using AssociaValoriQuota.Classi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AssociaValoriQuota

{

    public partial class Form1 : Form
    {
        char[] Delimiter;
        Boolean ToBeStopped;
        int ColumnsNumber;
        static string NORDlabel = "NORD";
        static string ESTlabel = "EST";
        static string ELLIlabel = "Q_ELLIS";
        static string ORTOlabel = "Q_ORTO";
        static string GENERIClabel = "GENERICO";
        string[] ElliList = new string[3];
        string[] OrtoList = new string[3];

        //cerco di mettere le combobox in due vettori
        public ComboBox[] ComboE = new ComboBox[10];
        public ComboBox[] ComboO = new ComboBox[10];

        string[] CoordinateE = { GENERIClabel, NORDlabel, ESTlabel, ELLIlabel };
        string[] CoordinateO = { GENERIClabel, NORDlabel, ESTlabel, ORTOlabel };

        char ELLI_delimiter, ORTO_delimiter;

        string Quota_2;
        double SearchRange;

        FileItemClass ElliFile; //--> Quote Ellisodiche
        FileItemClass OrtoFile; //--> Quote Ortometriche

        ConcurrentQueue<string> ElliQuoteList;
        ConcurrentQueue<string> OrtoQuoteList;
        //ELABORAZIONE DATI

        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Files di testo (*.txt)|*.txt|Files xyz (*.xyz)|*.xyz|All Files|*";
            openFileDialog1.FilterIndex = 3;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //assegno le combobox ai vettori
            ComboE[0] = comboBox1;
            ComboE[1] = comboBox2;
            ComboE[2] = comboBox3;
            ComboE[3] = comboBox4;
            ComboE[4] = comboBox5;
            ComboE[5] = comboBox6;
            ComboE[6] = comboBox7;
            ComboE[7] = comboBox8;
            ComboE[8] = comboBox9;
            ComboE[9] = comboBox10;
            //per le quote ortometriche
            ComboO[0] = comboBox20;
            ComboO[1] = comboBox19;
            ComboO[2] = comboBox18;
            ComboO[3] = comboBox17;
            ComboO[4] = comboBox16;
            ComboO[5] = comboBox15;
            ComboO[6] = comboBox14;
            ComboO[7] = comboBox13;
            ComboO[8] = comboBox12;
            ComboO[9] = comboBox11;

        }

        private Boolean DelimiterCheck()
        {
            ToBeStopped = false;
            if (radioButton1.Checked == false && radioButton2.Checked == false && radioButton3.Checked == false && radioButton4.Checked == false && radioButton5.Checked == false
                    && radioButton6.Checked == false)
            {
                MessageBox.Show("Devi prima selezionare un delimitatore di campo.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ToBeStopped = true;
            }
            return ToBeStopped;
        }

        private void GetDelimiter(Boolean ISelli_File)
        {
            if (radioButton1.Checked == true)
            {
                Delimiter = ".".ToCharArray();
            }
            else if (radioButton2.Checked == true)
            {
                Delimiter = ",".ToCharArray();
            }
            else if (radioButton3.Checked == true)
            {
                Delimiter = ";".ToCharArray();
            }
            else if (radioButton4.Checked == true)
            {
                Delimiter = " ".ToCharArray();
            }
            else if (radioButton5.Checked == true)
            {
                Delimiter = "\t".ToCharArray();
            }
            else if (radioButton6.Checked == true)
            {
                Delimiter = textBox1.Text.ToCharArray();
            }
            //memorizzo il delimitatore a seconda che riguardi il file delle quote ellissoidiche o quello delle quote ortometriche
            if (ISelli_File == true)
            {
                ELLI_delimiter = Delimiter[0];
            }
            else
            {
                ORTO_delimiter = Delimiter[0];
            }
        }

        private int CheckColumnsNumber(string FileName)
        {
            string[] text = System.IO.File.ReadAllLines(FileName);
            ColumnsNumber = 0;
            for (int x = 0; x <= text.GetUpperBound(0); x++)
            {
                string[] Columns = text[x].Split(Delimiter);
                if (ColumnsNumber < Columns.GetUpperBound(0))
                {
                    ColumnsNumber = Columns.GetUpperBound(0);
                }
            }
            Console.WriteLine(ColumnsNumber.ToString());
            return ColumnsNumber;
        }

        private void ComboBoxEnabler(ComboBox[] combo, string[] Coordinate)
        {
            //enable all comboboxes
            for (int i = 0; i < 10; i++)
            {
                if (ColumnsNumber >= i)
                {
                    combo[i].Enabled = true;
                    combo[i].Text = GENERIClabel;
                    for (int x = 0; x < 4; x++)
                    {
                        //add all values from the arrays
                        combo[i].Items.Add(Coordinate[x]);
                    }
                }
            }
        }

        private void GetFileInfo(string OpenFDTitle, Boolean ISelli_File)
        {
            //checking if the delimiter has been selected
            DelimiterCheck();
            //exit from the routine if ToBeStopped == True
            if (ToBeStopped == true)
            {
                return;
            }
            //If I'm still here, i can get the delimiter from the corresponding RadioButton
            GetDelimiter(ISelli_File);
            //setting the proper title for the openfiledialog
            openFileDialog1.Title = "Importa il file di coordinate ellissoidiche";
            //getting the result
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //I can start checking how many columns are in the file
                CheckColumnsNumber(openFileDialog1.FileName);
                //disabling comboboxes
                if (ISelli_File == true)
                {
                    //ELLI_File_Path = openFileDialog1.FileName;
                    ElliFile = new FileItemClass
                    {
                        FileDelimiter = ELLI_delimiter,
                        Path = openFileDialog1.FileName,
                    };

                    for (int x = 0; x < 10; x++)
                    {
                        if (ComboE[x].Text == ESTlabel)
                        {
                            ElliFile.CampoEst = x;
                        }
                        else if (ComboE[x].Text == NORDlabel)
                        {
                            ElliFile.CampoNord = x;
                        }
                        else if (ComboE[x].Text == ELLIlabel)
                        {
                            ElliFile.CampoQuota = x;
                        }
                    }

                    ComboBoxEnabler(ComboE, CoordinateE);
                }
                else
                {
                    OrtoFile = new FileItemClass
                    {
                        FileDelimiter = ORTO_delimiter,
                        Path = openFileDialog1.FileName,
                    };
                    //ORTO_File_Path = openFileDialog1.FileName;

                    for (int x = 0; x < 10; x++)
                    {
                        if (ComboO[x].Text == ESTlabel)
                        {
                            OrtoFile.CampoEst = x;
                        }
                        else if (ComboO[x].Text == NORDlabel)
                        {
                            OrtoFile.CampoNord = x;
                        }
                        else if (ComboO[x].Text == ELLIlabel)
                        {
                            OrtoFile.CampoQuota = x;
                        }
                    }

                    ComboBoxEnabler(ComboO, CoordinateO);
                }
            }
        }

        private void ResetComboboxes(ComboBox[] combo)
        {
            for (int x = 0; x < 10; x++)
            {
                combo[x].ResetText();
                combo[x].Items.Clear();
                combo[x].Enabled = false;
            }
        }

        private void ResetAllRadioButtons()
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetComboboxes(ComboE);
            GetFileInfo("Importa il file di coordinate ellissoidiche", true);
            try
            {
                ElliQuoteList = new ConcurrentQueue<string>(File.ReadLines(ElliFile.Path));
                ResetAllRadioButtons();
                MessageBox.Show("File quote ellissoidiche importato!");
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Errore:\n Eccezione-> " + Ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetComboboxes(ComboO);
            GetFileInfo("Importa il file di coordinate ortometriche", false);
            try
            {
                OrtoQuoteList = new ConcurrentQueue<string>(File.ReadLines(OrtoFile.Path));
                ResetAllRadioButtons();
                MessageBox.Show("File coordinate ortometriche importato!");
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Errore:\n Eccezione-> " + Ex.Message);
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void RimuoviElementoImportante(ComboBox[] combo, int ComboIndex, string ItemName)
        {
            //controllo se esiste il valore importante in tutte le combobox
            if (combo[ComboIndex].Text == ItemName)
            {
                //mi passo tutti le combobox (ad eccezione della corrente - i) e rimuovo il valore gia' trovato
                for (int x = 0; x < 10; x++)
                {
                    //se non si tratta della combobox nella quale ho trovato il valore selezionato, lo elimino dalla lista

                    if (x != ComboIndex)
                    {
                        combo[x].Items.Remove(ItemName);
                    }
                }
            }
        }

        private void AggiungiElementoImportante(ComboBox[] combo, string ItemName)
        {
            //AGGIUNGE UN ELEMENTO IMPORTANTE
            //quindi: mi passo tutte le combobox
            for (int i = 0; i < 10; i++)
            {
                //interrompo la procedura di addizione, qualora il valore importante fosse stato trovato SELEZIONATO in una combobox
                if (combo[i].Text == ItemName) { return; }
            }
            //NON HO TROVATO UN VALORE IMPORTANTE SELEZIONATO IN ALCUNA COMBOBOX
            for (int x = 0; x < 10; x++)
            {
                //per ogni combobox, verifico che il valore non sia gia' presente
                if (combo[x].Items.Contains(ItemName) == false)
                {
                    //NON E' PRESENTE NEI SELEZIONATI, NON E' PRESENTE NELLA COMBOBOX -> LO AGGIUNGO ALLA COMBOBOX
                    combo[x].Items.Add(ItemName);
                }
            }
        }

        private void ComboBOXChangEvent(bool ISelli_File, int Index)
        {
            ComboBox[] combo;
            string Quota;
            if (ISelli_File == true)
            {
                combo = ComboE;
                Quota = ELLIlabel;
            }
            else
            {
                combo = ComboO;
                Quota = ORTOlabel;
            }
            //controllo se è stato selezionato un valore importante
            if (combo[Index].Text != GENERIClabel)
            {
                //e' stato segnalato un valore univoco. rimuovo il valore dalle altre combobox
                //ma devo valutare che se ho cambiato valore alla combobox, uno dei valori importanti potrebbe essere rientrato nell'elenco dei valori da aggiungere a tutte le altre
                RimuoviElementoImportante(combo, Index, combo[Index].Text);
            }
            //controllo che siano presenti tutti i valori portanti in tutte le altre combobox, se suddetti valori non sono gia' stati selezionati in altre combobox
            AggiungiElementoImportante(combo, ESTlabel);
            AggiungiElementoImportante(combo, NORDlabel);
            AggiungiElementoImportante(combo, Quota);
        }

        private void CampiNonSegnalati()
        {
            MessageBox.Show("Non sono stati selezionati i campi relativi alle coordinate EST, NORD e QUOTE.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            suddividi();
        }
        
        private void suddividi()
        {
            ConcurrentQueue<string> Result1 = new ConcurrentQueue<string>();
            
            int totElementi = ElliQuoteList.Count() / 4;
            enableLabels();
            long k = 0;
            

            if (ControllaCampiUtente() == true)
            {                
                Parallel.ForEach(ElliQuoteList, (item) =>
                {                    
                    Result1.Enqueue(ConcatenaCampi(item));
                    Invoke(new MethodInvoker(delegate
                    {
                        progressBar1.Increment(1);
                        progressBar1.Refresh();
                        k++;
                        label23.Text = k.ToString();
                        label23.Refresh();
                    }));
                });

                
                var csv = new List<string>();
                csv.AddRange(Result1);              

                string SavePath = SaveCSVFilePath();
                File.WriteAllText(SavePath, csv.ToString());
                MessageBox.Show("Procedura di associazione completata." + System.Environment.NewLine + "Elenco esportato come EST - NORD - Q_ELLI - Q_ORTO - DELTA_N", "Operazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void enableLabels()
        {
            label23.Visible = label24.Visible = label25.Visible = true;
            label23.Text = "0";
            label24.Text = "of";
            progressBar1.Maximum = ElliQuoteList.Count();
            label25.Text = ElliQuoteList.Count().ToString();
        }

        private Boolean ControllaCampiUtente()
        {
            //se non sono stati assegnati i campi, sono costretto ad interrompere la procedura sollevando un errore
            int sum = 0;

            bool IS_textbox2numeric = double.TryParse(textBox2.Text, out double n);
            if (IS_textbox2numeric == true)
            {
                SearchRange = Convert.ToDouble(textBox2.Text);
            }
            else
            {
                SearchRange = 0.012;
            }

            for (int X = 0; X < 10; X++)
            {
                if (ComboE[X].Enabled == true)
                {
                    if (ComboE[X].Text == ESTlabel)
                    {
                        sum = sum + 1;
                    }
                    else if (ComboE[X].Text == NORDlabel)
                    {
                        sum = sum + 1;
                    }
                    if (ComboE[X].Text == ELLIlabel)
                    {
                        sum = sum + 1;
                    }
                }
                if (ComboO[X].Enabled == true)
                {
                    if (ComboO[X].Text == ESTlabel)
                    {
                        sum = sum + 1;
                    }
                    else if (ComboO[X].Text == NORDlabel)
                    {
                        sum = sum + 1;
                    }
                    if (ComboO[X].Text == ORTOlabel)
                    {
                        sum = sum + 1;
                    }
                }
            }
            if (sum != 6)
            {
                CampiNonSegnalati();
                return false;
            }
            else
            {
                return true;
            }
        }

        private string ConcatenaCampi(string item)
        {
            //LO SCOPO DI QUESTA SUBROUTINE E' QUELLO DI TROVARE I CAMPI EST E NORD E FORNIRE UN OUTPUT CONCATENATO DEI DUE, MEMORIZZANDO LA QUOTA
            string Est = "";
            string Nord = "";
            //long NumeroRiga = 1;

            //string SavePath;
            string OutputLine = "";

            //separo la stringa utilizzando il delimitatore indicato
            string[] Columns = item.Split(ElliFile.FileDelimiter);
            double n;
            //verifico che l'attributo sia di tipo numerico, altrimenti passo alla linea successiva
            bool IS_Estnumerica = double.TryParse(Columns[ElliFile.CampoEst], out n);
            bool IS_Nordnumerica = double.TryParse(Columns[ElliFile.CampoNord], out n);

            //ASSOCIAZIONE DI QUOTA DEI DUE ELENCHI
            if (IS_Estnumerica == true && IS_Nordnumerica == true)
            {
                Est = string.Format("{0:#0.000}", Convert.ToDouble(Columns[ElliFile.CampoEst]));
                Nord = string.Format("{0:#0.000}", Convert.ToDouble(Columns[ElliFile.CampoNord]));
                ReturnObject returnObject = TrovaCorrispondente(Est, Nord);
                string itemToRemove = returnObject.ItemToRemove;
                bool elim = OrtoQuoteList.TryDequeue(out itemToRemove);
                Quota_2 = returnObject.Quota;

                string Quota_1 = string.Format("{0:#0.000}", Convert.ToDouble(Columns[ElliFile.CampoQuota]));

                if (double.TryParse(Quota_2, out n) == true)
                {
                    double DeltaN = Math.Abs(Convert.ToDouble(Quota_1) - Convert.ToDouble(Quota_2));
                    OutputLine = (Est + ";" + Nord + ";" + Quota_1 + ";" + Quota_2 + ";" + string.Format("{0:#0.000}", DeltaN));
                }
                else
                {
                    OutputLine = (Est + ";" + Nord + ";" + Quota_1 + ";" + Quota_2 + ";" + string.Format("{0:#0.000}", "Not Applicable"));
                }

            }

            return OutputLine;
            //MessageBox.Show("Procedura di associazione completata." + System.Environment.NewLine + "Elenco esportato come EST - NORD - Q_ELLI - Q_ORTO - DELTA_N", "Operazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public string SaveCSVFilePath()
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Salva";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
        retry:
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveFileDialog1.FileName) == false)
                {
                    var myfile = File.Create(saveFileDialog1.FileName);
                    myfile.Close();
                }
                try
                {
                    using (Stream stream = new FileStream(saveFileDialog1.FileName, FileMode.Open))
                    {
                        return saveFileDialog1.FileName;
                    }
                }
                catch
                {
                    MessageBox.Show("Il file e' correntemente in uso, si prega di chiuderlo.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    goto retry;
                }
            }
            else
            {
                goto retry;
            }
        }

        public ReturnObject TrovaCorrispondente(string Est, string Nord)
        {
            
            foreach (var line in OrtoQuoteList)
            {
                //separo la stringa utilizzando il delimitatore indicato
                string[] Columns = line.Split(OrtoFile.FileDelimiter);
                //verifico che l'attributo sia di tipo numerico, altrimenti passo alla linea successiva 
                bool IS_Estnumerica = double.TryParse(Columns[OrtoFile.CampoEst], out double n);
                bool IS_Nordnumerica = double.TryParse(Columns[OrtoFile.CampoNord], out n);
                if (IS_Estnumerica == true && IS_Nordnumerica == true)
                {
                    //Isolo la est e la nord di questo elenco di punti
                    double Est_secondo_elenco = Convert.ToDouble(Columns[OrtoFile.CampoEst]);
                    double Nord_secondo_elenco = Convert.ToDouble(Columns[OrtoFile.CampoNord]);

                    //Qua vado a calcolare la distanza tra il punto che ho ricevuto ed il punto attuale del secondo elenco
                    double Distance = Math.Sqrt(Math.Pow(Convert.ToDouble(Est) - Est_secondo_elenco, 2) + Math.Pow(Convert.ToDouble(Nord) - Nord_secondo_elenco, 2));

                    //se il valore e' inferiore al raggio di ricerca, considero il punto omologo
                    if (Distance < SearchRange)
                    {
                        //consegno, dalla funzione, il campo quota
                        return new ReturnObject(string.Format("{0:#0.000}", Convert.ToDouble(Columns[OrtoFile.CampoQuota])), line);
                    }
                }
            }
            return new ReturnObject(string.Empty, string.Empty);
        }

        #region Eventi CB
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 0);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 1);
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 2);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 3);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 4);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 5);
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 6);
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 7);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 8);
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(true, 9);
        }

        private void comboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 0);
        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 1);
        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 2);
        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 3);
        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 4);
        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 5);
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 6);
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 7);
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 8);
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBOXChangEvent(false, 9);
        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            //imposto la modalita' di selezione della textbox1-delimiter personalizzato
            textBox1.Select();
        }        
        #endregion


    }
}

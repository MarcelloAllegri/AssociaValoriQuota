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
using System.Windows.Threading;

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
        int k=0;
        delegate void StringArgReturningVoidDelegate();

        //cerco di mettere le combobox in due vettori
        public ComboBox[] ComboE = new ComboBox[3];
        public ComboBox[] ComboO = new ComboBox[3];

        string[] CoordinateE = { GENERIClabel, NORDlabel, ESTlabel, ELLIlabel };
        string[] CoordinateO = { GENERIClabel, NORDlabel, ESTlabel, ORTOlabel };

        char ELLI_delimiter, ORTO_delimiter;
        List<string> ElliQuoteFile;
        List<string> OrtoQuoteFile;


        double SearchRange;

        FileItemClass ElliFile = new FileItemClass(); //--> Quote Ellisodiche
        FileItemClass OrtoFile = new FileItemClass(); //--> Quote Ortometriche

        List<Campi> ListaQuoteEllisoidiche = new List<Campi>();
        List<Campi> ListaQuoteOrtometriche;
        ConcurrentBag<string> Result1 = new ConcurrentBag<string>();
        ConcurrentBag<string> NotApplicableList = new ConcurrentBag<string>();
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
           
            //per le quote ortometriche
            ComboO[0] = comboBox20;
            ComboO[1] = comboBox19;
            ComboO[2] = comboBox18;
            

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
            for (int i = 0; i < 3; i++)
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
                    ElliFile.FileDelimiter = ELLI_delimiter;
                    ElliFile.Path = openFileDialog1.FileName;
                    ComboBoxEnabler(ComboE, CoordinateE);
                }
                else
                {
                    OrtoFile.FileDelimiter = ORTO_delimiter;
                    OrtoFile.Path = openFileDialog1.FileName;
                    //ORTO_File_Path = openFileDialog1.FileName;
                    ComboBoxEnabler(ComboO, CoordinateO);
                }
                
            }
            
        }        

        private void ResetComboboxes(ComboBox[] combo)
        {
            for (int x = 0; x < 3; x++)
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
                ElliQuoteFile = new List<string>(File.ReadLines(ElliFile.Path));
                
                ResetAllRadioButtons();
                MessageBox.Show("File quote ellissoidiche importato!");
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Errore:\n Eccezione-> " + Ex.Message);
            }
        }

        private void SplitColumn(bool v,List<string> Quote)
        {
            if (v)
            {
                ListaQuoteEllisoidiche = new List<Campi>();
                foreach (var item in Quote)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] Columns = item.Split(ElliFile.FileDelimiter);
                        ListaQuoteEllisoidiche.Add(new Campi(
                            Convert.ToDouble(Columns[ElliFile.CampoEst]),
                            Convert.ToDouble(Columns[ElliFile.CampoNord]),
                            Convert.ToDouble(Columns[ElliFile.CampoQuota])));
                    }
                }
            }
            else
            {
                ListaQuoteOrtometriche = new List<Campi>();
                foreach (var item in Quote)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] Columns = item.Split(OrtoFile.FileDelimiter);
                        ListaQuoteOrtometriche.Add(new Campi(
                            Convert.ToDouble(Columns[OrtoFile.CampoEst]),
                            Convert.ToDouble(Columns[OrtoFile.CampoNord]),
                            Convert.ToDouble(Columns[OrtoFile.CampoQuota])));
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetComboboxes(ComboO);
            GetFileInfo("Importa il file di coordinate ortometriche", false);
            try
            {
                OrtoQuoteFile = new List<string>(File.ReadLines(OrtoFile.Path));
                
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
                for (int x = 0; x < 3; x++)
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
            for (int i = 0; i < 3; i++)
            {
                //interrompo la procedura di addizione, qualora il valore importante fosse stato trovato SELEZIONATO in una combobox
                if (combo[i].Text == ItemName) { return; }
            }
            //NON HO TROVATO UN VALORE IMPORTANTE SELEZIONATO IN ALCUNA COMBOBOX
            for (int x = 0; x < 3; x++)
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

                switch (combo[Index].Text)
                {
                    case "EST": ElliFile.CampoEst = Index; break;
                    case "NORD": ElliFile.CampoNord = Index; break;
                    case "Q_ELLIS": ElliFile.CampoQuota = Index; break;
                }
            }
            else
            {
                combo = ComboO;
                Quota = ORTOlabel;

                switch (combo[Index].Text)
                {
                    case "EST": OrtoFile.CampoEst = Index; break;
                    case "NORD": OrtoFile.CampoNord = Index; break;
                    case "Q_ORTO": OrtoFile.CampoQuota = Index; break;
                }
            }
            //controllo se è stato selezionato un valore importante
            if (combo[Index].Text != GENERIClabel && !string.IsNullOrEmpty(combo[Index].Text))
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
            Task[] splitTask = new Task[]
            {
                Task.Factory.StartNew(() => SplitColumn(true,ElliQuoteFile)),
                Task.Factory.StartNew(() => SplitColumn(false,OrtoQuoteFile))
            };

            Task.WaitAll(splitTask);

            suddividi();
        }

        private void suddividi()
        {
            enableLabels();

            Result1 = new ConcurrentBag<string>();
            NotApplicableList = new ConcurrentBag<string>();
            List<Task> TaskList = new List<Task>();
            if (ControllaCampiUtente() == true)
            {

                
                
                foreach (var item in ListaQuoteEllisoidiche)
                {
                    
                    TaskList.Add(Task.Factory.StartNew(() =>
                    {
                        ConcatenaCampi(item);

                    }).ContinueWith((parentTask) => { Invoke(new MethodInvoker(() => progressBar1.Increment(1))); }));

                    
                }

                Task.Factory.ContinueWhenAll(TaskList.ToArray(), completedTasks => {

                    Invoke(new MethodInvoker(() =>  SaveFiles()));

                    
                });

                
            }
        }       

        private void enableLabels()
        {
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = ListaQuoteEllisoidiche.Count();
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

            for (int X = 0; X < 3; X++)
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

        private void ConcatenaCampi(Campi item)
        {
            int firstResult = SearchCampo(item, 0);
            if (firstResult == -1)
            {
                int secondResult = SearchCampo(item, SearchRange);
                if (secondResult == -1) NotApplicableList.Add(item.getCoordinatesWithSeparator(',') + " Not Applicable,");
            }
        }

        public int SearchCampo(Campi item, double searchRange)
        {
            try
            {
                Campi campo = ListaQuoteOrtometriche.Find(x => item.CampoEst - x.CampoEst < searchRange && item.CampoNord - x.CampoNord == searchRange);
                if (campo != null)
                {
                    Result1.Add(item.getCoordinatesWithSeparator(',') + string.Format("{0:#0.000}", campo.CampoQuota) + "," + (Math.Abs(item.CampoQuota - campo.CampoQuota).ToString()));
                    ListaQuoteOrtometriche.Remove(campo);
                }
            }
            catch (Exception)
            {
                return -1;
            }

            return 0;
        }

        public void SaveFiles()
        {
            bool retry = true;
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Salva File Risultato";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
            while (retry)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(saveFileDialog1.FileName) == false)
                    {
                        //var Resultfile = File.Create(saveFileDialog1.FileName);
                        File.AppendAllLines(saveFileDialog1.FileName, Result1.ToList());
                        string SavePathNotApplicable = Path.GetDirectoryName(saveFileDialog1.FileName);
                        SavePathNotApplicable = SavePathNotApplicable + "\\" + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + "_NotApplicable.csv";
                        File.AppendAllLines(SavePathNotApplicable, NotApplicableList.ToList());
                        retry = false;
                        //Resultfile.Close();
                        MessageBox.Show("Procedura di associazione completata." + System.Environment.NewLine + "Elenco esportato come EST - NORD - Q_ELLI - Q_ORTO - DELTA_N", "Operazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Il file e' correntemente in uso, si prega di chiuderlo. (File risultato)", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        retry = true;
                    }                    
                }
                else
                {
                    retry = true;
                }                         
            }
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
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

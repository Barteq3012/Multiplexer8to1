using System.Security.Cryptography;

namespace Multiplexer8to1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //tutaj podpinamy logikê zaraz po starcie okna
            AddEvents();
        }

        private void AddEvents()
        {
            CheckBox[] inputs = {
                checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8,
                checkBox9, checkBox10, checkBox11
            };

            foreach (var checkBox in inputs)
            {
                checkBox.CheckedChanged += OnInputsChanged;
            }
        }

        //to jest funkcja, która spina frontend z backendem
        private void OnInputsChanged(object? sender, EventArgs e)
        {
            bool d0 = checkBox1.Checked;
            bool d1 = checkBox2.Checked;
            bool d2 = checkBox3.Checked;
            bool d3 = checkBox4.Checked;
            bool d4 = checkBox5.Checked;
            bool d5 = checkBox6.Checked;
            bool d6 = checkBox7.Checked;
            bool d7 = checkBox8.Checked;

            bool s0 = checkBox9.Checked;
            bool s1 = checkBox10.Checked;
            bool s2 = checkBox11.Checked;

            //wywo³anie logiki
            byte result = MuxLogic.SolveFromFrontend(
                d0, d1, d2, d3, d4, d5, d6, d7,
                s0, s1, s2
            );


            //update widoku
            bool isResultTrue = (result == 1);
            radioButton1.Checked = isResultTrue;

            //zmiana koloru dla wzmocnienia efektu wizualnego
            radioButton1.ForeColor = isResultTrue ? System.Drawing.Color.Green : System.Drawing.Color.Black;
            radioButton1.Font = isResultTrue ? new Font(radioButton1.Font, FontStyle.Bold) : new Font(radioButton1.Font, FontStyle.Regular);
        }

        private void RadioButton1_MouseDown(object sender, MouseEventArgs e)
        {
            ((RadioButton)sender).AutoCheck = false;
        }
    }
}

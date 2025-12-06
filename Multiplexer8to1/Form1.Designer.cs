using System.Drawing;
using System.Windows.Forms;

namespace Multiplexer8to1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private RadioButton radioButton1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private CheckBox checkBox7;
        private CheckBox checkBox8;
        private CheckBox checkBox9;
        private CheckBox checkBox10;
        private CheckBox checkBox11;
        private DataGridView dataGridViewTruth;
        private Panel panelMux;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // tworzenie kontrolek
            radioButton1 = new RadioButton();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox3 = new CheckBox();
            checkBox4 = new CheckBox();
            checkBox5 = new CheckBox();
            checkBox6 = new CheckBox();
            checkBox7 = new CheckBox();
            checkBox8 = new CheckBox();
            checkBox9 = new CheckBox();
            checkBox10 = new CheckBox();
            checkBox11 = new CheckBox();

            SuspendLayout();

            // ustawienia formularza
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 700);
            Text = "MUX 8:1 – symulator";

            // title
            Label labelTitle = new Label();
            labelTitle.Text = "Multiplekser 8 → 1 (MUX 8:1)";
            labelTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            labelTitle.AutoSize = true;
            labelTitle.Location = new Point(20, 15);
            Controls.Add(labelTitle);

            // data input group
            GroupBox groupBoxInputs = new GroupBox();
            groupBoxInputs.Text = "Wejścia danych (d0–d7)";
            groupBoxInputs.Location = new Point(20, 60);
            groupBoxInputs.Size = new Size(180, 360);
            Controls.Add(groupBoxInputs);

            CheckBox[] dataInputs = {
                checkBox1, checkBox2, checkBox3, checkBox4,
                checkBox5, checkBox6, checkBox7, checkBox8
            };

            string[] dataLabels = { "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7" };

            int y = 30;
            for (int i = 0; i < dataInputs.Length; i++)
            {
                var cb = dataInputs[i];
                cb.AutoSize = true;
                cb.Text = dataLabels[i];
                cb.Font = new Font("Segoe UI", 11);
                cb.Location = new Point(20, y);
                groupBoxInputs.Controls.Add(cb);
                y += 35;
            }

            // selector
            GroupBox groupBoxSelector = new GroupBox();
            groupBoxSelector.Text = "Selector (s0–s2)";
            groupBoxSelector.Location = new Point(300, 550);
            groupBoxSelector.Size = new Size(260, 90);
            Controls.Add(groupBoxSelector);

            checkBox9.AutoSize = true;
            checkBox10.AutoSize = true;
            checkBox11.AutoSize = true;

            checkBox9.Text = "s0";
            checkBox10.Text = "s1";
            checkBox11.Text = "s2";

            checkBox9.Font = checkBox10.Font = checkBox11.Font = new Font("Segoe UI", 12);

            checkBox9.Location = new Point(20, 30);   // s0
            checkBox10.Location = new Point(90, 30);  // s1
            checkBox11.Location = new Point(160, 30); // s2

            groupBoxSelector.Controls.Add(checkBox9);
            groupBoxSelector.Controls.Add(checkBox10);
            groupBoxSelector.Controls.Add(checkBox11);

            // output
            GroupBox groupBoxOutput = new GroupBox();
            groupBoxOutput.Text = "Wyjście (Y)";
            groupBoxOutput.Location = new Point(620, 120);
            groupBoxOutput.Size = new Size(180, 140);
            Controls.Add(groupBoxOutput);

            radioButton1.AutoSize = true;
            radioButton1.Text = "output";
            radioButton1.Font = new Font("Segoe UI", 14);
            radioButton1.Location = new Point(40, 50);
            radioButton1.TabStop = false;
            groupBoxOutput.Controls.Add(radioButton1);
            
            // tabela prawdy
            dataGridViewTruth = new DataGridView();
            dataGridViewTruth.Size = new Size(320, 300);
            dataGridViewTruth.Location = new Point(600, 300);
            dataGridViewTruth.ReadOnly = true;
            dataGridViewTruth.AllowUserToAddRows = false;
            dataGridViewTruth.AllowUserToDeleteRows = false;
            dataGridViewTruth.RowHeadersVisible = false;
            dataGridViewTruth.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTruth.MultiSelect = false;

            // kolumny: s2, s1, s0, aktywne wejście, wynik Y
            dataGridViewTruth.Columns.Add("s2", "s2");
            dataGridViewTruth.Columns.Add("s1", "s1");
            dataGridViewTruth.Columns.Add("s0", "s0");
            dataGridViewTruth.Columns.Add("input", "Wejście aktywne");
            dataGridViewTruth.Columns.Add("y", "Y");

            dataGridViewTruth.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridViewTruth.Columns["s2"].Width = 40;
            dataGridViewTruth.Columns["s1"].Width = 40;
            dataGridViewTruth.Columns["s0"].Width = 40;
            dataGridViewTruth.Columns["input"].Width = 110;
            dataGridViewTruth.Columns["y"].Width = 40;

            // wiersze 0 - 7 (000..111)
            for (int i = 0; i < 8; i++)
            {
                int s0v = (i & 1);
                int s1v = (i >> 1) & 1;
                int s2v = (i >> 2) & 1;
                string inputName = $"D{i}";

                dataGridViewTruth.Rows.Add(s2v, s1v, s0v, inputName, "-");
            }

            Controls.Add(dataGridViewTruth);


            // diagram panelu mux
            panelMux = new Panel();
            panelMux.Name = "panelMux";
            panelMux.Location = new Point(230, 70);
            panelMux.Size = new Size(350, 460);
            panelMux.BorderStyle = BorderStyle.FixedSingle;
            panelMux.BackColor = Color.White;

            // podpięcie eventu rysującego
            panelMux.Paint += PanelMux_Paint;

            Controls.Add(panelMux);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Multiplexer8to1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AddEvents();
        }

        private void AddEvents()
        {
            CheckBox[] inputs = {
                checkBox1, checkBox2, checkBox3, checkBox4,
                checkBox5, checkBox6, checkBox7, checkBox8,
                checkBox9, checkBox10, checkBox11
            };

            foreach (var checkBox in inputs)
            {
                checkBox.CheckedChanged += OnInputsChanged;
            }
        }

        // powiazanie frontend z backendem
        private void OnInputsChanged(object? sender, EventArgs e)
        {
            // pobranie danych wejœciowych D0 - D7
            bool d0 = checkBox1.Checked;
            bool d1 = checkBox2.Checked;
            bool d2 = checkBox3.Checked;
            bool d3 = checkBox4.Checked;
            bool d4 = checkBox5.Checked;
            bool d5 = checkBox6.Checked;
            bool d6 = checkBox7.Checked;
            bool d7 = checkBox8.Checked;

            // pobranie selektora S0..S2
            bool s0 = checkBox9.Checked;
            bool s1 = checkBox10.Checked;
            bool s2 = checkBox11.Checked;

            // wywo³anie logiki MUX
            byte result = MuxLogic.SolveFromFrontend(
                d0, d1, d2, d3, d4, d5, d6, d7,
                s0, s1, s2
            );

            // wynik koñcowy (0/1)
            bool isResultTrue = (result == 1);

            // wyœwietlenie na radiobuttonie
            radioButton1.Checked = isResultTrue;
            radioButton1.ForeColor = isResultTrue ? Color.Green : Color.Black;
            radioButton1.Font = isResultTrue ?
                new Font(radioButton1.Font, FontStyle.Bold) :
                new Font(radioButton1.Font, FontStyle.Regular);

            // aktualizacja mapy prawdy

            // jeœli mapa prawdy z jakiegoœ powodu nie istnieje – wyjœcie
            if (dataGridViewTruth == null)
                return;

            // wyliczenie liczby 0 - 7 na podstawie selektora
            int selectorValue = 0;
            if (s0) selectorValue |= 1;
            if (s1) selectorValue |= 2;
            if (s2) selectorValue |= 4;

            // wyczyœæ kolumny Y + podœwietlenia
            for (int i = 0; i < dataGridViewTruth.Rows.Count; i++)
            {
                var row = dataGridViewTruth.Rows[i];
                row.DefaultCellStyle.BackColor = Color.White;
                row.Cells[4].Value = "-";
            }

            // ustaw aktualny wiersz zgodnie z selektorem
            if (selectorValue >= 0 && selectorValue < dataGridViewTruth.Rows.Count)
            {
                var row = dataGridViewTruth.Rows[selectorValue];
                row.DefaultCellStyle.BackColor = Color.LightYellow;
                row.Cells[4].Value = isResultTrue ? "1" : "0";
            }
            panelMux.Invalidate();
        }

        private void PanelMux_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var panel = (Panel)sender!;
            int w = panel.ClientSize.Width;
            int h = panel.ClientSize.Height;

            int marginLeft = 40;
            int marginTop = 20;

            // wiêkszy i lepiej wycentrowany trapez
            Rectangle body = new Rectangle(marginLeft + 70, marginTop + 20, 120, 180);

            Point[] muxShape =
            {
                new Point(body.Left, body.Top),
                new Point(body.Right, body.Top + 25),
                new Point(body.Right, body.Bottom - 25),
                new Point(body.Left, body.Bottom)
            };

            using (var bodyBrush = new SolidBrush(Color.WhiteSmoke))
                g.FillPolygon(bodyBrush, muxShape);

            g.DrawPolygon(Pens.Black, muxShape);

            // napis "MUX 8:1"
            using var fontSmall = new Font("Segoe UI", 10, FontStyle.Bold);
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            g.DrawString("MUX 8:1", fontSmall, Brushes.Black,
                new RectangleF(body.Left, body.Top + 60, body.Width, 30), sfCenter);

            // pobranie danych wejœciowych
            bool[] ds =
            {
                checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked,
                checkBox5.Checked, checkBox6.Checked, checkBox7.Checked, checkBox8.Checked
            };

            bool[] ss =
            {
                checkBox9.Checked, checkBox10.Checked, checkBox11.Checked
            };

            int selectorValue = (ss[0] ? 1 : 0) | (ss[1] ? 2 : 0) | (ss[2] ? 4 : 0);

            byte result = MuxLogic.SolveFromFrontend(
                ds[0], ds[1], ds[2], ds[3], ds[4], ds[5], ds[6], ds[7],
                ss[0], ss[1], ss[2]
            );

            bool isResultTrue = (result == 1);

            // wejœcia d0..d7
            int inputsCount = 8;
            int spacing = (body.Height - 20) / (inputsCount - 1);

            using var fontInputs = new Font("Segoe UI", 9);

            for (int i = 0; i < inputsCount; i++)
            {
                int yLine = body.Top + 10 + spacing * i;

                Point p1 = new Point(marginLeft, yLine);
                Point p2 = new Point(body.Left, yLine);

                Color lineColor = (i == selectorValue) ?
                    (ds[i] ? Color.LimeGreen : Color.Red) : Color.Gray;

                float lineWidth = (i == selectorValue) ? 3f : 1.5f;

                using (var pen = new Pen(lineColor, lineWidth))
                    g.DrawLine(pen, p1, p2);

                // kó³ko
                Rectangle dotRect = new Rectangle(p1.X - 12, yLine - 6, 12, 12);
                using (var b = new SolidBrush(ds[i] ? Color.LimeGreen : Color.LightGray))
                    g.FillEllipse(b, dotRect);

                g.DrawEllipse(Pens.Black, dotRect);
                g.DrawString($"d{i}", fontInputs, Brushes.Black, dotRect.X - 25, yLine - 7);
            }

            // selektor s0..s2
            using var fontSel = new Font("Segoe UI", 9);

            for (int i = 0; i < 3; i++)
            {
                int x = body.Left + 25 + i * 30;
                int y1 = body.Bottom;
                int y2 = body.Bottom + 30;

                Color c = ss[i] ? Color.Blue : Color.Gray;
                float widthPen = ss[i] ? 2f : 1.5f;

                using (var pen = new Pen(c, widthPen))
                    g.DrawLine(pen, new Point(x, y1), new Point(x, y2));

                g.DrawString($"s{i}", fontSel, Brushes.Black, x - 7, y2 + 2);
            }

            // wyjœcie Y
            int yOut = body.Top + body.Height / 2;
            Point o1 = new Point(body.Right, yOut);
            Point o2 = new Point(w - 20, yOut);

            using (var pen = new Pen(isResultTrue ? Color.LimeGreen : Color.Black, 3))
                g.DrawLine(pen, o1, o2);

            g.DrawString("Y", fontSmall, Brushes.Black, o2.X - 15, yOut - 12);

            // opis dzia³ania
            using var fontDesc = new Font("Segoe UI", 9);
            g.DrawString($"Wejœcie aktywne: D{selectorValue}, Y = {(isResultTrue ? 1 : 0)}",
                fontDesc, Brushes.Black, new PointF(marginLeft, body.Bottom + 60));
        }

        // blokada klikania w radiobutton (wynik do odczytu)
        private void RadioButton1_MouseDown(object sender, MouseEventArgs e)
        {
            ((RadioButton)sender).AutoCheck = false;
        }
    }
}

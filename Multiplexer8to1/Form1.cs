using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Multiplexer8to1
{
    public partial class Form1 : Form
    {
        // Kolory aplikacji
        private readonly Color colBackground = Color.FromArgb(245, 247, 250);
        private readonly Color colAccent = Color.FromArgb(0, 122, 204);
        private readonly Color colActiveLine = Color.FromArgb(0, 200, 83);
        private readonly Color colInactiveLine = Color.FromArgb(189, 195, 199);
        private readonly Color colText = Color.FromArgb(50, 50, 50);
        private readonly Color colMuxFill = Color.White;

        // === LEGENDA (NIEZBEDNE DODATKI) ===
        private Panel legendPanel;

        public Form1()
        {
            InitializeComponent();

            this.Text = "MUX 8:1 – symulator";
            this.BackColor = colBackground;
            this.MinimumSize = new Size(1000, 700);
            this.DoubleBuffered = true;

            AddEvents();
            StyleDataGridView();
            AddExportButton();

            // legenda pod tabel¹
            AddLegend();

            if (radioButton1.Parent != null) radioButton1.Parent.Visible = false;

            this.Resize += (s, e) => RecalculateLayout();

            RecalculateLayout();
            OnInputsChanged(null, EventArgs.Empty);
        }

        // Skalowanie i uk³adanie elementów przy zmianie rozmiaru okna
        private void RecalculateLayout()
        {
            int margin = 20;
            int topPadding = 60;

            // Wejœcia danych (lewa strona)
            var grpIn = checkBox1.Parent;
            if (grpIn != null)
                grpIn.Location = new Point(margin, (this.ClientSize.Height - grpIn.Height) / 2);

            // Selektor (dó³)
            var grpSel = checkBox9.Parent;
            if (grpSel != null)
                grpSel.Location = new Point((this.ClientSize.Width - grpSel.Width) / 2, this.ClientSize.Height - grpSel.Height - margin);

            // Uk³ad prawej kolumny (tabela + legenda)
            if (dataGridViewTruth != null)
            {
                int dynamicTableWidth = Math.Max(350, (int)(this.ClientSize.Width * 0.30));
                dataGridViewTruth.Width = dynamicTableWidth;

                int rightX = this.ClientSize.Width - dynamicTableWidth - margin;
                dataGridViewTruth.Left = rightX;

                // ile miejsca realnie zostaje na dó³ (selektor + margines)
                int bottomReserved = 100;
                if (grpSel != null)
                    bottomReserved = (this.ClientSize.Height - grpSel.Top) + margin;

                int legendHeight = legendPanel != null ? legendPanel.Height : 0;
                int gapLegend = legendPanel != null ? 10 : 0;

                // sekcja pionowa dla prawego "bloku"
                int availableHeight = this.ClientSize.Height - topPadding - bottomReserved;
                availableHeight = Math.Max(200, availableHeight);

                // maksymalna wysokoœæ tabeli tak, by zmieœci³a siê legenda
                int maxTableHeight = availableHeight - legendHeight - gapLegend;
                int tableHeight = Math.Max(150, maxTableHeight);

                // wysokoœæ ca³ego bloku (tabela + legenda + odstêp)
                int blockHeight = tableHeight + (legendPanel != null ? (gapLegend + legendHeight) : 0);

                // startY tak, ¿eby blok by³ wycentrowany w dostêpnej przestrzeni
                int startY = topPadding + Math.Max(0, (availableHeight - blockHeight) / 2);

                dataGridViewTruth.Top = startY;
                dataGridViewTruth.Height = tableHeight;
            }

            // legenda pod tabel¹
            if (legendPanel != null && dataGridViewTruth != null)
            {
                legendPanel.Width = dataGridViewTruth.Width;
                legendPanel.Location = new Point(dataGridViewTruth.Left, dataGridViewTruth.Bottom + 10);
                legendPanel.BringToFront();
            }

            // Panel z wykresem (œrodek)
            if (panelMux != null && grpIn != null && grpSel != null && dataGridViewTruth != null)
            {
                int startX = grpIn.Right + margin;
                int endX = dataGridViewTruth.Left - margin;
                int availableHeight = grpSel.Top - topPadding - margin;

                panelMux.Location = new Point(startX, topPadding);
                panelMux.Size = new Size(Math.Max(100, endX - startX), Math.Max(100, availableHeight));
                panelMux.Invalidate();
            }
        }

        private void PanelMux_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var panel = (Panel)sender!;
            int w = panel.Width;
            int h = panel.Height;

            // Wymiary trapezu
            int muxW = Math.Min(180, (int)(w * 0.3));
            int muxH = (int)(h * 0.7);
            Rectangle body = new Rectangle((w - muxW) / 2, (h - muxH) / 2, muxW, muxH);

            Point[] trapezoid = {
                new Point(body.Left, body.Top),
                new Point(body.Right, body.Top + 40),
                new Point(body.Right, body.Bottom - 40),
                new Point(body.Left, body.Bottom)
            };

            // Cieñ pod spodem
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                g.TranslateTransform(5, 5);
                g.FillPolygon(shadowBrush, trapezoid);
                g.ResetTransform();
            }

            // Rysowanie obudowy
            using (var b = new SolidBrush(colMuxFill)) g.FillPolygon(b, trapezoid);
            using (var p = new Pen(colText, 2)) g.DrawPolygon(p, trapezoid);

            // Napis MUX w tle
            using (var f = new Font("Segoe UI", 20, FontStyle.Bold))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("MUX 8:1", f, new SolidBrush(Color.FromArgb(15, 0, 0, 0)), body, sf);
            }

            // Pobranie danych
            bool[] ds = { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked,
                          checkBox5.Checked, checkBox6.Checked, checkBox7.Checked, checkBox8.Checked };
            bool[] ss = { checkBox9.Checked, checkBox10.Checked, checkBox11.Checked };

            int selVal = (ss[0] ? 1 : 0) | (ss[1] ? 2 : 0) | (ss[2] ? 4 : 0);
            byte result = MuxLogic.SolveFromFrontend(ds[0], ds[1], ds[2], ds[3], ds[4], ds[5], ds[6], ds[7], ss[0], ss[1], ss[2]);
            bool isOutHigh = (result == 1);

            // Linie wejœciowe
            float spacing = (muxH - 40) / 7.0f;

            for (int i = 0; i < 8; i++)
            {
                float yTarget = body.Top + 20 + (i * spacing);
                PointF pStart = new PointF(0, yTarget);
                PointF pEnd = new PointF(body.Left, yTarget);

                bool isSelected = (i == selVal);
                bool isActive = ds[i];

                // Kolor linii: aktywne/wybrane/nieaktywne
                Pen pen;

                // Aktywne wejœcie = zielona linia
                if (isActive)
                {
                    pen = new Pen(colActiveLine, isSelected ? 3.0f : 2.2f);
                }
                // Wybrane, ale brak sygna³u = czarna przerywana linia
                else if (isSelected)
                {
                    pen = new Pen(Color.Black, 2.5f);
                    pen.DashStyle = DashStyle.Dash;
                    pen.DashCap = DashCap.Round;
                }
                // Nieaktywne i niewybrane = szara linia
                else
                {
                    pen = new Pen(colInactiveLine, 1.5f);
                }

                using (pen)
                {
                    g.DrawLine(pen, pStart, pEnd);

                    if (isSelected)
                    {
                        PointF pOut = new PointF(body.Right, h / 2);
                        g.DrawBezier(
                            pen,
                            pEnd,
                            new PointF(pEnd.X + 60, pEnd.Y),
                            new PointF(pOut.X - 60, pOut.Y),
                            pOut
                        );
                    }
                }

                using (var f = new Font("Segoe UI", 9))
                    g.DrawString($"d{i}", f, Brushes.Gray, 5, yTarget - 18);
            }

            // Selektory
            for (int i = 0; i < 3; i++)
            {
                float x = body.Left + 40 + (i * 35);
                using (var pen = new Pen(ss[i] ? colAccent : colInactiveLine, ss[i] ? 3 : 1.5f))
                    g.DrawLine(pen, x, body.Bottom, x, body.Bottom + 30);

                using (var f = new Font("Segoe UI", 9, FontStyle.Bold))
                    g.DrawString($"s{i}", f, Brushes.Black, x - 8, body.Bottom + 32);
            }

            // Wyjœcie Y
            PointF pOutStart = new PointF(body.Right, h / 2);
            PointF pOutEnd = new PointF(w - 50, h / 2);

            using (var pen = new Pen(isOutHigh ? colActiveLine : colText, 3))
                g.DrawLine(pen, pOutStart, pOutEnd);

            // Dioda LED
            float ledSize = 32;
            RectangleF ledRect = new RectangleF(pOutEnd.X - 10, pOutEnd.Y - ledSize / 2, ledSize, ledSize);

            if (isOutHigh)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(ledRect.X - 5, ledRect.Y - 5, ledSize + 10, ledSize + 10);
                    using (var brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = Color.FromArgb(150, colActiveLine);
                        brush.SurroundColors = new[] { Color.Transparent };
                        g.FillPath(brush, path);
                    }
                }
            }

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(ledRect);
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterPoint = new PointF(ledRect.X + ledSize / 3, ledRect.Y + ledSize / 3);
                    brush.CenterColor = isOutHigh ? Color.White : Color.FromArgb(200, 200, 200);
                    brush.SurroundColors = new[] { isOutHigh ? colActiveLine : Color.Gray };
                    g.FillPath(brush, path);
                }
            }

            g.DrawEllipse(new Pen(Color.DimGray, 1), ledRect);
            g.FillEllipse(Brushes.White, ledRect.X + 8, ledRect.Y + 8, 8, 8); // Odblask

            using (var f = new Font("Segoe UI", 14, FontStyle.Bold))
                g.DrawString("Y", f, Brushes.Black, pOutEnd.X + 25, pOutEnd.Y - 10);
        }

        private void StyleDataGridView()
        {
            if (dataGridViewTruth == null) return;

            dataGridViewTruth.BackgroundColor = Color.White;
            dataGridViewTruth.BorderStyle = BorderStyle.None;
            dataGridViewTruth.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewTruth.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewTruth.RowHeadersVisible = false;
            dataGridViewTruth.EnableHeadersVisualStyles = false;

            dataGridViewTruth.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridViewTruth.ColumnHeadersDefaultCellStyle.BackColor = colAccent;
            dataGridViewTruth.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewTruth.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridViewTruth.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewTruth.ColumnHeadersHeight = 45;

            dataGridViewTruth.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridViewTruth.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewTruth.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewTruth.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 235, 59);
            dataGridViewTruth.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridViewTruth.RowTemplate.Height = 30;

            // Ustawienie Fill, ale z wagami
            dataGridViewTruth.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Jeœli kolumny ju¿ istniej¹, ustawiamy ich wagi szerokoœci
            // 0=s2, 1=s1, 2=s0, 3=Wejœcie, 4=Y
            if (dataGridViewTruth.Columns.Count >= 5)
            {
                dataGridViewTruth.Columns[0].HeaderText = "s2";
                dataGridViewTruth.Columns[0].FillWeight = 15;

                dataGridViewTruth.Columns[1].HeaderText = "s1";
                dataGridViewTruth.Columns[1].FillWeight = 15;

                dataGridViewTruth.Columns[2].HeaderText = "s0";
                dataGridViewTruth.Columns[2].FillWeight = 15;

                dataGridViewTruth.Columns[3].HeaderText = "Wejœcie aktywne";
                dataGridViewTruth.Columns[3].FillWeight = 40; // Ta kolumna dostaje najwiêcej miejsca

                dataGridViewTruth.Columns[4].HeaderText = "Y";
                dataGridViewTruth.Columns[4].FillWeight = 15;
            }
        }

        private void AddEvents()
        {
            CheckBox[] inputs = { checkBox1, checkBox2, checkBox3, checkBox4,
                                  checkBox5, checkBox6, checkBox7, checkBox8,
                                  checkBox9, checkBox10, checkBox11 };
            foreach (var cb in inputs) if (cb != null) cb.CheckedChanged += OnInputsChanged;
        }

        private void OnInputsChanged(object? sender, EventArgs e)
        {
            bool[] d = { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked,
                         checkBox5.Checked, checkBox6.Checked, checkBox7.Checked, checkBox8.Checked };
            bool s0 = checkBox9.Checked; bool s1 = checkBox10.Checked; bool s2 = checkBox11.Checked;

            int selectorValue = (s0 ? 1 : 0) | (s1 ? 2 : 0) | (s2 ? 4 : 0);
            byte result = MuxLogic.SolveFromFrontend(d[0], d[1], d[2], d[3], d[4], d[5], d[6], d[7], s0, s1, s2);

            if (dataGridViewTruth != null)
            {
                dataGridViewTruth.ClearSelection();
                if (selectorValue >= 0 && selectorValue < dataGridViewTruth.Rows.Count)
                {
                    dataGridViewTruth.Rows[selectorValue].Selected = true;
                    dataGridViewTruth.Rows[selectorValue].Cells[4].Value = (result == 1) ? "1" : "0";
                }

                for (int i = 0; i < dataGridViewTruth.Rows.Count; i++)
                    if (i != selectorValue) dataGridViewTruth.Rows[i].Cells[4].Value = "-";
            }
            if (panelMux != null) panelMux.Invalidate();
        }

        private void AddExportButton()
        {
            Button btnSave = new Button();
            btnSave.Text = "Zapisz do pliku";
            btnSave.Size = new Size(160, 45);
            btnSave.Location = new Point(20, this.ClientSize.Height - 70);
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderColor = colAccent;
            btnSave.ForeColor = colAccent;
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            btnSave.Click += (s, e) => {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Plik tekstowy|*.txt", FileName = "Wyniki_MUX.txt" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(sfd.FileName))
                            {
                                sw.WriteLine("Symulator MUX 8:1 - Wyniki");
                                sw.WriteLine("Data: " + DateTime.Now);
                                sw.WriteLine("--------------------------------------------------");

                                foreach (DataGridViewRow row in dataGridViewTruth.Rows)
                                {
                                    if (row.IsNewRow) continue;
                                    string line = "";
                                    foreach (DataGridViewCell cell in row.Cells)
                                    {
                                        line += (cell.Value?.ToString() ?? "-") + "\t";
                                    }
                                    sw.WriteLine(line);
                                }
                            }
                            MessageBox.Show("Zapisano pomyœlnie.");
                        }
                        catch
                        {
                            MessageBox.Show("B³¹d zapisu pliku.");
                        }
                    }
                }
            };
            this.Controls.Add(btnSave);
        }

        // legenda
        private void AddLegend()
        {
            legendPanel = new Panel();
            legendPanel.Height = 150;
            legendPanel.BackColor = Color.White;
            legendPanel.BorderStyle = BorderStyle.FixedSingle;

            legendPanel.Paint += LegendPanel_Paint;

            legendPanel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            this.Controls.Add(legendPanel);
        }

        private void LegendPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int xLine1 = 12;
            int xLine2 = 40;
            int xText = 55;

            int y = 12;

            using (var titleFont = new Font("Segoe UI", 9, FontStyle.Bold))
                g.DrawString("Legenda:", titleFont, Brushes.Black, 12, y);

            y += 24;

            DrawLegendItem(g, xLine1, xLine2, xText, ref y, colActiveLine, DashStyle.Solid, "Aktywny sygna³ (1)");
            DrawLegendItem(g, xLine1, xLine2, xText, ref y, Color.Black, DashStyle.Dash, "Wybrane wejœcie bez sygna³u (0)");
            DrawLegendItem(g, xLine1, xLine2, xText, ref y, colInactiveLine, DashStyle.Solid, "Wejœcie nieaktywne");

            // LED Y
            int ledY = y + 2;
            using (var brush = new SolidBrush(colActiveLine))
                g.FillEllipse(brush, xLine1, ledY + 2, 12, 12);
            g.DrawEllipse(Pens.DimGray, xLine1, ledY + 2, 12, 12);

            using (var font = new Font("Segoe UI", 9))
                g.DrawString("Dioda Y pokazuje stan wyjœcia", font, Brushes.Black, xText, ledY);
        }

        private void DrawLegendItem(Graphics g, int x1, int x2, int xText, ref int y, Color color, DashStyle dash, string text)
        {
            using (var pen = new Pen(color, 3))
            {
                pen.DashStyle = dash;
                pen.DashCap = DashCap.Round;
                g.DrawLine(pen, x1, y + 6, x2, y + 6);
            }

            using (var font = new Font("Segoe UI", 9))
                g.DrawString(text, font, Brushes.Black, xText, y);

            y += 22;
        }
    }
}

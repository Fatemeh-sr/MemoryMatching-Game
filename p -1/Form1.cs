// فاطمه سراوانی - پروژه اول

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace p__1
{
    public partial class Form1 : Form
    {
        int x = 50, y = 100, pSize = 100;
        int row = 0, column = 0;
        PictureBox[,] puzz;
        PictureBox[] images = new PictureBox[57];
        Random rand = new Random();

        Timer previewTimer = new Timer();


        Label[,] labels;
        PictureBox pic1 = null;
        PictureBox pic2 = null;
        bool isProcessing = false;
        Timer revealTimer = new Timer();
        Timer hideTimer = new Timer();

        DateTime start;
        Timer gameTimer = new Timer();
        int pair = 0;

        public Form1()
        {
            InitializeComponent();
            Image_load(null, null);



            previewTimer.Interval = 2500;
            previewTimer.Tick += PreviewTimer_Tick;


            hideTimer.Interval = 1000;
            hideTimer.Tick += HideTimer_Tick;

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            row = 0;
            column = 0;
            pair = 0;
            x = 50;
            y = 100;
            pSize = 100;
            foreach (PictureBox pic in puzz)
            {
                this.Controls.Remove(pic);
            }

            foreach (Label lbl in labels)
            {
                this.Controls.Remove(lbl);
            }

            gameTimer.Stop();
            start = DateTime.Now;

            Tlable.Text = "Time: 00:00";

            button1.Enabled = true;
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
        }
        private void Image_load(object sender, EventArgs e)
        {
            int j = 1;
            for (int i = 0; i < 57; i++)
            {
                images[i] = new PictureBox();
                images[i].Image = (Image)Properties.Resources.ResourceManager.GetObject($"_{j}");
                j++;
            }
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            column = (int)numericUpDown2.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            row = (int)numericUpDown1.Value;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if ((row * column) % 2 == 0 && row != 0 && column != 0)
            {
                puzz = new PictureBox[row, column];


                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        puzz[i, j] = new PictureBox();
                        puzz[i, j].Size = new Size(pSize, pSize);
                        puzz[i, j].Location = new Point(x, y);
                        puzz[i, j].BorderStyle = BorderStyle.FixedSingle;
                        puzz[i, j].SizeMode = PictureBoxSizeMode.StretchImage;

                        this.Controls.Add(puzz[i, j]);
                        puzz[i, j].Click += new System.EventHandler(Puzz_Click);
                        x += pSize;

                    }
                    x = 50;
                    y += pSize;
                }

                pair = 0;


                int[] imageIn = new int[images.Length];
                for (int i = 0; i < images.Length; i++)
                    imageIn[i] = i;

                for (int i = 0; i < imageIn.Length; i++)
                {
                    int j = rand.Next(i, imageIn.Length);
                    int temp = imageIn[i];
                    imageIn[i] = imageIn[j];
                    imageIn[j] = temp;
                }

                int index = 0;
                for (int n = 0; n < puzz.Length / 2; n++)
                {
                    if (index >= imageIn.Length)
                        break;

                    for (int count = 0; count < 2; count++)
                    {
                        int r, c;
                        do
                        {
                            r = rand.Next(0, row);
                            c = rand.Next(0, column);
                        } while (puzz[r, c].Image != null);

                        puzz[r, c].Image = images[imageIn[index]].Image;
                    }
                    index++;
                }


                Labels();
                foreach (Label lbl in labels)
                {
                    lbl.Visible = false;
                }

                previewTimer.Start();
                gameTimer.Start();

                start = DateTime.Now;

                button1.Enabled = false;
            }
            else
            {
                MessageBox.Show("At least one of the row or column values must be even!");
            }

        }

        private void PreviewTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    labels[i, j].Visible = true;
                }
            }
            previewTimer.Stop();
        }

        private void Puzz_Click(object sender, EventArgs e)
        {

        }

        private void Labels()
        {
            labels = new Label[row, column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    labels[i, j] = new Label();

                    labels[i, j].Size = puzz[i, j].Size;
                    labels[i, j].Location = puzz[i, j].Location;
                    labels[i, j].BackColor = Color.LightGray;
                    labels[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    labels[i, j].BorderStyle = BorderStyle.FixedSingle;
                    labels[i, j].Tag = puzz[i, j];
                    labels[i, j].Visible = false;

                    labels[i, j].Click += Label_Click;
                    this.Controls.Add(labels[i, j]);
                    labels[i, j].BringToFront();
                }
            }
        }
        private void Label_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            Label lab = (Label)sender;
            PictureBox pb = (PictureBox)lab.Tag;

            if (pb.Tag != null && (bool)pb.Tag) return;

            lab.Visible = false;

            if (pic1 == null)
            {
                pic1 = pb;
            }
            else if (pic2 == null && pb != pic1)
            {
                pic2 = pb;
                isProcessing = true;

                if (pic1.Image == pic2.Image)
                {
                    pic1.Tag = true;
                    pic2.Tag = true;
                    pic1 = null;
                    pic2 = null;
                    isProcessing = false;

                    pair++;
                    EndoftheGame();
                }
                else
                {
                    hideTimer.Start();
                }
            }
        }

        private void RevealTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    labels[i, j].Visible = true;
                }
            }
            revealTimer.Stop();
        }

        private void HideTimer_Tick(object sender, EventArgs e)
        {
            foreach (Label lbl in labels)
            {
                if (lbl.Tag == pic1 || lbl.Tag == pic2)
                {
                    lbl.Visible = true;
                }
            }

            pic1 = null;
            pic2 = null;
            isProcessing = false;
            hideTimer.Stop();
        }

        private void EndoftheGame()
        {
            if (pair == puzz.Length / 2)
            {
                gameTimer.Stop();
                DateTime end = DateTime.Now;
                TimeSpan elapsed = end - start;
                string message = $"Excellent job!! you did it! \nYour time: {elapsed.Minutes:00}:{elapsed.Seconds:00}";
                MessageBox.Show(message, "End of the Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void GameTimer(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - start;
            Tlable.Text = $"Time: {elapsed.Minutes:00}:{elapsed.Seconds:00}";
        }

    }
}
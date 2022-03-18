using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _05_Paint
{
    public partial class Paint : Form
    {
        Graphics graph;
        Point pStart;
        Pen myPen;
        Image previewCopy;
        List<Image> history;
        int undoCount;

        public Paint()
        {
            InitializeComponent();
            myPen = new Pen(buttonPenColor.BackColor, (float)numericUpDownPenWidth.Value);
            nowyToolStripMenuItem_Click(null, null);
        }

        private void pictureBoxImage_MouseDown(object sender, MouseEventArgs e)
        {
            pStart = e.Location;
            previewCopy = new Bitmap(pictureBoxImage.Image);
        }

        private void pictureBoxImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (radioButtonCurve.Checked)
                {
                    graph.DrawLine(myPen, pStart, e.Location);
                }
                else if (radioButtonLine.Checked)
                {
                    graph.DrawLine(myPen, pStart, e.Location);
                }
                else if (radioButtonRectangle.Checked)
                {
                    graph.DrawRectangle(myPen,
                                      Math.Min(pStart.X, e.X),
                                      Math.Min(pStart.Y, e.Y),
                                      Math.Abs(e.X - pStart.X),
                                      Math.Abs(e.Y - pStart.Y));
                }
                else if (radioButtonEllipse.Checked)
                {
                    graph.DrawEllipse(myPen,
                                      pStart.X,
                                      pStart.Y,
                                      e.X - pStart.X,
                                      e.Y - pStart.Y);
                }

                //czyszczenie cofniętych
                history.RemoveRange(history.Count - undoCount, undoCount);
                undoCount = 0;

                history.Add(new Bitmap(pictureBoxImage.Image));
                pictureBoxImage.Refresh();
            }
        }

        private void nowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBoxImage.Image = new Bitmap(pictureBoxImage.Width, pictureBoxImage.Height);
            graph = Graphics.FromImage(pictureBoxImage.Image);
            graph.Clear(Color.White);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            history = new List<Image>();
            undoCount = 0;
            history.Add(new Bitmap(pictureBoxImage.Image));

            /*
            history.Clear();
            history.Insert(5, new Bitmap(2,2));
            history.First();
            history.Last();
            history.ElementAt(5);
            history.Remove(history[5]);
            history.RemoveAt(3);
            for(int i=0; i< history.Count; i++)
            foreach(Image i in history)
            */
        }

        private void pictureBoxImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (radioButtonCurve.Checked)
                {
                    graph.DrawLine(myPen, pStart, e.Location);
                    pStart = e.Location;
                }
                else
                {
                    pictureBoxImage.Image = new Bitmap(previewCopy);
                    //pictureBoxImage.Image = previewCopy.Clone() as Image;

                    graph = Graphics.FromImage(pictureBoxImage.Image);

                    if (radioButtonLine.Checked)
                    {
                        graph.DrawLine(myPen, pStart, e.Location);
                    }
                    else if (radioButtonRectangle.Checked)
                    {
                        graph.DrawRectangle(myPen,
                                          Math.Min(pStart.X, e.X),
                                          Math.Min(pStart.Y, e.Y),
                                          Math.Abs(e.X - pStart.X),
                                          Math.Abs(e.Y - pStart.Y));
                    }
                    else if (radioButtonEllipse.Checked)
                    {
                        graph.DrawEllipse(myPen,
                                          pStart.X,
                                          pStart.Y,
                                          e.X - pStart.X,
                                          e.Y - pStart.Y);
                    }
                }
                pictureBoxImage.Refresh();
            }
        }

        private void buttonPenColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                buttonPenColor.BackColor = cd.Color;
                myPen.Color = cd.Color;
            }
        }

        private void numericUpDownPenWidth_ValueChanged(object sender, EventArgs e)
        {
            myPen.Width = (float)numericUpDownPenWidth.Value;
        }

        private void cofnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoCount < history.Count - 1)
            {
                undoCount++;
                pictureBoxImage.Image = history[history.Count - 1 - undoCount];
            }
        }

        private void ponówToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoCount > 0)
            {
                undoCount--;
                pictureBoxImage.Image = history[history.Count - 1 - undoCount];
            }
        }

        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "BMP|*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBoxImage.Image = new Bitmap(ofd.FileName);
                graph = Graphics.FromImage(pictureBoxImage.Image);
                graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                history = new List<Image>();
                undoCount = 0;
                history.Add(new Bitmap(pictureBoxImage.Image));
            }
        }

        private void zapiszJakoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP|*.bmp|JPEG|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(sfd.FileName))
                {
                    case ".bmp":
                        pictureBoxImage.Image.Save(sfd.FileName, ImageFormat.Bmp);
                        break;
                    case ".jpg":
                        pictureBoxImage.Image.Save(sfd.FileName, ImageFormat.Jpeg);
                        break;
                }
            }
        }
    }
}

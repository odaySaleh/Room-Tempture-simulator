using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Room_Tempture_simulator
{

    public partial class color_key : UserControl
    {
        public color_key()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int w = 50;
            int h = 60;

            if (comboBox1.SelectedItem.ToString() == "Continuous")
            {
               
                Graphics graphic = panel1.CreateGraphics();
                 w = 50;
                 h = 60;
                Rectangle r = new Rectangle(0, 0, w, h);
                Rectangle r2 = new Rectangle(w, 0, w, h);
                Rectangle r3 = new Rectangle(2 * w, 0, w, h);

                Brush x = new LinearGradientBrush(r, Color.Red, Color.Yellow, LinearGradientMode.Horizontal);
                Brush y = new LinearGradientBrush(r2, Color.Yellow, Color.Green, LinearGradientMode.Horizontal);
                Brush z = new LinearGradientBrush(r3, Color.Green, Color.Blue, LinearGradientMode.Horizontal);

                graphic.FillRectangle(x, r);
                graphic.FillRectangle(y, r2);
                graphic.FillRectangle(z, r3);
            }
            else if (comboBox1.SelectedItem.ToString() == "Discrete")
            {
               
                w = 37;
                Graphics g = panel1.CreateGraphics();
                Rectangle r = new Rectangle(0, 0, w, h);
                Rectangle r2 = new Rectangle(w, 0, w, h);
                Rectangle r3 = new Rectangle(2 * w, 0, w, h);
                Rectangle r4 = new Rectangle(3 * w, 0, w, h);

                Brush d = new LinearGradientBrush(r, Color.Blue, Color.Blue, LinearGradientMode.Horizontal);
                Brush c = new LinearGradientBrush(r2, Color.Green, Color.Green, LinearGradientMode.Horizontal);
                Brush b = new LinearGradientBrush(r3, Color.Yellow, Color.Yellow, LinearGradientMode.Horizontal);
                Brush a = new LinearGradientBrush(r4, Color.Red, Color.Red, LinearGradientMode.Horizontal);

                g.FillRectangle(a, r);
                g.FillRectangle(b, r2);
                g.FillRectangle(c, r3);
                g.FillRectangle(d, r4);
            }
        }

        Color[] n_color = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue };
        Color color;


        //return color corspondeing to a certain tempreture and start and end in contionus linear system
        public Color continous(double start, double end, double tempreture)
        {
            if (comboBox1.SelectedItem.ToString() == "Continuous")
            {
                double Smax = end;
                double Smin = start;
                double S_bet = tempreture;
                if (S_bet >= Smax)
                    S_bet = Smax - 1;
                double deltaS = (Smax - Smin) / 4;
                double ds = (S_bet - Smin) / deltaS;
                int i = (int)Math.Floor(ds);
                double alpha = ds - i;
                double R = n_color[i].R + (alpha * (n_color[i + 1].R - n_color[i].R));
                double G = n_color[i].G + (alpha * (n_color[i + 1].G - n_color[i].G));
                double B = n_color[i].B + (alpha * (n_color[i + 1].B - n_color[i].B));
                color = Color.FromArgb((int)R, (int)G, (int)B);
                
            }

            //--------------------------------Discreate---------------------------------------
            else if (comboBox1.SelectedItem.ToString() == "Discrete")
            {
                float dis_result;
                if (tempreture < start)
                {
                    dis_result = 0;
                }

                else if (tempreture >= end)
                {
                    dis_result = n_color.Length - 1;
                }

                else
                {
                    dis_result = ((n_color.Length - 1) * (((float)tempreture - (float)start) / ((float)end - (float)start)));
                    dis_result = (float)Math.Ceiling((decimal)dis_result);

                }
                color = n_color[(int)dis_result];
                return color;

            }
             return color;
        }

        //return color corspondeing to a certain tempreture and start and end in discrete linear system
        private Color discrete(double start, double end, double tempreture)
        {
            float dis_result;
            if (tempreture < start)
            {
                dis_result = 0;
            }

            else if (tempreture >= end)
            {
                dis_result = n_color.Length - 1;
            }

            else
            {
                dis_result = ((n_color.Length - 1) * (((float)tempreture - (float)start) / ((float)end - (float)start)));
                dis_result = (float)Math.Ceiling((decimal)dis_result);

            }
            return n_color[(int)dis_result];
        }

    }
}

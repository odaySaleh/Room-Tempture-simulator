using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Room_Tempture_simulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public int milliseconds = 550;

        //intiate the grid with the intial data and draw it
        private void Form1_Load(object sender, EventArgs e)
        {
            //intiate_grid();
            //grid_draw(grid);
            trackBar1.Value = 1;
            checkBox1.Checked = false;
            button1.Hide();
            button2.Hide();
            threads_box.Hide();
            label6.Hide();
            redo.Hide();
            undo.Hide();
        }
        Color[] n_color = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue };
        // Color[] n_color = { Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Red };
        int mouseX, mouseY = 0; //mouse coordinations
        int square_size = 40;
        int square_index = 0;
        List<Square> grid = new List<Square>();
        int Xmax = 1000;
        int Ymax = 700;
        Color square_color = Color.Green;
        int square_type = 1;
        bool drag = false;
        int brush_size = 1;
        bool liniar_margen_feature = false;
        double start_tempreture = 0;
        double end_tempreture = 100;
        double square_temp;
        List<Square> heat_source = new List<Square>();
        List<Square>[] History_grid = new List<Square>[5];
        int history_step = 0;
        List<Square> cold_source = new List<Square>();
        List<Square> temp_grid = new List<Square>();
        List<Thread> Threads_lis = new List<Thread>();
        Thread heat;
        Thread cold;
        bool done = false;
        int simulation_key = 0;
        int phase_start;
        int phase_end;
        int threads_num = 1;

        //get the mouse coordination
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mouseX = e.X; mouseY = e.Y;
            //label1.Location = new Point(x-label1.Width,y);
            //label2.Location = new Point(x ,y-label2.Height);
            label1.Text = "x: " + mouseX.ToString(); label2.Text = "y: " + mouseY.ToString();
            if (mouse_in_range())
            {
                button1.Text = "in range";

                if (grid.Count() > 0)
                {
                    index_label.Show();
                    temperature_lable.Show();
                    square_index = get_index();
                    index_label.Text = "index: " + square_index;
                    temperature_lable.Text = "Tempreture: " + grid[square_index].tempture;
                    if (checkBox2.Checked == true)
                    {
                        toolTip1.Show("Tempreture: " + grid[square_index].tempture + " index " + square_index, this, mouseX, mouseY, 1);
                        //toolTip1.ToolTipTitle = "Tempreture: " + grid[square_index].tempture + " index " + square_index;
                    }
                    else
                    {
                        toolTip1.Hide(this);
                    }

                    //    index_label.Location = new Point(mouseX , mouseY);
                    //  temperature_lable.Location = new Point(mouseX, mouseY - temperature_lable.Height);
                }
                label5.Text = "Square index: " + square_index;
            }
            else
            {
                index_label.Hide();
                temperature_lable.Hide();
                button1.Text = "Not in range";
            }

            if (drag == true && mouse_in_range())
            {
                auto_size(square_index, brush_size);
                // square_coloring(grid, square_index, square_color);
            }
        }

        //chech whiether the mouse ccursoerin teh range of teh drawing area or not
        private bool mouse_in_range()
        {
            bool condition = false;
            if (liniar_margen_feature == false)
            {
                if ((0 + (brush_size / 2 * square_size)) < mouseX && mouseX < (Xmax - (brush_size / 2 * square_size)) && (0 + (brush_size / 2 * square_size)) < mouseY && mouseY < (Ymax - (brush_size / 2 * square_size)))
                    condition = true;
            }

            else
            {
                if (0 < mouseX && mouseX < Xmax && 0 < mouseY && mouseY < Ymax)
                    condition = true;
            }

            return condition;
        }

        //fill the grid with the initial data
        private void intiate_grid()
        {
            int index = 0;
            grid.Clear();
            temp_grid.Clear();
            Square temp_square = new Square();
            for (int y = 0; y < Ymax; y += square_size)
            {
                for (int x = 0; x < Xmax; x += square_size)
                {
                    //intial square data
                    temp_square.X = x;
                    temp_square.Y = y;
                    temp_square.width = temp_square.height = square_size;
                    temp_square.index = index;
                    temp_square.type = 1;
                    temp_square.tempture = (((float)end_tempreture - (float)start_tempreture) * 75f) / 100f;
                    temp_square.done = false;
                    temp_square.color =color_key1.continous(start_tempreture, end_tempreture, temp_square.tempture);
                    //add the element to the list
                    grid.Add(temp_square);
                    index++;
                }
            }
            temp_grid = list_copy(grid);
        }

        //draw any given squares list
        private void grid_draw(List<Square> grid)
        {
            Color square_type = Color.Green;
            System.Drawing.Graphics graphics = this.CreateGraphics();
            SolidBrush brush = new SolidBrush(square_type);
            graphics.Clear(Color.Beige);
            for (int index = 0; index < grid.Count(); index++)
            {
                brush = new SolidBrush(grid[index].color);
                if (grid[index].type == 5)
                {
                    SolidBrush brush2 = new SolidBrush(Color.Black);
                    System.Drawing.Rectangle rectangle2 = new System.Drawing.Rectangle(grid[index].X, grid[index].Y, grid[index].width, grid[index].height);
                    graphics.FillRectangle(brush2, rectangle2);
                    System.Drawing.Rectangle rectangle3 = new System.Drawing.Rectangle(grid[index].X + 3, grid[index].Y + 3, (int)(grid[index].width - 6), (int)(grid[index].height - 6));
                    brush2.Color = Color.Green;
                    graphics.FillRectangle(brush2, rectangle3);
                }
                else
                {
                    //make a rectangle object
                    System.Drawing.Rectangle rectangle1 = new System.Drawing.Rectangle(grid[index].X, grid[index].Y, grid[index].width, grid[index].height);
                    //fill that outline
                    graphics.FillRectangle(brush, rectangle1);
                }

                ////make a rectangle object
                //System.Drawing.Rectangle rectangle1 = new System.Drawing.Rectangle(grid[i].X, grid[i].Y, grid[i].width, grid[i].height);
                ////change the crush color
                //brush.Color = grid[i].color;
                ////draw the outline of the rectangle
                //graphics.DrawRectangle(System.Drawing.Pens.CornflowerBlue, rectangle1);
                ////fill that outline
                //graphics.FillRectangle(brush, rectangle1);
            }
        }

        //rtefactor the average value based on new start and end tempreture for a givin grid
        private void average_refactor(double start, double end, double tmp_start, double tmp_end, List<Square> grid)
        {
            for (int i = 0; i < grid.Count(); i++)
            {
                Square tmp = new Square();
                tmp = grid[i];
                tmp.tempture = tmp.tempture * 100f / (tmp_end - tmp_start);
                tmp.tempture = tmp.tempture * (end - start) / 100f;
                grid[i] = tmp;
            }

        }

        //find the index at which the given small square is lie on
        private int get_the_index(int s_xmax, int b_xmax, int index, int ratio)
        {
            float row;
            if ((index) % s_xmax == 0)
            {
                row = (float)Math.Floor(((decimal)index + 1) / (decimal)s_xmax);
            }
            else
            {
                row = (float)Math.Floor(((decimal)index) / (decimal)s_xmax);
            }
            int n = (int)Math.Floor((decimal)row / ratio);
            int new_index = n * b_xmax;
            index = index - ((int)row * s_xmax);
            int tmp = index;
            index = (int)Math.Ceiling((decimal)index / ratio);
            if (tmp != ratio)
                new_index = new_index - 1 + index;
            else
                new_index = new_index + index;
            return new_index;
        }

        //defines weither the sqaure in range of the given index or not
        private int square_in_range(List<Square> s_grid, List<Square> b_grid, int b_index, int s_index, int b_square_size, int s_square_size)
        {
            int range = -1;
            if (s_grid[s_index].X >= b_grid[b_index].X && s_grid[s_index].X + s_square_size <= b_grid[b_index].X + b_square_size)
                range = 1;
            else
                range = -1;

            if (s_grid[s_index].Y >= b_grid[b_index].Y && s_grid[s_index].Y + s_square_size <= b_grid[b_index].Y + b_square_size)
            {
                if (range == 1)
                    range = 1; //range=1: square in the x boundries and y boundries
                else
                    range = -2; //range=-2: square in the y boundries and y boundries but out of x boundries
            }
            else
            {
                if (range == 1)
                    range = 2; //range=2: square in the x boundries but out of the y boundries
                else
                    range = -1; //range=-1: square is out of the y boundries and y boundries and also out of x boundries
            }


            return range;
        }

        //refactor the new grid with new size
        private List<Square> grid_refactor(List<Square> s_grid, List<Square> b_grid, int old_size, int new_size)
        {
            int s_xmax = get_xmax(new_size);
            int b_xmax = get_xmax(old_size);
            if (new_size < old_size)
            {
                Square tmp_square = new Square();
                int ratio = (int)Math.Floor((decimal)old_size / (decimal)new_size);
                int b_index;
                for (int i = 0; i < s_grid.Count(); i++)
                {

                    b_index = get_the_index(s_xmax, b_xmax, i, ratio);
                    if (b_index != -1 && b_index < b_grid.Count())
                    {
                        int range = square_in_range(s_grid, b_grid, b_index, i, old_size, new_size);
                        if (range != 1)
                        {
                            if (range == -2)
                            {
                                //propte to Heat source
                                if (b_grid[b_index + 1].type == 3)
                                    b_index = b_index + 1;
                                //then cold
                                else if (b_grid[b_index + 1].type == 4 && b_grid[b_index].type != 3)
                                    b_index = b_index + 1;
                                //then windwo
                                else if (b_grid[b_index + 1].type == 5 && b_grid[b_index].type != 3 && b_grid[b_index].type != 4)
                                    b_index = b_index + 1;
                                //then wall
                                else if (b_grid[b_index + 1].type == 2 && b_grid[b_index].type != 3 && b_grid[b_index].type != 4 && b_grid[b_index].type != 5)
                                    b_index = b_index + 1;
                            }
                            else if (range == 2 && b_index + b_xmax <= b_grid.Count())
                            {
                                //propte to Heat source
                                if (b_grid[b_index + b_xmax].type == 3)
                                    b_index = b_index + b_xmax;
                                //then cold
                                else if (b_grid[b_index + b_xmax].type == 4 && b_grid[b_index].type != 3)
                                    b_index = b_index + b_xmax;
                                //then windwo
                                else if (b_grid[b_index + b_xmax].type == 5 && b_grid[b_index].type != 3 && b_grid[b_index].type != 4)
                                    b_index = b_index + b_xmax;
                                //then wall
                                else if (b_grid[b_index + b_xmax].type == 2 && b_grid[b_index].type != 3 && b_grid[b_index].type != 4 && b_grid[b_index].type != 5)
                                    b_index = b_index + b_xmax;
                            }
                        }
                        tmp_square = s_grid[i];
                        tmp_square.type = b_grid[b_index].type;
                        tmp_square.tempture = b_grid[b_index].tempture;
                        tmp_square.color = b_grid[b_index].color;
                        s_grid[i] = tmp_square;
                    }
                }
            }
            else
            {
                Square tmep_square = new Square();
                int ratio = (int)Math.Ceiling((decimal)new_size / (decimal)old_size);
                int counter = 0;
                int index = 0;
                for (int i = 0; i < s_grid.Count(); i++)
                {
                    if (counter >= b_xmax)
                    {
                        index = index + b_xmax;
                        counter = 0;
                    }

                    tmep_square = average_squares(index, ratio, b_xmax, b_grid, s_grid, i);
                    index += ratio;
                    s_grid[i] = tmep_square;
                    counter += ratio;
                }
            }

            return s_grid;
        }

        //merge the small sized cells into one big cell based on their average or the piorety stack
        private Square average_squares(int index, int size, int xmax, List<Square> b_grid, List<Square> s_grid, int s_index)
        {
            Square tmep_square = new Square();
            tmep_square = s_grid[s_index];
            tmep_square.type = 1;
            double average = 0;
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++, index++)
                {
                    if (index < b_grid.Count())
                    {
                        average = average + b_grid[index].tempture;
                        if (b_grid[index].type != 1)
                        {
                            if (b_grid[index].type == 3)
                            {
                                tmep_square.type = b_grid[index].type;
                                tmep_square.color = b_grid[index].color;
                                tmep_square.tempture = b_grid[index].tempture;
                            }
                            else if (b_grid[index].type == 4 && tmep_square.type != 3)
                            {
                                tmep_square.type = b_grid[index].type;
                                tmep_square.color = b_grid[index].color;
                                tmep_square.tempture = b_grid[index].tempture;
                            }
                            else if (b_grid[index].type == 5 && tmep_square.type != 3 && tmep_square.type != 4)
                            {
                                tmep_square.type = b_grid[index].type;
                                tmep_square.color = b_grid[index].color;
                                tmep_square.tempture = b_grid[index].tempture;
                            }
                            else if (b_grid[index].type == 2 && tmep_square.type != 3 && tmep_square.type != 4 && tmep_square.type != 5)
                            {
                                tmep_square.type = b_grid[index].type;
                                tmep_square.color = b_grid[index].color;
                                tmep_square.tempture = b_grid[index].tempture;
                            }
                            // break;
                        }
                        counter++;
                    }
                }
                index = index - size;
                index = index + xmax;
            }
            if (tmep_square.type == 1)
            {
                average = average / counter;
                tmep_square.tempture = average;
            }
            return tmep_square;
        }

        //clear the drawing area
        private void button1_Click(object sender, EventArgs e)
        {
            if (minbox.Text != "" && maxbox.Text != "")
            {
                if (start_tempreture != double.Parse(minbox.Text) || end_tempreture != double.Parse(maxbox.Text))
                {
                    double tmp_start = start_tempreture;
                    double tmp_end = end_tempreture;
                    start_tempreture = double.Parse(minbox.Text);
                    end_tempreture = double.Parse(maxbox.Text);
                    average_refactor(start_tempreture, end_tempreture, tmp_start, tmp_end, grid);
                }
            }
            if (square_sizebox.Text != "" && int.Parse(square_sizebox.Text) != square_size)
            {
                int old_size = square_size;
                square_size = int.Parse(square_sizebox.Text);
                List<Square> temp_grid = new List<Square>();
                temp_grid = list_copy(grid);
                intiate_grid();
                grid = grid_refactor(grid, temp_grid, old_size, square_size);
                grid_draw(grid);
            }

        }

        //return the max number of squares in x axies for certain square size
        private int get_xmax(int square_size)
        {
            float xmax = Xmax;
            xmax = xmax / square_size;
            float x = mouseX;
            x = x / square_size;
            xmax = (float)Math.Ceiling(xmax);
            return (int)xmax;
        }

        //return the max number of squares in y axies for certain square size
        private int get_ymax(int square_size)
        {
            float ymax = Ymax;
            ymax = ymax / square_size;
            float y = mouseY;
            y = y / square_size;
            ymax = (float)Math.Ceiling(ymax);
            return (int)ymax;
        }

        //get the imdex of teh squer the mouse courser stands on 
        private int get_index()
        {
            int index = 0;
            int ymax = get_ymax(square_size);
            int xmax = get_xmax(square_size);
            float x = mouseX;
            float y = mouseY;
            x = x / square_size;
            y = y / square_size;
            x = (float)Math.Floor(x);
            y = (float)Math.Ceiling(y);
            index = (int)((y - 1) * xmax + x);
            return index;
        }

        //color the given index square with the given color
        private void square_coloring(List<Square> grid, int index, Color color, float tempreture, int type)
        {
            Square temp = new Square();
            System.Drawing.Graphics graphics = this.CreateGraphics();
            SolidBrush brush = new SolidBrush(color);
            if (type == 5)
            {
                SolidBrush brush2 = new SolidBrush(Color.Black);
                System.Drawing.Rectangle rectangle2 = new System.Drawing.Rectangle(grid[index].X, grid[index].Y, grid[index].width, grid[index].height);
                graphics.FillRectangle(brush2, rectangle2);
                System.Drawing.Rectangle rectangle3 = new System.Drawing.Rectangle(grid[index].X + 3, grid[index].Y + 3, (int)(grid[index].width - 6), (int)(grid[index].height - 6));
                brush2.Color = color;
                graphics.FillRectangle(brush2, rectangle3);
            }
            else
            {
                //make a rectangle object
                System.Drawing.Rectangle rectangle1 = new System.Drawing.Rectangle(grid[index].X, grid[index].Y, grid[index].width, grid[index].height);
                //fill that outline
                graphics.FillRectangle(brush, rectangle1);
            }
            temp = grid[index];
            temp.color = color;
            temp.type = type;
            temp.tempture = tempreture;
            grid[index] = temp;
        }

        //adjusting the number of colored squares based on brush size
        private void auto_size(int index, int brush_size)
        {
            //gething the half number of steps required 
            float x = brush_size / 2;
            //the new adjusted index
            int new_index = index;
            int xmax = get_xmax(square_size);
            //round down the x to the neareast integer
            x = (float)Math.Floor(x);
            //adjust the new index in the y axies
            new_index = index - ((int)x * (int)xmax);
            //adjust the new index in the x axies
            new_index = new_index - (int)x;

            for (int i = 0; i < brush_size; i++)
            {
                for (int j = 0; j < brush_size; j++)
                {
                    if (new_index < grid.Count() && new_index >= 0)
                        square_coloring(grid, new_index, square_color, (float)square_temp, square_type);
                    new_index++;
                }
                new_index = new_index - (brush_size);
                new_index = new_index + (int)xmax;
            }
        }

        //color the index givern squera upon teh mopuse click
        private void Form1_Click(object sender, EventArgs e)
        {
            if (mouse_in_range())
            {
                int index = get_index();
                auto_size(index, brush_size);
                // square_coloring(grid, index, square_color);
            }
        }

        //redraw teh latest updated grid if teh form size was updated
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // grid_draw(grid);
        }

        //normal cell 1
        private void green_CheckedChanged(object sender, EventArgs e)
        {
            square_color = Color.Green;
            square_temp = (((float)end_tempreture - (float)start_tempreture) * 75f) / 100f;
            square_type = 1;
        }

        //wall cell 2
        private void black_CheckedChanged(object sender, EventArgs e)
        {
            square_color = Color.Black;
            square_temp = (((float)end_tempreture - (float)start_tempreture) * 75f) / 100f;
            square_type = 2;
        }

        //hot cell 3
        private void red_CheckedChanged(object sender, EventArgs e)
        {
            square_color = Color.Red;
            square_temp = start_tempreture;
            square_type = 3;
        }

        //cold cell 4
        private void blue_CheckedChanged(object sender, EventArgs e)
        {
            square_color = Color.Blue;
            square_temp = end_tempreture;
            square_type = 4;
        }

        //windwo cell 5
        private void widnow_CheckedChanged(object sender, EventArgs e)
        {
            square_color = Color.Green;
            square_temp = (((float)end_tempreture - (float)start_tempreture) * 75f) / 100f;
            square_type = 5;
        }

        //if mouse kept pressed down
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            label3.Text = "mouse pressed down";

        }

        //if mopuse was released
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
            label3.Text = "mouse released";
            if (done == false && mouse_in_range())
                history(grid);
            if (history_step > 0)
                undo.Show();
            if (history_step <= 0)
            {
                undo.Hide();
            }
            //if (history_step < 4)
            //    redo.Show();
        }

        //insert and handel the history process
        private void history(List<Square> grid)
        {
            if (history_step < 5 && history_step >= 0)
            {
                History_grid[history_step] = list_copy(grid);
                history_step++;
                //for (int i = history_step; i < 5; i++)
                //{
                //    try
                //    {
                //        History_grid[i].Clear();
                //    }
                //  catch
                //    {
                //        continue;
                //    }
                //}
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    History_grid[i] = list_copy(History_grid[i + 1]);
                }
                history_step = 4;
                History_grid[history_step] = list_copy(grid);
            }
        }

        //adjust ateh track bar value
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            brush_size = trackBar1.Value;
            label4.Text = trackBar1.Value.ToString();
        }

        //enable and disabl ethe liniar margen feature
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                liniar_margen_feature = true;
            else
                liniar_margen_feature = false;
        }

        //calulate the shortest number of squares between 2 indexes
        private int distance(int start, int end)
        {
            int xmax = get_xmax(square_size);
            //swap start and end
            if (start < end)
            {
                int tmp = start;
                start = end;
                end = tmp;
            }
            // X and Y represent which row each index lies whithin
            float X = (float)Math.Ceiling((start + 1) / (float)xmax);
            float Y = (float)Math.Ceiling((end + 1) / (float)xmax);

            if (X == Y)
                return (int)Math.Abs(start - end);
            else
            {
                start = start - ((int)xmax * ((int)X - (int)Y));
                return (int)Math.Abs(start - end) + ((int)X - (int)Y);
            }
        }

        //fill lists with cold and heat source 
        private void tempreture_sources(List<Square> grid)
        {
            int temp = 0;
            for (int i = 0; i < grid.Count(); i++)
            {
                temp = grid[i].type;
                if (temp == 3)
                    heat_source.Add(grid[i]);
                else if (temp == 4)
                    cold_source.Add(grid[i]);
            }
        }

        //copy list to another list
        private List<Square> list_copy(List<Square> grid)
        {
            List<Square> temp_grid = new List<Square>();
            for (int i = 0; i < grid.Count(); i++)
                temp_grid.Add(grid[i]);
            return temp_grid;
        }

        //color the around frame of a givn temperture source basd on level number
        private void heat_itteration_coloring(List<Square> grid, List<Square> temp_grid, int itteration_num, int index)
        {
            Square temp_square = new Square();
            int xmax = get_xmax(square_size);
            index = index - itteration_num * (int)xmax - itteration_num;
            bool triger = false;
            for (int i = 0; i < itteration_num * 2 + 1; i++)
            {
                for (int j = 0; j < itteration_num * 2 + 1 && index < grid.Count(); j++, index++)
                {
                    if (index >= 0)
                    {
                        if (temp_grid[index].type == 1 && temp_grid[index].done == false)
                        {
                            float average = neighbour_average(grid, index);
                            average = (float)Math.Floor(average);
                            if (average == 50)
                            {
                                triger = true;
                                average = 75f;
                            }
                            else if (average < start_tempreture)
                                average = (float)start_tempreture + 1;
                            //else
                            //    average = average * 0.25f;
                            Color color =  color_key1.continous(start_tempreture, end_tempreture, average);
                            // Color color = discrete(start_tempreture, end_tempreture, average);
                            if (triger == true)
                            {
                                average = 50f;
                                triger = false;
                            }
                            square_coloring(grid, index, color, average, grid[index].type);
                        }
                        temp_square = temp_grid[index];
                        temp_square.done = true;
                        temp_grid[index] = temp_square;
                    }
                }
                if (index >= grid.Count())
                    break;
                index = index - (itteration_num * 2 + 1);
                index = index + (int)xmax;
            }
        }

        //color the around frame of a givn temperture source basd on level number
        private void cold_itteration_coloring(List<Square> grid, List<Square> temp_grid, int itteration_num, int index)
        {
            Square temp_square = new Square();
            int xmax = get_xmax(square_size);
            index = index - itteration_num * (int)xmax - itteration_num;
            bool triger = false;
            for (int i = 0; i < itteration_num * 2 + 1; i++)
            {
                for (int j = 0; j < itteration_num * 2 + 1 && index < grid.Count(); j++, index++)
                {
                    if (index >= 0)
                    {
                        if (temp_grid[index].type == 1 && temp_grid[index].done == false)
                        {
                            float average = neighbour_average(grid, index);
                            average = (float)Math.Ceiling(average);
                            if (average == 50)
                            {
                                triger = true;
                                average = 75f;
                            }
                            else if (average > end_tempreture)
                                average = (float)end_tempreture - 1;
                            //else
                            //    average = average * 1.75f;

                            Color color = color_key1.continous(start_tempreture, end_tempreture, average);
                            // Color color = discrete(start_tempreture, end_tempreture, average);
                            if (triger == true)
                            {
                                average = 50f;
                                triger = false;
                            }
                            square_coloring(grid, index, color, average, grid[index].type);
                        }
                        temp_square = temp_grid[index];
                        temp_square.done = true;
                        temp_grid[index] = temp_square;
                    }
                }
                if (index > grid.Count())
                    break;
                index = index - (itteration_num * 2 + 1);
                index = index + (int)xmax;
            }
        }

        //simulate the interval of the grid of 
        private void simulate(int phase_start, int phase_end)
        {
            while (done == false)
            {
                temp_grid = list_copy(grid);
                bool triger = false;
                for (int i = phase_start; i < phase_end; i++)
                {
                    if (grid[i].type == 1)
                    {
                        float average = neighbour_average(temp_grid, i);
                        if (average == (end_tempreture - start_tempreture) / 2)
                        {
                            triger = true;
                            average = (((float)end_tempreture - (float)start_tempreture) * 75f) / 100f;
                        }
                        else if (average < start_tempreture)
                            average = (float)start_tempreture + 1;
                        //else
                        //    average = average * 0.25f;

                        Color color = color_key1.continous(start_tempreture, end_tempreture, average);
                        //  Color color = discrete(start_tempreture, end_tempreture, average);
                        if (triger == true)
                        {
                            average = 50f;
                            triger = false;
                        }
                        square_coloring(grid, i, color, average, grid[i].type);
                    }
                }
                temp_grid = list_copy(grid);
            }
        }

        //simulte button
        private void button2_Click(object sender, EventArgs e)
        {

            simulation_key++;

            if (simulation_key > 0)
            {
                simulation_key = -1;
                done = false;
                button2.Text = "Stop";
                threads_intiate();
            }
            else
            {
                done = true;
                button2.Text = "Simulate";
                threads_suspend();
            }


        }

        //suspend teh created threads 
        private void threads_suspend()
        {
            for (int i = 0; i < threads_num; i++)
            {
                try
                {
                    Threads_lis[i].Suspend();
                }
                catch
                {
                    break;
                }
            }
            //  Threads_lis.Clear();
        }

        //create threds uopon the number of threads
        private void threads_intiate()
        {
            Threads_lis.Clear();
            if (threads_box.Text != "")
            {
                threads_num = int.Parse(threads_box.Text);
            }
            int gap = grid.Count() / threads_num;
            for (int i = 0; i < threads_num; i++)
            {
                int temp_end = gap * i;
                int temp_start = temp_end;
                temp_end += gap + 1;
                if (temp_end > grid.Count())
                    temp_end = grid.Count();


                Thread tmp = new Thread(() =>
               {
                   simulate(temp_start, temp_end);
               });
                Threads_lis.Add(tmp);
                Threads_lis[i].Start();
            }

            for (int i = 0; i < threads_num; i++)
            {
                //   threads[i].Join();
            }
        }

        //simulate the heat sources only
        private void simulate_heat()
        {
            int max = Math.Max(heat_source.Count(), cold_source.Count());
            for (int j = 1; done == false; j++)
            {
                for (int i = 0; i < heat_source.Count(); i++)
                {
                    heat_itteration_coloring(grid, temp_grid, j, heat_source[i].index);
                }
                temp_grid = list_copy(grid);
                Thread.Sleep(milliseconds);
            }
        }

        //simulate the cold sources only
        private void simulate_cold()
        {
            int max = Math.Max(heat_source.Count(), cold_source.Count());
            for (int j = 1; done == false; j++)
            {
                for (int i = 0; i < cold_source.Count(); i++)
                {
                    cold_itteration_coloring(grid, temp_grid, j, cold_source[i].index);
                }
                temp_grid = list_copy(grid);
                Thread.Sleep(milliseconds);
            }
        }

        //return the average of the tempreture of given index based on its neighbour
        private float neighbour_average(List<Square> grid, int index)
        {
            float average = 0;
            int xmax = get_xmax(square_size);
            int ymax = get_ymax(square_size);
            int counter = 0;
            //array of neighbours indexes
            int[] neighbours_indexes = { index - (int)xmax - 1 , index - (int)xmax , index - (int)xmax + 1,
                                         index-1               , index             , index+1              ,
                                         index + (int)xmax - 1 , index + (int)xmax , index + (int)xmax + 1 };

            int condition = index_on_edge_column(index, grid, xmax, ymax);
            //if inedx on the right edge
            if (condition == 1)
            {
                neighbours_indexes[2] = neighbours_indexes[5] = neighbours_indexes[8] = -1;
            }
            //if inedx on the left edge
            else if (condition == -1)
            {
                neighbours_indexes[0] = neighbours_indexes[3] = neighbours_indexes[6] = -1;
            }

            for (int i = 0; i < 9; i++)
            {
                if (neighbours_indexes[i] >= 0 && neighbours_indexes[i] <= grid.Count() - 1 && grid[neighbours_indexes[i]].type != 2)
                {
                    average = average + (float)grid[neighbours_indexes[i]].tempture;
                    counter++;
                }
            }

            average = average / counter;
            return average;
        }

        //check if the given index within the given grid is one of it's corners
        private bool index_on_Grid_Corner(int index, List<Square> grid, int xmax)
        {
            bool condition = false;
            int[] corners = { 0, xmax - 1, grid.Count() - 1, grid.Count() - xmax };
            for (int i = 0; i < 4; i++)
            {
                if (index == corners[i])
                {
                    condition = true;
                    break;
                }

            }
            return condition;
        }

        // //check if the given index within the given grid is on it's edge row
        private bool index_on_edge_rows(int index, List<Square> grid, float xmax, float ymax)
        {
            bool condition = false;
            index = (int)Math.Ceiling((decimal)(index + 1) / (decimal)xmax);
            if (index == 1 || index == (int)ymax)
            {
                condition = true;
            }
            return condition;
        }

        //check if the given index within the given grid is on it's edge columns
        private int index_on_edge_column(int index, List<Square> grid, float xmax, float ymax)
        {
            int condition = 0;
            int row_index = (int)Math.Ceiling((decimal)(index + 1) / (decimal)xmax);
            float column = (float)(row_index - 1) * xmax;
            //the index is on the righ far edge
            if (index - column == (xmax - 1))
            {
                condition = 1;
            }
            //the index is on the left far edge
            else if (index - column == 0)
                condition = -1;
            return condition;
        }

        //check if all the grid was alterd and done
        private bool grid_done(List<Square> grid)
        {
            bool condition = true;
            for (int i = 0; i < grid.Count(); i++)
            {
                if (grid[i].done == false)
                {
                    condition = false;
                    break;
                }
            }
            return condition;
        }

        //redraw teh grid
        private void button3_Click(object sender, EventArgs e)
        {
            button1.Show();
            button2.Show();
            if (square_sizebox.Text != "")
            {
                square_size = int.Parse(square_sizebox.Text);
            }
            intiate_grid();
            grid_draw(grid);
        }

        //Multi threaded radio button
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            threads_box.Show();
            label6.Show();
        }

        //Sequintial button
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            threads_box.Hide();
            label6.Hide();
            threads_num = 1;
        }

        //undo button
        private void undo_Click(object sender, EventArgs e)
        {
            try
            {
                grid = list_copy(History_grid[history_step - 2]);
            }
            catch
            {
            }
            grid_draw(grid);
            history_step--;
            if (history_step <= 0)
            {
                undo.Hide();
            }
            redo.Show();
        }

        //redo button
        private void redo_Click(object sender, EventArgs e)
        {
            try
            {
                grid = list_copy(History_grid[history_step]);
            }
            catch
            {
            }
            grid_draw(grid);
            history_step++;
            if (history_step >= 4)
            {
                redo.Hide();
            }
        }

        //load 
        private void color_key1_Load(object sender, EventArgs e)
        {
        }
    }
}
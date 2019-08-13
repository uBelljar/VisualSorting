using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualSorting
{
    public partial class Form1 : Form
    {
        readonly int length = 256; //количество столбцов
        readonly int columnWidth = 2; //множитель ширины столбцов
        readonly int columnHeight = 1; //множитель высоты столбцов
        readonly int interval = 1; // интервал между столбцами
        readonly int speed_1 = 1; //замедление создания и перемешивания
        int speed_2 = 10; //замедление сортировки


        Rectangle[] rectangles;
        bool refresh; //отключает обновление pictureBox
        bool inProgress; //Запрещает запуск других операций во время работы одной


        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(length * (columnWidth + interval) + 50, length * columnHeight + 97); // установка размеров формы
            label1.Text = speed_2.ToString(); //начальное значение лейбла скорости
        }

        //==============================================================================

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            RenderArray(e);
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e) //Переключатель скорости сортировки
        {
            speed_2 = trackBar1.Value;
            label1.Text = Convert.ToString(21 - speed_2);
        } 

        async private void Button1_Click(object sender, EventArgs e) //New
        {
            if (inProgress == false)
            {
                inProgress = true;
                NewArrayAsync();
                for (; ; )
                {
                    pictureBox1.Refresh();
                    if (refresh)
                        break;
                    //Thread.Sleep(speed_1);
                    await Task.Delay(speed_1);
                }
                inProgress = false;
            }
        }

        async private void Button2_Click(object sender, EventArgs e) //Shuffle
        {
            if (inProgress == false)
            {
                inProgress = true;
                ShuffleArrayAsync();
                for (; ; )
                {
                    pictureBox1.Refresh();
                    if (refresh)
                        break;
                    //Thread.Sleep(speed_1);
                    await Task.Delay(speed_1);
                }
                inProgress = false;
            }
        }

        async private void Button3_Click(object sender, EventArgs e) //Пузырьковая сортировка
        {
            if (inProgress == false)
            {
                inProgress = true;
                SortBubleAsync();
                for (; ; )
                {
                    pictureBox1.Refresh();
                    if (refresh)
                        break;
                    //Thread.Sleep(speed_2);
                    await Task.Delay(speed_2);
                }
                inProgress = false;
            }
        }

        async private void Button4_Click(object sender, EventArgs e) //Merge сортировка
        {
            if (inProgress == false)
            {
                inProgress = true;
                SortMergeAsync();
                for (; ; )
                {
                    pictureBox1.Refresh();
                    if (refresh)
                        break;
                    //Thread.Sleep(speed_2);
                    await Task.Delay(speed_2);
                }
                inProgress = false;
            }
        }

        //==============================================================================
        void NewArray()
        {
            if (rectangles == null)
                rectangles = new Rectangle[length];

            for (int i = 0; i < length; i++)
            {
                rectangles[i].Y = 5; //отступ столбцов от верхней границы 
                //rectangles[i].X = ... //положение столбца на поле назначается при отрисовке
                rectangles[i].Width = columnWidth; //ввод ширины столбцов
                rectangles[i].Height = (1 + i) * columnHeight; //ввод высоты столбцов
                Thread.Sleep(speed_1); //Async
            }
            refresh = true;
        }
        void ShuffleArray()
        {
            if (rectangles != null)
            {
                Random rand = new Random();
                int rand_i;
                for (int i = 0; i < length; i++)
                {
                    rand_i = rand.Next(length - 1);
                    SwapRect(i, rand_i);
                    Thread.Sleep(speed_1); //Async
                }
            }
            refresh = true;
        }
        void RenderArray(PaintEventArgs e)
        {
            if (rectangles != null)
            {
                for (int i = 0; i < length; i++)
                {
                    rectangles[i].X = 10 + i * (columnWidth + interval); //положение столбца на поле
                    e.Graphics.FillRectangle(Brushes.LightBlue, rectangles[i]);
                }
            }
        }
        void SortBubble()
        {
            if (rectangles != null)
            {
                bool isSorted;
                int k = 0;
                do
                {
                    isSorted = true;
                    for (int i = 1; i < length - k; i++)
                    {
                        if (rectangles[i - 1].Height > rectangles[i].Height)
                        {
                            isSorted = false;
                            SwapRect(i - 1, i);
                            Thread.Sleep(speed_2);
                        }
                    }
                    k++;
                }
                while (isSorted == false);
            }
            Thread.Sleep(10); //Что-бы соритровка успела отрендериться
            refresh = true;
        }
        void SortMerge()
        {
            if(rectangles != null && rectangles.Length >= 2)
            {
                SortMergeDivide(rectangles, 0); 
            }
            Thread.Sleep(10);
            refresh = true;

            void SortMergeDivide(Rectangle[] undivided, int inindex)
            {
                Rectangle[] left = new Rectangle[undivided.Length / 2];
                setLeftRight(0, left.Length, undivided, left);
                if (left.Length > 1)
                    SortMergeDivide(left, inindex);

                Rectangle[] right = new Rectangle[undivided.Length - left.Length];
                setLeftRight(left.Length, undivided.Length, undivided, right);
                if (right.Length > 1)
                    SortMergeDivide(right,inindex + left.Length);

                SortMergeMerging(undivided, left, right, inindex);
            }

            void SortMergeMerging(Rectangle[] merging, Rectangle[] left, Rectangle[] right, int inIndex)
            {
                int l = 0;
                int r = 0;
                int m = 0;
                for(int i = 0; i < merging.Length; i++)
                {
                    if (l < left.Length && r < right.Length)
                    {
                        if (left[l].Height < right[r].Height)
                        {
                            merging[i] = left[l];
                            l++;
                        }
                        else
                        {
                            merging[i] = right[r];
                            r++;
                        }
                    }
                    else if (l < left.Length)
                    {
                        merging[i] = left[l];
                        l++;
                    }
                    else
                    {
                        merging[i] = right[r];
                        r++;
                    }
                    if (merging == rectangles) //Для отображения в реальном времени последнего прохода
                        Thread.Sleep(speed_2);
                }
                for(int i = inIndex; i < inIndex + left.Length + right.Length; i++) //Цикл для отображения сортировки в реальном времени
                {
                    rectangles[i] = merging[m];
                    m++;
                    Thread.Sleep(speed_2); //Async

                }

            }

            void setLeftRight (int inIndex, int outIndex, Rectangle[] undivided, Rectangle[] leftRight) 
            {
                int lR = 0;
                for(int i = inIndex; i < outIndex; i++)
                {
                    leftRight[lR] = undivided[i];
                    lR++;
                }
            }
        }
        void SwapRect(int i, int j) //меняет местами 2 элемента в массиве
        {
            Rectangle temp;
            temp = rectangles[i];
            rectangles[i] = rectangles[j];
            rectangles[j] = temp;
        }        

        async void NewArrayAsync()
        {
            refresh = false;
            await Task.Run(() => NewArray());
        }
        async void ShuffleArrayAsync()
        {
            refresh = false;
            await Task.Run(() => ShuffleArray());
        }
        async void SortBubleAsync()
        {
            refresh = false;
            await Task.Run(() => SortBubble());
        }
        async void SortMergeAsync()
        {
            refresh = false;
            await Task.Run(() => SortMerge());
        }
    }
}

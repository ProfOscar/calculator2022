using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator2022
{
    public partial class FormMain : Form
    {
        public struct BtnStruct
        {
            public char Content;
            public bool IsBold;
            public BtnStruct(char content, bool isBold)
            {
                this.Content = content;
                this.IsBold = isBold;
            }
        }

        private BtnStruct[,] buttons =
            {
                { new BtnStruct('%', false), new BtnStruct('Œ', false), new BtnStruct('C', false), new BtnStruct('<', false) },
                { new BtnStruct('*', false), new BtnStruct('*', false), new BtnStruct('*', false), new BtnStruct('/', false) },
                { new BtnStruct('7', false), new BtnStruct('8', false), new BtnStruct('9', false), new BtnStruct('x', false) },
                { new BtnStruct('4', false), new BtnStruct('5', false), new BtnStruct('6', false), new BtnStruct('-', false) },
                { new BtnStruct('1', false), new BtnStruct('2', false), new BtnStruct('3', false), new BtnStruct('+', false) },
                { new BtnStruct('*', false), new BtnStruct('0', false), new BtnStruct(',', false), new BtnStruct('=', false) }
            };

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MakeButtons(buttons.GetLength(0), buttons.GetLength(1));
        }

        private void MakeButtons(int rows, int cols)
        {
            int btnWidth = 80;
            int btnHeight = 60;
            int posX = 0;
            int posY = 110;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Button myButton = new Button();
                    myButton.Font = new Font("Segoe UI", 16);
                    myButton.Text = buttons[i, j];
                    myButton.Width = btnWidth;
                    myButton.Height = btnHeight;
                    myButton.Left = posX;
                    myButton.Top = posY;
                    this.Controls.Add(myButton);
                    posX += btnWidth;
                }
                posX = 0;
                posY += btnHeight;
            }
        }
    }
}

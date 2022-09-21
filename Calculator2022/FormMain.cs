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
        private RichTextBox resultBox;
        private int resultBoxTextSize = 24;

        public struct BtnStruct
        {
            public char Content;
            public bool IsBold;
            public bool IsNumber;
            public bool IsDecimalSeparator;
            public bool IsPlusMinusSign;
            public BtnStruct(char content, bool isBold, 
                bool isNumber = false, bool isDecimalSeparator = false,
                bool isPlusMinusSign = false)
            {
                this.Content = content;
                this.IsBold = isBold;
                this.IsNumber = isNumber;
                this.IsDecimalSeparator = isDecimalSeparator;
                this.IsPlusMinusSign = isPlusMinusSign;
            }
            public override string ToString()
            {
                return Content.ToString();
            }
        }

        private BtnStruct[,] buttons =
            {
                { new BtnStruct('%', false), new BtnStruct('Œ', false), new BtnStruct('C', false), new BtnStruct('←', false) },
                { new BtnStruct('⅟', false), new BtnStruct('²', false), new BtnStruct('√', false), new BtnStruct('/', false) },
                { new BtnStruct('7', true, true), new BtnStruct('8', true, true), new BtnStruct('9', true, true), new BtnStruct('x', false) },
                { new BtnStruct('4', true, true), new BtnStruct('5', true, true), new BtnStruct('6', true, true), new BtnStruct('-', false) },
                { new BtnStruct('1', true, true), new BtnStruct('2', true, true), new BtnStruct('3', true, true), new BtnStruct('+', false) },
                { new BtnStruct('±', false, false, false, true), new BtnStruct('0', true, true), new BtnStruct(',', false, false, true), new BtnStruct('=', false) }
            };

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MakeResultBox();
            MakeButtons(buttons.GetLength(0), buttons.GetLength(1));
        }

        private void MakeResultBox()
        {
            resultBox = new RichTextBox();
            resultBox.ReadOnly = true;
            resultBox.SelectionAlignment = HorizontalAlignment.Right;
            resultBox.Font = new Font("Segoe UI", resultBoxTextSize, FontStyle.Bold);
            resultBox.Width = this.Width - 16;
            resultBox.Height = 120;
            resultBox.Text = "0";
            resultBox.TextChanged += ResultBox_TextChanged;
            this.Controls.Add(resultBox);
        }

        private void ResultBox_TextChanged(object sender, EventArgs e)
        {
            int newSize = resultBoxTextSize - resultBox.TextLength + 12;
            if (newSize > 8 && newSize <= resultBoxTextSize)
            {
                resultBox.Font = new Font("Segoe UI", newSize, FontStyle.Bold);
            }
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
                    FontStyle myStyle = buttons[i, j].IsBold ? FontStyle.Bold : FontStyle.Regular;
                    myButton.Font = new Font("Segoe UI", 16, myStyle);
                    myButton.Text = buttons[i, j].ToString();
                    myButton.Width = btnWidth;
                    myButton.Height = btnHeight;
                    myButton.Left = posX;
                    myButton.Top = posY;
                    myButton.Tag = buttons[i, j];
                    myButton.Click += Button_Click;
                    this.Controls.Add(myButton);
                    posX += btnWidth;
                }
                posX = 0;
                posY += btnHeight;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            BtnStruct btnStruct = (BtnStruct)clickedButton.Tag;
            if (btnStruct.IsNumber)
            {
                if (resultBox.Text == "0") resultBox.Text = "";
                resultBox.Text += clickedButton.Text;
            }
            else
            {
                if (btnStruct.IsDecimalSeparator && !resultBox.Text.Contains(btnStruct.Content)) resultBox.Text += clickedButton.Text;
                if (btnStruct.IsPlusMinusSign && resultBox.Text != "0")
                    resultBox.Text = resultBox.Text.Contains("-") ? resultBox.Text.Replace("-", "") : "-" + resultBox.Text;
                else
                    switch (btnStruct.Content)
                    {
                        case 'C':
                            resultBox.Text = "0";
                            break;
                        case '←':
                            resultBox.Text = resultBox.Text.Remove(resultBox.TextLength - 1);
                            if (resultBox.TextLength == 0 || resultBox.Text == "-")
                                resultBox.Text = "0";
                            break;
                    }
            }
        }
    }
}

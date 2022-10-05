using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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

        private decimal operand1, operand2, result;
        private char lastOperator = ' ';
        private BtnStruct lastButtonClicked;
        private bool isBackspaceEnabled = false;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint wMsg, UIntPtr wParam, IntPtr lParam);

        public struct BtnStruct
        {
            public char Content;
            public bool IsBold;
            public bool IsNumber;
            public bool IsDecimalSeparator;
            public bool IsPlusMinusSign;
            public bool IsOperator;
            public bool IsEqualSign;
            public BtnStruct(char content, bool isBold, 
                bool isNumber = false, bool isDecimalSeparator = false,
                bool isPlusMinusSign = false, bool isOperator = false, bool isEqualSign = false)
            {
                this.Content = content;
                this.IsBold = isBold;
                this.IsNumber = isNumber;
                this.IsDecimalSeparator = isDecimalSeparator;
                this.IsPlusMinusSign = isPlusMinusSign;
                this.IsOperator = isOperator;
                this.IsEqualSign = isEqualSign;
            }
            public override string ToString()
            {
                return Content.ToString();
            }
        }

        private BtnStruct[,] buttons =
            {
                { new BtnStruct('%', false), new BtnStruct('Œ', false), new BtnStruct('C', false), new BtnStruct('←', false) },
                { new BtnStruct('⅟', false), new BtnStruct('²', false), new BtnStruct('√', false), new BtnStruct('/', false, false, false, false, true) },
                { new BtnStruct('7', true, true), new BtnStruct('8', true, true), new BtnStruct('9', true, true), new BtnStruct('x', false, false, false, false, true) },
                { new BtnStruct('4', true, true), new BtnStruct('5', true, true), new BtnStruct('6', true, true), new BtnStruct('-', false, false, false, false, true) },
                { new BtnStruct('1', true, true), new BtnStruct('2', true, true), new BtnStruct('3', true, true), new BtnStruct('+', false, false, false, false, true) },
                { new BtnStruct('±', false, false, false, true), new BtnStruct('0', true, true), new BtnStruct(',', false, false, true), new BtnStruct('=', false, false, false, false, false, true) }
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
            //resultBox.SelectionIndent += 20;
            SendMessage(resultBox.Handle, 0xd3, (UIntPtr)0x3, (IntPtr)0x00080008);
            resultBox.SelectionAlignment = HorizontalAlignment.Right;
            resultBox.Font = new Font("Segoe UI", resultBoxTextSize, FontStyle.Bold);
            resultBox.Width = this.Width - 16;
            resultBox.Height = 110;
            resultBox.Text = "0";
            resultBox.TabStop = false;
            resultBox.TextChanged += ResultBox_TextChanged;
            resultBox.GotFocus += ResultBox_HideCaretHandler;
            resultBox.MouseDown += ResultBox_HideCaretHandler;
            resultBox.SelectionChanged += ResultBox_HideCaretHandler;
            this.Controls.Add(resultBox);
        }

        private void ResultBox_HideCaretHandler(object sender, EventArgs e)
        {
            HideCaret(resultBox.Handle);
        }

        private void ResultBox_TextChanged(object sender, EventArgs e)
        {
            if (resultBox.TextLength > 0 && resultBox.Text != "-")
            {
                decimal num = decimal.Parse(resultBox.Text); string stOut = "";
                NumberFormatInfo nfi = new CultureInfo("it-IT", false).NumberFormat;
                int commaPosition = resultBox.Text.IndexOf(",");
                nfi.NumberDecimalDigits = commaPosition == -1 ? 0 : resultBox.TextLength - commaPosition - 1;
                stOut = num.ToString("N", nfi);
                if (commaPosition == resultBox.TextLength - 1) stOut += ",";
                resultBox.Text = stOut;
            }
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
                if (resultBox.Text == "0" || lastButtonClicked.IsOperator) resultBox.Text = "";
                if (resultBox.TextLength < 32) resultBox.Text += clickedButton.Text;
                isBackspaceEnabled = true;
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
                            ClearAll();
                            isBackspaceEnabled = false;
                            break;
                        case '←':
                            if (isBackspaceEnabled)
                            {
                                resultBox.Text = resultBox.Text.Remove(resultBox.TextLength - 1);
                                if (resultBox.TextLength == 0 || resultBox.Text == "-")
                                    resultBox.Text = "0";
                            }
                            break;
                        default:
                            if (btnStruct.IsOperator || btnStruct.IsEqualSign)
                                ManageOperator(btnStruct);
                            isBackspaceEnabled = false;
                            break;
                    }
            }
            lastButtonClicked = btnStruct;
        }

        private void ClearAll()
        {
            operand1 = 0;
            operand2 = 0;
            result = 0;
            lastOperator = ' ';
            resultBox.Text = "0";
            resultBox.Font = new Font("Segoe UI", resultBoxTextSize, FontStyle.Bold);
        }

        private void ManageOperator(BtnStruct btnStruct)
        {
            if (lastOperator == ' ')
            {
                operand1 = decimal.Parse(resultBox.Text);
                if (!btnStruct.IsEqualSign) lastOperator = btnStruct.Content;
            }
            else
            {
                if (!lastButtonClicked.IsEqualSign)
                    operand2 = decimal.Parse(resultBox.Text);
                if (lastButtonClicked.IsEqualSign && btnStruct.IsOperator)
                    lastOperator = ' ';
                switch (lastOperator)
                {
                    case '+':
                        if (lastButtonClicked.Content != '+') result = operand1 + operand2;
                        break;
                    case '-':
                        if (lastButtonClicked.Content != '-') result = operand1 - operand2;
                        break;
                    case 'x':
                        if (lastButtonClicked.Content != 'x') result = operand1 * operand2;
                        break;
                    case '/':
                        if (lastButtonClicked.Content != '/') result = operand1 / operand2;
                        break;
                    default:
                        break;
                }
                operand1 = result;
                if (!btnStruct.IsEqualSign)
                { 
                    lastOperator = btnStruct.Content;
                    if (lastButtonClicked.IsEqualSign) operand2 = 0;
                }
                string stResult = result.ToString();
                if (stResult.Contains(","))
                {
                    while (stResult.Substring(stResult.Length - 1) == "0")
                        stResult = stResult.Substring(0, stResult.Length - 1);
                    if (stResult.Substring(stResult.Length - 1) == ",")
                        stResult = stResult.Substring(0, stResult.Length - 1);
                }
                resultBox.Text = stResult;
            }
        }
    }
}

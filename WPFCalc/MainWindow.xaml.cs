using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace WPFCalc
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Current textbox value (right operand)
        /// </summary>
        private double textboxValue = .0;
        /// <summary>
        /// If TRUE decimal point is entered and decimal part of the number is not yet entered (in the textBoxValue)
        /// </summary>
        private bool decimalEntered = false;
        /// <summary>
        /// Left operand value
        /// </summary>
        private double Operand = .0;
        /// <summary>
        /// Current operation
        /// </summary>
        private string operation = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the value textbox with the specified number
        /// </summary>
        /// <param name="number">Number to update to the value textbox</param>
        /// <param name="append">Append number if TRUE, set the new number otherwise</param>
        private void UpdateValue(double number, bool append = true)
        {
            string textValue = "";
            if (append)
                textValue = txt_Value.Text + number.ToString(CultureInfo.CurrentCulture);
            else
                textValue = number.ToString(CultureInfo.CurrentCulture);
            
            if (!double.TryParse(textValue, NumberStyles.Number, CultureInfo.CurrentCulture, out textboxValue))
            {
                txt_Value.Text = "0";
                textboxValue = .0;
                decimalEntered = false;
                return;
            }

            txt_Value.Text = textboxValue.ToString(CultureInfo.CurrentCulture);
            if (decimalEntered) txt_Value.Text += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        }

        /// <summary>
        /// Calculates the current operation
        /// </summary>
        private void Calculate()
        {
            switch (operation)
            {
                case "+":
                    textboxValue = Operand + textboxValue;
                    break;
                case "-":
                    textboxValue = Operand - textboxValue;
                    break;
                case "*":
                    textboxValue = Operand * textboxValue;
                    break;
                case "/":
                    textboxValue = Operand / textboxValue;
                    break;
                case "min":
                    textboxValue = Math.Min(Operand, textboxValue);
                    break;
                case "max":
                    textboxValue = Math.Max(Operand, textboxValue);
                    break;
                case "avg":
                    textboxValue = (textboxValue + Operand) / 2.0;
                    break;
                case "^":
                    textboxValue = Math.Pow(Operand, textboxValue);
                    break;

                default:
                    return;
            }
            Operand = .0;
            decimalEntered = false;
        }

        private void btn_number_Click(object sender, RoutedEventArgs e)
        {
            double number = .0;
            Window senderWindow = sender as Window;
            if (sender == null) return;     //sender is not a window
            if (senderWindow.Title != Title) return;    //sender window is not the calc window
            ExecutedRoutedEventArgs executedArgs = e as ExecutedRoutedEventArgs;
            RoutedUICommand executedCommand = executedArgs.Command as RoutedUICommand;
            string commandText = executedCommand.Text;
            if (!double.TryParse(commandText, out number)) return;
            decimalEntered = false;
            UpdateValue(number);
        }

        private void btn_operation_Click(object sender, RoutedEventArgs e)
        {
            if (operation != "") Calculate();

            Window senderWindow = sender as Window;
            if (sender == null) return; //sender is not a window
            if (senderWindow.Title != Title) return;   //sender window is not a calc window
            ExecutedRoutedEventArgs executedArgs = e as ExecutedRoutedEventArgs;
            RoutedUICommand executedCommand = executedArgs.Command as RoutedUICommand;
            operation = executedCommand.Text;

            Operand = textboxValue;
            textboxValue = .0;
            decimalEntered = false;
            UpdateValue(0, false);
        }

        private void btn_eq_Click(object sender, RoutedEventArgs e)
        {
            Calculate();
            UpdateValue(textboxValue, false);
            operation = "";
        }

        private void btn_C_Click(object sender, RoutedEventArgs e)
        {
            operation = "";
            textboxValue = .0;
            decimalEntered = false;
            Operand = .0;
            UpdateValue(.0, false);
        }

        private void btn_CE_Click(object sender, RoutedEventArgs e)
        {
            textboxValue = .0;
            decimalEntered = false;
            UpdateValue(.0, false);
        }

        private void btn_Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (decimalEntered)
            {
                decimalEntered = false;
            }
            else
            {
                string textBoxValueText = textboxValue.ToString(CultureInfo.CurrentCulture);
                if (textBoxValueText.Length <= 1 ||
                    textboxValue < 0 && textBoxValueText.Length <= 2)
                {
                    textboxValue = .0;
                }
                else
                {
                    textBoxValueText = textBoxValueText.Substring(0, textBoxValueText.Length - 1);
                    if (textBoxValueText[textBoxValueText.Length - 1].ToString() ==
                        CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    {
                        textBoxValueText = textBoxValueText.Substring(0, textBoxValueText.Length - 1);
                    }
                    if (textBoxValueText == "") textBoxValueText = "0";

                    if (!double.TryParse(textBoxValueText, NumberStyles.Number, CultureInfo.CurrentCulture, out textboxValue)) return;
                }
            }
            UpdateValue(textboxValue, false);
        }

        private void btn_PlusMinus_Click(object sender, RoutedEventArgs e)
        {
            textboxValue *= -1.0;
            UpdateValue(textboxValue, false);
        }

        private void btn_Comma_Click(object sender, RoutedEventArgs e)
        {
            double remainder = Math.IEEERemainder(textboxValue, 1.0);
            if (remainder != .0) return;    //decimal point is already entered
            decimalEntered = true;
            UpdateValue(textboxValue, false);
        }
    }
}

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCalc
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
   public partial class MainWindow : Window
    {
        /// <summary>
        /// Current expression string
        /// </summary>
        private string expression = "";
        /// <summary>
        /// Current textbox value (right operand)
        /// </summary>
        private double textboxValue = .0;
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
            if (append) textboxValue = textboxValue * 10.0 + number;
            else textboxValue = number;
            txt_Value.Text = textboxValue.ToString();
        }

        /// <summary>
        /// Updates the expression textbox with the specified string
        /// </summary>
        /// <param name="str">String to update to the expression textbox</param>
        /// <param name="append">Append str if TRUE, set the new str otherwise</param>
        private void UpdateExpression(string str, bool append = true)
        {
            if (append) expression += " " + str;
            else expression = str;
            txt_Expression.Text = expression;
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
            UpdateValue(number);
        }

        private void btn_operation_Click(object sender, RoutedEventArgs e)
        {
            UpdateExpression(textboxValue.ToString());
            if (operation != "") Calculate();

            Window senderWindow = sender as Window;
            if (sender == null) return; //sender is not a window
            if (senderWindow.Title != Title) ;   //sender window is not a calc window
            ExecutedRoutedEventArgs executedArgs = e as ExecutedRoutedEventArgs;
            RoutedUICommand executedCommand = executedArgs.Command as RoutedUICommand;
            operation = executedCommand.Text;

            Operand = textboxValue;
            textboxValue = .0;
            UpdateExpression(operation);
            UpdateValue(0);
        }

        private void btn_eq_Click(object sender, RoutedEventArgs e)
        {
            Calculate();
            UpdateValue(textboxValue, false);
            UpdateExpression("", false);
            operation = "";
        }

        private void btn_C_Click(object sender, RoutedEventArgs e)
        {
            expression = "";
            operation = "";
            textboxValue = .0;
            Operand = .0;
            UpdateValue(.0, false);
            UpdateExpression("", false);
        }

        private void btn_CE_Click(object sender, RoutedEventArgs e)
        {
            textboxValue = .0;
            UpdateValue(.0, false);
        }

        private void btn_Backspace_Click(object sender, RoutedEventArgs e)
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
            UpdateValue(textboxValue, false);
        }
    }
}

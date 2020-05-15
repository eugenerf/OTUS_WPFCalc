using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;

namespace WPFCalc
{
    /// <summary>
    /// Calculator command (number, operation, clear etc.)
    /// </summary>
    public class CalcCommand : RoutedUICommand
    {
        /// <summary>
        /// Operation
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// Operation priority
        /// </summary>
        public ushort Priority { get; set; }
    }

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
        private CalcCommand currentOperation = null;
        /// <summary>
        /// Already entered commands stack
        /// </summary>
        private Stack<CalcCommand> enteredCommands;

        public MainWindow()
        {
            InitializeComponent();
            enteredCommands = new Stack<CalcCommand>();
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
                textValue = txt_Result.Text + number.ToString(CultureInfo.CurrentCulture);
            else
                textValue = number.ToString(CultureInfo.CurrentCulture);

            if (!double.TryParse(textValue, NumberStyles.Number, CultureInfo.CurrentCulture, out textboxValue))
            {
                txt_Result.Text = "0";
                textboxValue = .0;
                decimalEntered = false;
                return;
            }

            txt_Result.Text = textboxValue.ToString(CultureInfo.CurrentCulture);
            if (decimalEntered) txt_Result.Text += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
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
            txt_Expression.ScrollToEnd();
        }

        /// <summary>
        /// Updates the expression textbox with the specified operation
        /// </summary>
        /// <param name="operation">Operation to update to the expression textbox</param>
        private void UpdateExpression(CalcCommand operation)
        {
            expression += " " + operation.Operation;
            txt_Expression.Text = expression;
        }

        /// <summary>
        /// Calculates the current operation
        /// </summary>
        private void Calculate()
        {
            if (currentOperation == null) return;
            string operation = currentOperation.Operation;
            if (operation == (Resources["Addition"] as CalcCommand).Operation)
            {
                if (enteredCommands.Count > 2)  //one operation is already entered
                {
                    if(!CheckPriority())
                    {
                        Operand = CalculateEntered();
                        textboxValue = Operand + textboxValue;
                    }
                }
                else                            //this operation is the first entered one
                {
                    textboxValue = Operand + textboxValue;
                }
            }
            else if (operation == (Resources["Subtraction"] as CalcCommand).Operation)
            {
                textboxValue = Operand - textboxValue;
            }
            else if (operation == (Resources["Multiplication"] as CalcCommand).Operation)
            {
                textboxValue = Operand * textboxValue;
            }
            else if (operation == (Resources["Division"] as CalcCommand).Operation)
            {
                textboxValue = Operand / textboxValue;
            }
            else if (operation == (Resources["Minimum"] as CalcCommand).Operation)
            {
                textboxValue = Math.Min(Operand, textboxValue);
            }
            else if (operation == (Resources["Maximum"] as CalcCommand).Operation)
            {
                textboxValue = Math.Max(Operand, textboxValue);
            }
            else if (operation == (Resources["Average"] as CalcCommand).Operation)
            {
                textboxValue = (textboxValue + Operand) / 2.0;
            }
            else if (operation == (Resources["Power"] as CalcCommand).Operation)
            {
                textboxValue = Math.Pow(Operand, textboxValue);
            }
            else
            {
                return;
            }
            Operand = .0;
            decimalEntered = false;
        }

        /// <summary>
        /// Checks priorities of current and previous operations.
        /// If current priority is bigger pushes commands to stack.
        /// Otherwise does nothing.
        /// </summary>
        /// <returns>TRUE if current priority is bigger</returns>
        private bool CheckPriority()
        {
            CalcCommand pushOperand = new CalcCommand();
            pushOperand.Operation = Operand.ToString(CultureInfo.CurrentCulture);
            pushOperand.Priority = 0;
            enteredCommands.Push(pushOperand);
            if (currentOperation.Priority > enteredCommands.Peek().Priority)    //current operation priority is bigger than the previous one
            {
                enteredCommands.Push(currentOperation);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates already entered commands (e.g. stack)
        /// </summary>
        /// <returns>Calculation result</returns>
        private double CalculateEntered()
        {
            double result = .0;
            if (enteredCommands.Count <= 0) return .0;
            if (enteredCommands.Count == 1)
            {
                double.TryParse(enteredCommands.Pop().Operation, NumberStyles.Number, CultureInfo.CurrentCulture, out result);
                return result;
            }
            double right = .0;
            double.TryParse(enteredCommands.Pop().Operation, NumberStyles.Number, CultureInfo.CurrentCulture, out right);
            string operation = enteredCommands.Pop().Operation;
            double left = .0;
            double.TryParse(enteredCommands.Pop().Operation, NumberStyles.Number, CultureInfo.CurrentCulture, out left);
            if (operation == (Resources["Addition"] as CalcCommand).Operation)
            {
                result = left + right;
            }
            else if (operation == (Resources["Subtraction"] as CalcCommand).Operation)
            {
                result = left - right;
            }
            else if (operation == (Resources["Multiplication"] as CalcCommand).Operation)
            {
                result = left * right;
            }
            else if (operation == (Resources["Division"] as CalcCommand).Operation)
            {
                result = left / right;
            }
            else if (operation == (Resources["Minimum"] as CalcCommand).Operation)
            {
                result = Math.Min(left, right);
            }
            else if (operation == (Resources["Maximum"] as CalcCommand).Operation)
            {
                result = Math.Max(left, right);
            }
            else if (operation == (Resources["Average"] as CalcCommand).Operation)
            {
                result = (left + right) / 2.0;
            }
            else if (operation == (Resources["Power"] as CalcCommand).Operation)
            {
                result = Math.Pow(left, right);
            }
            else
            {
                return .0;
            }
            return result;
        }

        private void btn_number_Click(object sender, RoutedEventArgs e)
        {
            double number = .0;
            Window senderWindow = sender as Window;
            if (sender == null) return;     //sender is not a window
            if (senderWindow.Title != Title) return;    //sender window is not the calc window
            ExecutedRoutedEventArgs executedArgs = e as ExecutedRoutedEventArgs;
            CalcCommand executedCommand = executedArgs.Command as CalcCommand;
            string commandText = executedCommand.Operation;
            if (!double.TryParse(commandText, out number)) return;
            decimalEntered = false;
            UpdateValue(number);
        }

        private void btn_operation_Click(object sender, RoutedEventArgs e)
        {
            UpdateExpression(textboxValue.ToString());
            if (currentOperation != null) Calculate();

            Window senderWindow = sender as Window;
            if (sender == null) return; //sender is not a window
            if (senderWindow.Title != Title) return;   //sender window is not a calc window
            ExecutedRoutedEventArgs executedArgs = e as ExecutedRoutedEventArgs;
            currentOperation = executedArgs.Command as CalcCommand;

            Operand = textboxValue;
            textboxValue = .0;
            decimalEntered = false;
            UpdateExpression(currentOperation);
            UpdateValue(0, false);
        }

        private void btn_eq_Click(object sender, RoutedEventArgs e)
        {
            Calculate();
            UpdateValue(textboxValue, false);
            UpdateExpression("", false);
            currentOperation = null;
            enteredCommands.Clear();
        }

        private void btn_C_Click(object sender, RoutedEventArgs e)
        {
            expression = "";
            currentOperation = null;
            textboxValue = .0;
            decimalEntered = false;
            Operand = .0;
            UpdateValue(.0, false);
            UpdateExpression("", false);
            enteredCommands.Clear();
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

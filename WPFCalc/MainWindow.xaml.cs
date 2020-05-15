using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using WPFCalc.CalcEngine;

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
        /// Current expression string (without the number that is currently entered)
        /// </summary>
        private string expression = "";

        public MainWindow()
        {
            InitializeComponent();

            //adding the calculator operations to the expression reader
            AddOperation("Addition", () => { return new Addition(); });
            AddOperation("Subtraction", () => { return new Subtraction(); });
        }

        private void AddOperation(string name, Func<IExpression> operation)
        {
            CalcCommand command = Resources[name] as CalcCommand;
            ExpressionReader.AddOperation(command.Operation, operation, command.Priority);
        }

        /// <summary>
        /// Returns TRUE if decimal point is already entered to the specified number
        /// </summary>
        /// <returns>TRUE if decimal is already entered</returns>
        private bool DecimalEntered(string number)
        {
            return number.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

        /// <summary>
        /// Updates the expression textbox with the current expression
        /// </summary>
        private void UpdateExpression()
        {
            txt_Expression.Text = expression;
        }

        /// <summary>
        /// Gets the executed command text from the sender object
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Routed event arguments</param>
        /// <returns>Executed command text (or empty string if something...)</returns>
        private string GetCommandText(object sender, RoutedEventArgs e)
        {
            Window senderWindow = sender as Window;
            if (sender == null) return "";     //sender is not a window
            if (senderWindow.Title != Title) return "";    //sender window is not the calc window
            ExecutedRoutedEventArgs executedArgs = e as ExecutedRoutedEventArgs;
            if (e == null) return "";
            CalcCommand executedCommand = executedArgs.Command as CalcCommand;
            if (executedCommand == null) return "";
            if (executedCommand.Operation == null) return "";
            return executedCommand.Operation;
        }

        /// <summary>
        /// Gets the currently inputted number or operation from the expression
        /// </summary>
        /// <param name="strInput">Currently inputted number or operation</param>
        /// <returns>Position of the currently inputted smth in the expression string</returns>
        private int GetCurrentInput(out string strInput)
        {
            int posInExpression = expression.LastIndexOf(" ");
            if (posInExpression == -1)
            {
                strInput = expression;
                return 0;
            }
            strInput = expression.Substring(posInExpression + 1, expression.Length - posInExpression - 1);
            return posInExpression + 1;
        }

        private void btn_number_Click(object sender, RoutedEventArgs e)
        {
            string commandText = GetCommandText(sender, e);
            string currentInput = "";
            GetCurrentInput(out currentInput);
            double currentNumber = .0;

            if (!double.TryParse(currentInput, out currentNumber))        //if current input is not a number then it is an operation
            {
                //so we'll begin the new number input
                if (expression != "") expression += " ";
            }
            expression += commandText;

            UpdateResult();
            UpdateExpression();
        }

        private void btn_operation_Click(object sender, RoutedEventArgs e)
        {
            string commandText = GetCommandText(sender, e);
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            double currentNumber = .0;

            if (!double.TryParse(currentInput, out currentNumber))        //if current input is not a number then it is an operation
            {
                //so we'll replace the current operation with the entered one
                if (currentPosition == 0) expression = "0 ";
                else expression = expression.Substring(0, currentPosition);
            }
            else                                                        //if current input is a number
            {
                //we'll just add the new operation
                expression += " ";
            }
            expression += commandText;

            UpdateResult();
            UpdateExpression();
        }

        private void UpdateResult()
        {
            Context currentContext = null;
            IExpression currentExpression = null;
            if (ExpressionReader.ReadExpression(expression, ref currentExpression, ref currentContext))
            {
                txt_Result.Text = currentExpression.Interpret(currentContext).ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                txt_Result.Text = "Expression invalid!";
            }
        }

        private void btn_eq_Click(object sender, RoutedEventArgs e)
        {
            UpdateResult();
            double result = .0;
            if (!double.TryParse(txt_Result.Text, out result)) expression = "";
            else expression = txt_Result.Text;
            UpdateExpression();
            txt_Result.Text = "";
        }

        private void btn_C_Click(object sender, RoutedEventArgs e)
        {
            expression = "";
            UpdateExpression();
            txt_Result.Text = "";
        }

        private void btn_CE_Click(object sender, RoutedEventArgs e)
        {
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            if (currentPosition == 0) expression = "";
            else expression = expression.Substring(0, currentPosition - 1);
            UpdateExpression();
            UpdateResult();
        }

        private void btn_Backspace_Click(object sender, RoutedEventArgs e)
        {
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            if (currentPosition == 0) expression = "";
            else
            {
                double currentNumber = .0;
                if (!double.TryParse(currentInput, out currentNumber))
                    expression = expression.Substring(0, currentPosition - 1);
                else
                {
                    expression = expression.Substring(0, expression.Length - 1);
                    if(expression[expression.Length-1]==' ')
                        expression = expression.Substring(0, expression.Length - 1);
                }
            }
            UpdateExpression();
            UpdateResult();
            if (expression == "") txt_Result.Text = "";
        }

        private void btn_PlusMinus_Click(object sender, RoutedEventArgs e)
        {
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            double currentNumber = .0;

            if (!double.TryParse(currentInput, out currentNumber)) return;  //current input is not a number

            currentNumber *= -1.0;
            if (currentPosition == 0) expression = "";
            else expression = expression.Substring(0, currentPosition);
            expression += currentNumber.ToString(CultureInfo.CurrentCulture);
            UpdateExpression();
            UpdateResult();
        }

        private void btn_Comma_Click(object sender, RoutedEventArgs e)
        {
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            double currentNumber = .0;

            if (!double.TryParse(currentInput, out currentNumber)) return;  //current input is not a number
            if (currentInput.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) return;  //current input already contains a decimal point

            expression += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            UpdateExpression();
        }
    }
}

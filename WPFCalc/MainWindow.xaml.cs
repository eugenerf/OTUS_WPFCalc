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

        #region UI visual
        public MainWindow()
        {
            InitializeComponent();

            //adding the calculator operations to the expression reader
            AddOperation("Addition", () => { return new Addition(); });
            AddOperation("Subtraction", () => { return new Subtraction(); });
            AddOperation("Multiplication", () => { return new Multiplication(); });
            AddOperation("Division", () => { return new Division(); });
            AddOperation("Minimum", () => { return new Minimum(); });
            AddOperation("Maximum", () => { return new Maximum(); });
            AddOperation("Average", () => { return new Average(); });
            AddOperation("Power", () => { return new Power(); });
        }

        /// <summary>
        /// Updates the expression textbox with the current expression
        /// </summary>
        private void UpdateExpression()
        {
            txt_Expression.Text = expression;
        }

        /// <summary>
        /// Calculates the result and updates the result textbox
        /// </summary>
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
        #endregion

        #region UI controls
        /// <summary>
        /// Number button click handler
        /// </summary>
        private void btn_number_Click(object sender, RoutedEventArgs e)
        {
            //get the button command
            string commandText = GetCommandText(sender, e);
            //get what was the current input in the expression
            string currentInput = "";
            GetCurrentInput(out currentInput);
            double currentNumber = .0;
            if (!double.TryParse(currentInput, out currentNumber))        //if current input is not a number then it is an operation
            {
                //so we'll begin the new number input
                if (expression != "") expression += " ";
            }
            expression += commandText;
            //update UI
            UpdateResult();
            UpdateExpression();
        }

        /// <summary>
        /// Operation button click
        /// </summary>
        private void btn_operation_Click(object sender, RoutedEventArgs e)
        {
            //get the button command
            string commandText = GetCommandText(sender, e);
            //get what was the current input in the expression
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
            //update UI
            UpdateResult();
            UpdateExpression();
        }

        /// <summary>
        /// Result button click
        /// </summary>
        private void btn_eq_Click(object sender, RoutedEventArgs e)
        {
            UpdateResult();
            double result = .0;
            if (!double.TryParse(txt_Result.Text, out result)) expression = "";
            else expression = txt_Result.Text;
            UpdateExpression();
            txt_Result.Text = "";
        }

        /// <summary>
        /// Clear button click
        /// </summary>
        private void btn_C_Click(object sender, RoutedEventArgs e)
        {
            expression = "";
            UpdateExpression();
            txt_Result.Text = "";
        }

        /// <summary>
        /// Clear Entry (CE) button click
        /// </summary>
        private void btn_CE_Click(object sender, RoutedEventArgs e)
        {
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            if (currentPosition == 0) expression = "";
            else expression = expression.Substring(0, currentPosition - 1);
            UpdateExpression();
            UpdateResult();
        }

        /// <summary>
        /// Backspace button click
        /// </summary>
        private void btn_Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (expression.Length <= 1) expression = "";  //if expression has one or no symbols
            else
            {
                string currentInput = "";
                int currentPosition = GetCurrentInput(out currentInput);
                double currentNumber = .0;
                if (!double.TryParse(currentInput, out currentNumber))      //if current input is an operation
                    expression = expression.Substring(0, currentPosition - 1);  //clear this operation
                else                                                        //if current input is a number
                {
                    expression = expression.Substring(0, expression.Length - 1);    //delete the last digit
                    if (expression[expression.Length - 1] == ' ')                   //if now last simbol is a space
                        expression = expression.Substring(0, expression.Length - 1);    //delete it too
                }
            }
            //update UI
            UpdateExpression();
            UpdateResult();
            if (expression == "") txt_Result.Text = "";
        }

        /// <summary>
        /// Inversion (+/-) button click
        /// </summary>
        private void btn_PlusMinus_Click(object sender, RoutedEventArgs e)
        {
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            double currentNumber = .0;
            if (!double.TryParse(currentInput, out currentNumber)) return;  //current input is not a number - do nothing
            //current input is a number - invert it
            currentNumber *= -1.0;
            if (currentPosition == 0) expression = "";
            else expression = expression.Substring(0, currentPosition);
            expression += currentNumber.ToString(CultureInfo.CurrentCulture);
            //update UI
            UpdateExpression();
            UpdateResult();
        }

        /// <summary>
        /// Decimal point button click
        /// </summary>
        private void btn_Comma_Click(object sender, RoutedEventArgs e)
        {
            string currentInput = "";
            int currentPosition = GetCurrentInput(out currentInput);
            double currentNumber = .0;
            if (!double.TryParse(currentInput, out currentNumber)) return;  //current input is not a number - do nothing
            if (currentInput.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) return;  //current input already contains a decimal point - do nothing
            //add a decimal point to the end of the expression
            expression += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //update UI
            UpdateExpression();
        }
        #endregion

        #region Additional methods
        /// <summary>
        /// Adds the operation to the ExpressionReader
        /// </summary>
        /// <param name="name">Operation name (according to the commands in the Window resources</param>
        /// <param name="operation">Operation invokation expression</param>
        private void AddOperation(string name, Func<IExpression> operation)
        {
            CalcCommand command = Resources[name] as CalcCommand;
            ExpressionReader.AddOperation(command.Operation, operation, command.Priority);
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
            int posInExpression = expression.LastIndexOf(" ");  //look for the first space in the expression
            if (posInExpression == -1)  //if not found then the expression itself is the current input
            {
                strInput = expression;
                return 0;
            }
            //return the current input substring and its position in the expression
            strInput = expression.Substring(posInExpression + 1, expression.Length - posInExpression - 1);
            return posInExpression + 1;
        }
        #endregion
    }
}

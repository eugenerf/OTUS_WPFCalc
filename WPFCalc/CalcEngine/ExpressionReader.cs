using System;
using System.Collections.Generic;

namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Reads the expression from string to the expression tree
    /// </summary>
    static class ExpressionReader
    {
        /// <summary>
        /// Available operations dictionary
        /// </summary>
        static private Dictionary<string, Func<IExpression>> operations = null;
        /// <summary>
        /// Available operations sorted by their priorities
        /// </summary>
        static private string[][] byPriority = null;

        /// <summary>
        /// Adds operation to the dictionary
        /// </summary>
        /// <param name="symbol">Operation symbol</param>
        /// <param name="expression">Operation expression ctor</param>
        /// <param name="priority">Operation priority (0 - minimal)</param>
        static public void AddOperation(string symbol, Func<IExpression> expression, ushort priority)
        {
            if (operations == null) operations = new Dictionary<string, Func<IExpression>>();
            if (byPriority == null) byPriority = new string[priority + 1][];
            if (byPriority.Length <= priority) Array.Resize(ref byPriority, priority + 1);
            if (operations.ContainsKey(symbol))
            {
                operations[symbol] = expression;
                DeleteSymbolFromPriority(symbol);
            }
            else
            {
                operations.Add(symbol, expression);
            }
            AddSymbolByPriority(symbol, priority);
        }

        /// <summary>
        /// Deletes the specified symbol from the priority table
        /// </summary>
        /// <param name="symbol"></param>
        private static void DeleteSymbolFromPriority(string symbol)
        {
            if (byPriority == null) return;
            for (int i = 0; i < byPriority.Length; i++)
            {
                if (byPriority[i] == null) return;
                for (int j = 0; j < byPriority[i].Length; j++)
                {
                    if (byPriority[i][j] == symbol)
                    {
                        byPriority[i][j] = "\0";
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the new symbol to the priority table according to its priority
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="priority">Priority</param>
        static private void AddSymbolByPriority(string symbol, uint priority)
        {
            if (byPriority == null) byPriority = new string[priority + 1][];
            int index = (byPriority[priority] == null) ? 0 : byPriority[priority].Length;
            Array.Resize(ref byPriority[priority], index + 1);
            byPriority[priority][index] = symbol;
        }

        /// <summary>
        /// Gets the operation expression from the dictionary by its symbol
        /// </summary>
        /// <param name="symbol">Operation symbol</param>
        /// <returns>Operation expression ctor</returns>
        static public Func<IExpression> GetOperation(string symbol)
        {
            return operations[symbol];
        }

        /// <summary>
        /// Reads the expression from the source. Result is stored to the expression and context
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="expression">Result expression tree</param>
        /// <param name="context">Result expression context</param>
        /// <returns>TRUE if read successfully</returns>
        static public bool ReadExpression(string source, ref IExpression expression, ref Context context)
        {
            context = null;
            expression = null;
            string[] splittedSource = source.Split(' ');

            //Now source is splitted in number of strings. Each string is a number or an operation.
            //And also all even items in splitted source (0,2,4th etc) are numbers, all odd items are operations.
            //We will skip the last operation if it has no number after it.
            //So the length of splittedSource must become odd (position of the last number is even, but we remember about the zero as a starting index)
            int splittedLenght = splittedSource.Length;
            if (splittedLenght % 2.0 == .0)  //length is even - need to skip the last item
                Array.Resize(ref splittedSource, splittedLenght - 1);

            //and now we finally can read the source to the expression tree
            context = new Context();
            return ReadRecursively(splittedSource, ref expression, ref context, 0, 0);
        }

        /// <summary>
        /// Recursively reads the splittedSource to the expression
        /// </summary>
        /// <param name="splittedSource">Expression as an array of strings</param>
        /// <param name="expression">Result expression tree</param>
        /// <param name="context">Result context for the expression (expression variables)</param>
        /// <param name="curPriority">Currently reading priority</param>
        /// <param name="curOperation">Number of the currently reading operation with the curPriority</param>
        /// <returns>TRUE if read successfully</returns>
        private static bool ReadRecursively(string[] splittedSource, ref IExpression expression, ref Context context, int curPriority, int curOperation)
        {
            if (byPriority == null) return false;
            if (curPriority >= byPriority.Length)   //reached the end of the byPriority table
            {
                double variableValue = .0;
                if (!double.TryParse(splittedSource[0], out variableValue)) return false;
                string variableName = context.AddVariable(variableValue);
                expression = new Number(variableName);
                return true;
            }

            if (byPriority[curPriority] == null ||                 //operations of current priority don't exist
                curOperation >= byPriority[curPriority].Length) //reached the end of the current priority list in the byPriority table
            {
                return ReadRecursively(splittedSource, ref expression, ref context, curPriority + 1, 0);
            }

            //looking for the current operation in the source
            int operationIndexInSource = Array.IndexOf(splittedSource, byPriority[curPriority][curOperation]);
            //if not found move to the next operation
            if (operationIndexInSource == -1)
                return ReadRecursively(splittedSource, ref expression, ref context, curPriority, curOperation + 1);
            //adding the current operation to the expression tree
            expression = operations[byPriority[curPriority][curOperation]].Invoke();

            //moving to the left of the source
            string[] leftSource = new string[operationIndexInSource];
            Array.Copy(splittedSource, leftSource, operationIndexInSource);
            IExpression leftExpression = null;
            bool leftResult = ReadRecursively(leftSource, ref leftExpression, ref context, curPriority, curOperation);
            ((Operands)expression).AddLeft(leftExpression);

            //moving to the right of the source
            string[] rightSource = new string[splittedSource.Length - operationIndexInSource - 1];
            Array.Copy(splittedSource, operationIndexInSource + 1, rightSource, 0, splittedSource.Length - operationIndexInSource - 1);
            IExpression rightExpression = null;
            bool rightResult = ReadRecursively(rightSource, ref rightExpression, ref context, curPriority, curOperation);
            ((Operands)expression).AddRight(rightExpression);

            //returning the successfullness ot the reading
            return leftResult && rightResult;
        }
    }
}

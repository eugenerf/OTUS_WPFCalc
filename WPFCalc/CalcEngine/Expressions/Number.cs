namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Number (terminal expression)
    /// </summary>
    class Number : IExpression
    {
        /// <summary>
        /// Variable name
        /// </summary>
        string name;

        public Number(string variableName)
        {
            name = variableName;
        }

        public double Interpret(Context context)
        {
            return context.GetVariable(name);
        }
    }
}

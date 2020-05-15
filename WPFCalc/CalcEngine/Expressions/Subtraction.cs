namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Subtraction operation
    /// </summary>
    class Subtraction : Operands, IExpression
    {
        public Subtraction() : base() { }

        public double Interpret(Context context)
        {
            return leftOperand.Interpret(context) - rightOperand.Interpret(context);
        }
    }
}

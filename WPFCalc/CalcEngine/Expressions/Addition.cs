namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Addition operation
    /// </summary>
    class Addition : Operands, IExpression
    {
        public Addition() : base() { }

        public double Interpret(Context context)
        {
            return leftOperand.Interpret(context) + rightOperand.Interpret(context);
        }
    }
}

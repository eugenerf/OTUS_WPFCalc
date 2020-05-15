using System;

namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Minimum operation
    /// </summary>
    class Minimum : Operands, IExpression
    {
        public Minimum() : base() { }

        public double Interpret(Context context)
        {
            return Math.Min(leftOperand.Interpret(context), rightOperand.Interpret(context));
        }
    }
}

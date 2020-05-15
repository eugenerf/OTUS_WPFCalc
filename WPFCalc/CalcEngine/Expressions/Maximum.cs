using System;

namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Maximum operation
    /// </summary>
    class Maximum : Operands, IExpression
    {
        public Maximum() : base() { }

        public double Interpret(Context context)
        {
            return Math.Max(leftOperand.Interpret(context), rightOperand.Interpret(context));
        }
    }
}

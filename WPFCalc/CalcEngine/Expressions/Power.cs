using System;

namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Power operation
    /// </summary>
    class Power : Operands, IExpression
    {
        public Power() : base() { }

        public double Interpret(Context context)
        {
            return Math.Pow(leftOperand.Interpret(context), rightOperand.Interpret(context));
        }
    }
}

using System;

namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Multiplication operation
    /// </summary>
    class Multiplication : Operands, IExpression
    {
        public Multiplication() : base() { }

        public double Interpret(Context context)
        {
            return leftOperand.Interpret(context) * rightOperand.Interpret(context);
        }
    }
}

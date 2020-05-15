using System;

namespace WPFCalc.CalcEngine
{
    class Multiplication : Operands, IExpression
    {
        public Multiplication() : base() { }

        public double Interpret(Context context)
        {
            return leftOperand.Interpret(context) * rightOperand.Interpret(context);
        }
    }
}

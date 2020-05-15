using System;

namespace WPFCalc.CalcEngine
{
    class Average : Operands, IExpression
    {
        public Average() : base() { }

        public double Interpret(Context context)
        {
            return (leftOperand.Interpret(context) + rightOperand.Interpret(context)) / 2.0;
        }
    }
}

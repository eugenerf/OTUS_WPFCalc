using System;

namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Division operation
    /// </summary>
    class Division : Operands, IExpression
    {
        public Division() : base() { }

        public double Interpret(Context context)
        {
            return leftOperand.Interpret(context) / rightOperand.Interpret(context);
        }
    }
}

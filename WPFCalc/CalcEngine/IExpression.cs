namespace WPFCalc.CalcEngine
{ 
    interface IExpression
    {
        double Interpret(Context context);
    }
}

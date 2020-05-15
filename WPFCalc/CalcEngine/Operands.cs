namespace WPFCalc.CalcEngine
{ 
    class Operands
    {
        /// <summary>
        /// Left operand
        /// </summary>
        internal protected IExpression leftOperand;
        /// <summary>
        /// Right operand
        /// </summary>
        internal protected IExpression rightOperand;

        public Operands()
        {
            leftOperand = null;
            rightOperand = null;
        }

        public Operands(IExpression left, IExpression right)
        {
            leftOperand = left;
            rightOperand = right;
        }

        /// <summary>
        /// Adds the left operand
        /// </summary>
        /// <param name="left">Left operand</param>
        public void AddLeft(IExpression left)
        {
            leftOperand = left;
        }

        /// <summary>
        /// Adds the right operand
        /// </summary>
        /// <param name="right">Right operand</param>
        public void AddRight(IExpression right)
        {
            rightOperand = right;
        }
    }
}

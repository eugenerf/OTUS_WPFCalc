using System.Collections.Generic;

namespace WPFCalc.CalcEngine
{
    /// <summary>
    /// Expression context (keeps the expression variables)
    /// </summary>
    class Context
    {
        /// <summary>
        /// Expression variables
        /// </summary>
        Dictionary<string, double> variables;

        public Context()
        {
            variables = new Dictionary<string, double>();
        }

        /// <summary>
        /// Gets variable value from the Context by its name
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <returns>Variable value</returns>
        public double GetVariable(string name)
        {
            return variables[name];
        }

        /// <summary>
        /// Adds the new variable to the Context
        /// </summary>
        /// <param name="value">Vari</param>
        /// <returns>Added variable name</returns>
        public string AddVariable(double value)
        {
            string name = "op" + variables.Count.ToString();
            if (variables.ContainsKey(name))
                variables[name] = value;
            else
                variables.Add(name, value);
            return name;
        }
    }
}

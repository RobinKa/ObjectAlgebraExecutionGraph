using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph
{
    public interface ICSharpTranslatableInputDataPin<TType> : IInputDataPin<TType>
    {
        /// <summary>
        /// Name of the pin's variable in code.
        /// </summary>
        string VariableName { get; }

        IEnumerable<string> TranslateCallPureFunction();
    }
}

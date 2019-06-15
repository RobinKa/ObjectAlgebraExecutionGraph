using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph
{
    public interface ICSharpTranslatableOutputDataPin : IOutputDataPin
    {
        /// <summary>
        /// Name for the variable of this pin in code.
        /// </summary>
        string VariableName { get; }

        /// <summary>
        /// Whether the node this pin is on is pure.
        /// </summary>
        bool IsPure { get; }

        /// <summary>
        /// Proxy for translating to code for calling this pin's node's call to its local function.
        /// </summary>
        /// <returns>Code for calling this pin's node's call to its local function</returns>
        IEnumerable<string> TranslateCallPureFunction();
    }
}

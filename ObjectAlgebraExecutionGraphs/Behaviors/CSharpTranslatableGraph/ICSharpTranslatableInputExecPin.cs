using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph
{
    public interface ICSharpTranslatableInputExecPin : IInputExecPin
    {
        /// <summary>
        /// Goto label of the input execution pin in code.
        /// </summary>
        string Label { get; }
    }
}

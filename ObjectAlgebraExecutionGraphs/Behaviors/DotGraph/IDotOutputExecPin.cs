using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Behaviors.DotGraph
{
    public interface IDotOutputExecPin : IOutputExecPin
    {
        /// <summary>
        /// Generates a DOT graph for this pin's node and its incoming nodes recursively.
        /// </summary>
        /// <returns>DOT graph for this pin's node and its incoming nodes.</returns>
        public string GenerateDotGraph(string toName);
    }
}

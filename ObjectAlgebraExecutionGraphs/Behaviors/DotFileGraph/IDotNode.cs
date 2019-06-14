using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Behaviors.DataGraph
{
    /// <summary>
    /// A graph node holding a collection of pins. Can create dot graphs for visualization.
    /// </summary>
    /// <typeparam name="TIXP">Input execution pin type</typeparam>
    /// <typeparam name="TOXP">Output execution pin type</typeparam>
    /// <typeparam name="TIDP">Input data pin type</typeparam>
    /// <typeparam name="TODP">Output data pin type</typeparam>
    public interface IDotNode<TIDP, TODP, TIXP, TOXP> : IExecutionNode<TIDP, TODP, TIXP, TOXP>
    {
        /// <summary>
        /// Generates a DOT graph for this node and its incoming nodes recursively.
        /// </summary>
        /// <returns>DOT graph for this node and its incoming nodes.</returns>
        public string GenerateDotGraph(string toName);

        /// <summary>
        /// Name of the node in the DOT graph.
        /// </summary>
        public string DotName { get; }
    }
}

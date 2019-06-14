using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph
{
    /// <summary>
    /// A graph node holding a collection of pins.
    /// </summary>
    /// <typeparam name="TIXP">Input execution pin type</typeparam>
    /// <typeparam name="TOXP">Output execution pin type</typeparam>
    /// <typeparam name="TIDP">Input data pin type</typeparam>
    /// <typeparam name="TODP">Output data pin type</typeparam>
    public interface IExecutionNode<TIXP, TOXP, TIDP, TODP> : IDataNode<TIDP, TODP>
    {
        /// <summary>
        /// Input execution pins of this node that can receive execution.
        /// </summary>
        public IEnumerable<TIXP> InputExecPins { get; }

        /// <summary>
        /// Output execution pins of this node that can pass on execution.
        /// </summary>
        public IEnumerable<TOXP> OutputExecPins { get; }
    }
}

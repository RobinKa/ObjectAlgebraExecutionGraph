using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Behaviors.DataGraph
{
    /// <summary>
    /// A graph node holding a collection of pins.
    /// </summary>
    /// <typeparam name="TIDP">Input data pin type</typeparam>
    /// <typeparam name="TODP">Output data pin type</typeparam>
    public interface IDataNode<TIDP, TODP>
    {
        /// <summary>
        /// Input data pins of this node that can receive data.
        /// </summary>
        public IEnumerable<TIDP> InputDataPins { get; }

        /// <summary>
        /// Output data pins of this node that can pass on data.
        /// </summary>
        public IEnumerable<TODP> OutputDataPins { get; }
    }
}

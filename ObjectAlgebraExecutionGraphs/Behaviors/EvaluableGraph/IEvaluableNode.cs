using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph
{
    /// <summary>
    /// An evaluable graph node holding a collection of pins.
    /// </summary>
    /// <typeparam name="TIDP">Input data pin type</typeparam>
    /// <typeparam name="TODP">Output data pin type</typeparam>
    public interface IEvaluableNode<TIDP, TODP> : IDataNode<TIDP, TODP>
    {
        public IReadOnlyDictionary<TODP, string> Evaluate();
    }
}

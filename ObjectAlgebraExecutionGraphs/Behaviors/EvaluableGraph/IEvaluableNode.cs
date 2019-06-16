using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph
{
    public interface IEvaluableNode
    {
        public IReadOnlyList<object> Evaluate(IReadOnlyList<object> inputs);
    }
}

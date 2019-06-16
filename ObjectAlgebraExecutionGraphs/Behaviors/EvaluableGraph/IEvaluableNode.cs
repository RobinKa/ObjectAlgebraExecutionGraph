using System.Collections.Generic;
using System.Collections.Immutable;

namespace ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph
{
    public interface IEvaluableNode
    {
        public IImmutableList<object> Evaluate(IImmutableList<object> inputs);
    }
}

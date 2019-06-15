using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using System;

namespace ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph
{
    public interface IEvaluableOutputDataPin : IOutputDataPin<Type>
    {
        object Evaluate();
    }
}

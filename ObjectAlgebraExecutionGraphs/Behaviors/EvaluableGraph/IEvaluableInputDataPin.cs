using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using System;

namespace ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph
{
    public interface IEvaluableInputDataPin : IInputDataPin<Type>
    {
        object Evaluate();
    }
}

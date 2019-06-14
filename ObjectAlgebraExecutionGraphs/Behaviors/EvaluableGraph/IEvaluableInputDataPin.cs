using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;

namespace ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph
{
    public interface IEvaluableInputDataPin : IInputDataPin
    {
        string Evaluate();
    }
}

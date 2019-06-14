using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;

namespace ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph
{
    public interface IEvaluableOutputDataPin : IOutputDataPin
    {
        string Evaluate();
    }
}

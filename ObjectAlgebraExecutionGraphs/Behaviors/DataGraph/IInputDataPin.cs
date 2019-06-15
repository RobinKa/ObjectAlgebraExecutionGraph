namespace ObjectAlgebraExecutionGraphs.Behaviors.DataGraph
{
    public interface IInputDataPin<TType>
    {
        TType Type { get; }
    }
}

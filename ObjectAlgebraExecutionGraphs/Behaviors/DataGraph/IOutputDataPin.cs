namespace ObjectAlgebraExecutionGraphs.Behaviors.DataGraph
{
    public interface IOutputDataPin<TType>
    {
        TType Type { get; }
    }
}

using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Variants
{
    public interface IExecutionGraphAlgebra<TType, TNode, TIXP, TOXP, TIDP, TODP> : IDataGraphAlgebra<TType, TNode, TIDP, TODP>
        where TNode : IExecutionNode<TIXP, TOXP, TIDP, TODP>
    {
        public TNode CreateConcatenateNode(TODP aFrom, TODP bFrom, TIXP execTo);
    }
}

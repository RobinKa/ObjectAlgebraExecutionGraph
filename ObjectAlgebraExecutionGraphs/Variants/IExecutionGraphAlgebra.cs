using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Variants
{
    public interface IExecutionGraphAlgebra<TNode, TIXP, TOXP, TIDP, TODP> : IDataGraphAlgebra<TNode, TIDP, TODP>
        where TNode : IExecutionNode<TIXP, TOXP, TIDP, TODP>
    {
        public TNode CreateConcatenateNode(TODP aFrom, TODP bFrom, TIXP execTo);
    }
}

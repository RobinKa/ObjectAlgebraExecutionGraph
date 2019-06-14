using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Variants.ExecutionGraph
{
    public interface IExecutionGraphAlgebra<TNode, TIXP, TOXP, TIDP, TODP>
        where TNode : INode<TIXP, TOXP, TIDP, TODP>
    {
        public TNode CreateLiteralNode(string value);
        public TNode CreateConcatenateNode(TODP aFrom, TODP bFrom, TIXP execTo);
    }
}

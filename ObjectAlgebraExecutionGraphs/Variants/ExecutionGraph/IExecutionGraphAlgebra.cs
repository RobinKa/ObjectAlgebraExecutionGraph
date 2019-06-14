using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Variants.ExecutionGraph
{
    public abstract class IExecutionGraphAlgebra<TNode, TIXP, TOXP, TIDP, TODP>
        where TNode : INode<TIXP, TOXP, TIDP, TODP>
    {
        public abstract TNode CreateLiteralNode(string value);
        public abstract TNode CreateConcatenateNode(TODP aFrom, TODP bFrom, TIXP execTo);
    }
}

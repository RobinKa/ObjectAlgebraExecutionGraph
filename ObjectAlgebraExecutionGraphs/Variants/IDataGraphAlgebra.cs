using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;

namespace ObjectAlgebraExecutionGraphs.Variants
{
    public interface IDataGraphAlgebra<TNode, TIDP, TODP>
        where TNode : IDataNode<TIDP, TODP>
    {
        public TNode CreateLiteralNode(string value);
        public TNode CreateReverseStringNode(TODP aFrom);
    }
}

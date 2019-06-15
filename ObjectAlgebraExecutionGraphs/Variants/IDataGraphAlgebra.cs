using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;

namespace ObjectAlgebraExecutionGraphs.Variants
{
    public interface IDataGraphAlgebra<TType, TNode, TIDP, TODP>
        where TNode : IDataNode<TIDP, TODP>
    {
        public TNode CreateLiteralNode(TType type, object value);
        public TNode CreateReverseStringNode(TODP aFrom);
        public TType TypeFromString(string typeString);
    }
}

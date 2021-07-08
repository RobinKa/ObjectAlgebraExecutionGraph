namespace ObjectAlgebraExecutionGraphs.Variants
{
    public interface IDataGraphAlgebra<TType, TNode>
    {
        public TNode CreateLiteralNode(TType type, object value);
        public TNode CreateReverseStringNode();
        public TType TypeFromString(string typeString);
    }
}

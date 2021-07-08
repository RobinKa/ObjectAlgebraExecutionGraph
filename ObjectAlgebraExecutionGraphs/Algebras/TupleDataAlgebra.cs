using ObjectAlgebraExecutionGraphs.Variants;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class TupleDataAlgebra<TType1, TType2, TNode1, TNode2> : IDataGraphAlgebra<(TType1, TType2), (TNode1, TNode2)>
    {
        private readonly IDataGraphAlgebra<TType1, TNode1> alg1;
        private readonly IDataGraphAlgebra<TType2, TNode2> alg2;

        public TupleDataAlgebra(IDataGraphAlgebra<TType1, TNode1> algebra1, IDataGraphAlgebra<TType2, TNode2> algebra2)
        {
            alg1 = algebra1;
            alg2 = algebra2;
        }

        public (TNode1, TNode2) CreateLiteralNode((TType1, TType2) type, object value)
            => (alg1.CreateLiteralNode(type.Item1, value), alg2.CreateLiteralNode(type.Item2, value));

        public (TNode1, TNode2) CreateReverseStringNode()
            => (alg1.CreateReverseStringNode(), alg2.CreateReverseStringNode());

        public (TType1, TType2) TypeFromString(string typeString)
            => (alg1.TypeFromString(typeString), alg2.TypeFromString(typeString));
    }
}

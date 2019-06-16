using ObjectAlgebraExecutionGraphs.Utility;
using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Variants
{
    public interface IExecutionGraphAlgebra<TType, TNode>
    {
        public TNode CreateConcatenateNode();
        public string TranslateImperative(IEnumerable<TNode> nodes, IEnumerable<NodeConnection<TNode>> dataConnections, IEnumerable<NodeConnection<TNode>> execConnections);
    }
}

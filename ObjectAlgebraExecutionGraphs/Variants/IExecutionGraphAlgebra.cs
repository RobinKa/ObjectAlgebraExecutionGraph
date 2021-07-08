using ObjectAlgebraExecutionGraphs.Utility;
using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Variants
{
    public interface IExecutionGraphAlgebra<TType, TNode>
    {
        public TNode CreateConcatenateNode();
    }
}

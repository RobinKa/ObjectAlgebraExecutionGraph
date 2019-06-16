using ObjectAlgebraExecutionGraphs.Behaviors.DotGraph;
using ObjectAlgebraExecutionGraphs.Utility;
using ObjectAlgebraExecutionGraphs.Variants;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class DotExecutionGraphAlgebra : IDataGraphAlgebra<string, IDotNode>, IExecutionGraphAlgebra<string, IDotNode>
    {
        private int nodeCounter;
        private string MakeNodeName(string prefix) => $"{prefix}{nodeCounter++}";

        public IDotNode CreateConcatenateNode() => new DotNode(MakeNodeName("Concatenate"));
        public IDotNode CreateLiteralNode(string type, object value) => new DotNode(MakeNodeName("Literal"));
        public IDotNode CreateReverseStringNode() => new DotNode(MakeNodeName("ReverseString"));

        public string TypeFromString(string typeString) => typeString;

        public string TranslateImperative(IEnumerable<IDotNode> nodes, IEnumerable<NodeConnection<IDotNode>> dataConnections, IEnumerable<NodeConnection<IDotNode>> execConnections)
            => $"digraph graph {{\n{string.Join("\n", dataConnections.Concat(execConnections).Distinct().Select(conn => $"{conn.FromNode.DotName} -> {conn.ToNode.DotName}"))}\n}}";

        private class DotNode : IDotNode
        {
            public string DotName { get; }

            public DotNode(string name)
            {
                DotName = name;
            }
        }
    }
}

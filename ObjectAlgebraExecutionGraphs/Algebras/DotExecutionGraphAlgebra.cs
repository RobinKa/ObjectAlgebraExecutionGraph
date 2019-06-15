using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.DotGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;
using ObjectAlgebraExecutionGraphs.Variants;
using System.Collections.Generic;
using System.Text;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class DotExecutionGraphAlgebra : IExecutionGraphAlgebra<string, IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>>, IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>>
    {
        public IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>> CreateLiteralNode(string type, object value)
            => new LiteralNode(type, value);

        public IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>> CreateConcatenateNode(IDotOutputDataPin<string> aFrom, IDotOutputDataPin<string> bFrom, IInputExecPin execTo)
            => new ConcatenateNode(aFrom, bFrom, execTo);

        public IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>> CreateReverseStringNode(IDotOutputDataPin<string> aFrom)
            => new ReverseStringNode(aFrom);

        public string TypeFromString(string typeString) => typeString;

        private class InputExecPin : IInputExecPin
        {
        }

        private class OutputExecPin : IDotOutputExecPin
        {
            private IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>> node;

            public OutputExecPin(IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>> node)
            {
                this.node = node;
            }

            public string GenerateDotGraph(string toName) => node.GenerateDotGraph(toName);
        }

        private class InputDataPin : IInputDataPin<string>
        {
            public string Type { get; }

            public InputDataPin(string type)
            {
                Type = type;
            }
        }

        private class OutputDataPin : IDotOutputDataPin<string>
        {
            private IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>> node;

            public OutputDataPin(IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>> node, string type)
            {
                this.node = node;
                Type = Type;
            }

            public string Type { get; }

            public string GenerateDotGraph(string toName) => node.GenerateDotGraph(toName);
        }

        private abstract class BaseNode : IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin<string>, IDotOutputDataPin<string>>
        {
            public string DotName => $"Node_{nodeId}";

            private static int nodeCounter = 0;
            private readonly int nodeId = nodeCounter++;

            public string GenerateDotGraph(string toName)
            {
                StringBuilder builder = new StringBuilder();

                if (toName != null)
                {
                    builder.AppendLine($"{DotName} -> {toName};");
                }

                builder.Append(GenerateIncomingDotGraph());

                return builder.ToString();
            }

            protected abstract string GenerateIncomingDotGraph();

            public IEnumerable<IInputExecPin> InputExecPins => ixps;
            public IEnumerable<IDotOutputExecPin> OutputExecPins => oxps;
            public IEnumerable<IInputDataPin<string>> InputDataPins => idps;
            public IEnumerable<IDotOutputDataPin<string>> OutputDataPins => odps;

            protected readonly IList<IInputDataPin<string>> idps = new List<IInputDataPin<string>>();
            protected readonly IList<IDotOutputDataPin<string>> odps = new List<IDotOutputDataPin<string>>();
            protected readonly IList<IInputExecPin> ixps = new List<IInputExecPin>();
            protected readonly IList<IDotOutputExecPin> oxps = new List<IDotOutputExecPin>();
        }

        private class LiteralNode : BaseNode
        {
            private object value;

            public LiteralNode(string type, object value)
            {
                this.value = value;
                odps.Add(new OutputDataPin(this, type));
            }

            protected override string GenerateIncomingDotGraph()
            {
                return "";
            }
        }

        private class ConcatenateNode : BaseNode
        {
            private readonly IDotOutputDataPin<string> aFrom;
            private readonly IDotOutputDataPin<string> bFrom;
            private readonly IInputExecPin execTo;

            public ConcatenateNode(IDotOutputDataPin<string> aFrom, IDotOutputDataPin<string> bFrom, IInputExecPin execTo)
            {
                ixps.Add(new InputExecPin());
                odps.Add(new OutputDataPin(this, typeof(string).AssemblyQualifiedName));
                oxps.Add(new OutputExecPin(this));

                this.aFrom = aFrom;
                this.bFrom = bFrom;
                this.execTo = execTo;
            }

            protected override string GenerateIncomingDotGraph()
            {
                StringBuilder builder = new StringBuilder();

                if (aFrom != null)
                {
                    builder.Append(aFrom.GenerateDotGraph(DotName));
                }

                if (bFrom != null && aFrom != bFrom)
                {
                    builder.Append(bFrom.GenerateDotGraph(DotName));
                }

                return builder.ToString();
            }
        }

        private class ReverseStringNode : BaseNode
        {
            private IDotOutputDataPin<string> aFrom;

            public ReverseStringNode(IDotOutputDataPin<string> aFrom)
            {
                this.aFrom = aFrom;

                idps.Add(new InputDataPin(typeof(string).AssemblyQualifiedName));
                odps.Add(new OutputDataPin(this, typeof(string).AssemblyQualifiedName));
            }

            protected override string GenerateIncomingDotGraph()
            {
                StringBuilder builder = new StringBuilder();

                if (aFrom != null)
                {
                    builder.Append(aFrom.GenerateDotGraph(DotName));
                }

                return builder.ToString();
            }
        }
    }
}

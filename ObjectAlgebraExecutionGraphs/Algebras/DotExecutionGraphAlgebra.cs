using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.DotFileGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;
using ObjectAlgebraExecutionGraphs.Variants;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class DotExecutionGraphAlgebra : IExecutionGraphAlgebra<IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin>, IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin>
    {
        public IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin> CreateLiteralNode(string value)
            => new LiteralNode(value);

        public IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin> CreateConcatenateNode(IDotOutputDataPin aFrom, IDotOutputDataPin bFrom, IInputExecPin execTo)
            => new ConcatenateNode(aFrom, bFrom, execTo);

        private class InputExecPin : IInputExecPin
        {
        }

        private class OutputExecPin : IDotOutputExecPin
        {
            private IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin> node;

            public OutputExecPin(IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin> node)
            {
                this.node = node;
            }

            public string GenerateDotGraph(string toName) => node.GenerateDotGraph(toName);
        }

        private class InputDataPin : IInputDataPin
        {
        }

        private class OutputDataPin : IDotOutputDataPin
        {
            private IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin> node;

            public OutputDataPin(IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin> node)
            {
                this.node = node;
            }

            public string GenerateDotGraph(string toName) => node.GenerateDotGraph(toName);
        }

        private abstract class BaseNode : IDotNode<IInputExecPin, IDotOutputExecPin, IInputDataPin, IDotOutputDataPin>
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
            public IEnumerable<IInputDataPin> InputDataPins => idps;
            public IEnumerable<IDotOutputDataPin> OutputDataPins => odps;

            protected readonly IList<IInputDataPin> idps = new List<IInputDataPin>();
            protected readonly IList<IDotOutputDataPin> odps = new List<IDotOutputDataPin>();
            protected readonly IList<IInputExecPin> ixps = new List<IInputExecPin>();
            protected readonly IList<IDotOutputExecPin> oxps = new List<IDotOutputExecPin>();
        }

        private class LiteralNode : BaseNode
        {
            private string value;

            public LiteralNode(string value)
            {
                this.value = value;
                odps.Add(new OutputDataPin(this));
            }

            protected override string GenerateIncomingDotGraph()
            {
                return "";
            }
        }

        private class ConcatenateNode : BaseNode
        {
            private readonly IDotOutputDataPin aFrom;
            private readonly IDotOutputDataPin bFrom;
            private readonly IInputExecPin execTo;

            public ConcatenateNode(IDotOutputDataPin aFrom, IDotOutputDataPin bFrom, IInputExecPin execTo)
            {
                ixps.Add(new InputExecPin());
                odps.Add(new OutputDataPin(this));
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
    }
}

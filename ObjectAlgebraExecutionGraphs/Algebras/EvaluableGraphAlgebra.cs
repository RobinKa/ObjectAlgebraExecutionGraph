using ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph;
using ObjectAlgebraExecutionGraphs.Variants;
using System.Collections.Generic;
using System.Linq;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class EvaluableGraphAlgebra : IDataGraphAlgebra<IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin>, IEvaluableInputDataPin, IEvaluableOutputDataPin>
    {
        public IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> CreateLiteralNode(string value)
            => new LiteralNode(value);

        public IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> CreateReverseStringNode(IEvaluableOutputDataPin aFrom)
            => new ReverseStringNode(aFrom);

        private class InputDataPin : IEvaluableInputDataPin
        {
            private readonly IEvaluableOutputDataPin incomingOdp;

            public InputDataPin(IEvaluableOutputDataPin incomingOdp)
            {
                this.incomingOdp = incomingOdp;
            }

            public string Evaluate()
            {
                return incomingOdp.Evaluate();
            }
        }

        private class OutputDataPin : IEvaluableOutputDataPin
        {
            private readonly IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> node;

            public OutputDataPin(IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> node)
            {
                this.node = node;
            }

            public string Evaluate()
            {
                return node.Evaluate()[this];
            }
        }

        private abstract class BaseNode : IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin>
        {
            public IEnumerable<IEvaluableInputDataPin> InputDataPins => idps;
            public IEnumerable<IEvaluableOutputDataPin> OutputDataPins => odps;

            protected readonly IList<IEvaluableInputDataPin> idps = new List<IEvaluableInputDataPin>();
            protected readonly IList<IEvaluableOutputDataPin> odps = new List<IEvaluableOutputDataPin>();

            public abstract IReadOnlyDictionary<IEvaluableOutputDataPin, string> Evaluate();
        }

        private class LiteralNode : BaseNode
        {
            private string value;

            public LiteralNode(string value)
            {
                this.value = value;
                odps.Add(new OutputDataPin(this));
            }

            public override IReadOnlyDictionary<IEvaluableOutputDataPin, string> Evaluate()
            {
                return new Dictionary<IEvaluableOutputDataPin, string> { [odps.Single()] = value };
            }
        }

        private class ReverseStringNode : BaseNode
        {
            public ReverseStringNode(IEvaluableOutputDataPin aFrom)
            {
                idps.Add(new InputDataPin(aFrom));
                odps.Add(new OutputDataPin(this));
            }

            public override IReadOnlyDictionary<IEvaluableOutputDataPin, string> Evaluate()
            {
                return new Dictionary<IEvaluableOutputDataPin, string>() { [odps.Single()] = string.Concat(idps.Single().Evaluate().Reverse()) };
            }
        }
    }
}

using ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class EvaluableGraphAlgebra : IDataGraphAlgebra<Type, IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin>, IEvaluableInputDataPin, IEvaluableOutputDataPin>
    {
        public IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> CreateLiteralNode(Type type, object value)
            => new LiteralNode(type, value);

        public IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> CreateReverseStringNode(IEvaluableOutputDataPin aFrom)
            => new ReverseStringNode(aFrom);

        public Type TypeFromString(string typeString) => Type.GetType(typeString);

        private class InputDataPin : IEvaluableInputDataPin
        {
            public Type Type { get; }

            private readonly IEvaluableOutputDataPin incomingOdp;

            public InputDataPin(Type type, IEvaluableOutputDataPin incomingOdp)
            {
                this.incomingOdp = incomingOdp;
                Type = type;
            }

            public object Evaluate()
            {
                return incomingOdp.Evaluate();
            }
        }

        private class OutputDataPin : IEvaluableOutputDataPin
        {
            public Type Type { get; }

            private readonly IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> node;

            public OutputDataPin(Type type, IEvaluableNode<IEvaluableInputDataPin, IEvaluableOutputDataPin> node)
            {
                this.node = node;
                Type = type;
            }

            public object Evaluate()
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

            public abstract IReadOnlyDictionary<IEvaluableOutputDataPin, object> Evaluate();
        }

        private class LiteralNode : BaseNode
        {
            private readonly Type type;
            private readonly object value;

            public LiteralNode(Type type, object value)
            {
                this.type = type;
                this.value = value;
                odps.Add(new OutputDataPin(type, this));
            }

            public override IReadOnlyDictionary<IEvaluableOutputDataPin, object> Evaluate()
            {
                return new Dictionary<IEvaluableOutputDataPin, object> { [odps.Single()] = value };
            }
        }

        private class ReverseStringNode : BaseNode
        {
            public ReverseStringNode(IEvaluableOutputDataPin aFrom)
            {
                idps.Add(new InputDataPin(typeof(string), aFrom));
                odps.Add(new OutputDataPin(typeof(string), this));
            }

            public override IReadOnlyDictionary<IEvaluableOutputDataPin, object> Evaluate()
            {
                return new Dictionary<IEvaluableOutputDataPin, object>() { [odps.Single()] = string.Concat(((string)idps.Single().Evaluate()).Reverse()) };
            }
        }
    }
}

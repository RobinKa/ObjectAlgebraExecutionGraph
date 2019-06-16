﻿using ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class EvaluableGraphAlgebra : IDataGraphAlgebra<Type, IEvaluableNode>
    {
        public IEvaluableNode CreateLiteralNode(Type type, object value) => new LiteralNode(type, value);

        public IEvaluableNode CreateReverseStringNode() => new ReverseStringNode();

        public Type TypeFromString(string typeString) => Type.GetType(typeString);

        private class LiteralNode : IEvaluableNode
        {
            private readonly object value;

            public LiteralNode(Type type, object value)
            {
                this.value = value;
            }

            public IReadOnlyList<object> Evaluate(IReadOnlyList<object> inputs)
            {
                if (inputs.Count != 0)
                {
                    throw new Exception();
                }

                return new[] { value };
            }
        }

        private class ReverseStringNode : IEvaluableNode
        {
            public IReadOnlyList<object> Evaluate(IReadOnlyList<object> inputs)
            {
                return new[] { string.Concat(inputs.Cast<string>().Single().Reverse()) };
            }
        }
    }
}

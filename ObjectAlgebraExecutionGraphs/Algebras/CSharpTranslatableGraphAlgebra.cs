using ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph;
using ObjectAlgebraExecutionGraphs.Utility;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class CSharpTranslatableGraphAlgebra : IDataGraphAlgebra<Type, ICSharpTranslatableNode>, IExecutionGraphAlgebra<Type, ICSharpTranslatableNode>
    {
        public ICSharpTranslatableNode CreateConcatenateNode() => new ConcatenateNode();
        public ICSharpTranslatableNode CreateLiteralNode(Type type, object value) => new LiteralNode(type, value);
        public ICSharpTranslatableNode CreateReverseStringNode() => new ReverseStringNode();

        public Type TypeFromString(string typeString) => Type.GetType(typeString);

        public string TranslateImperative(IEnumerable<ICSharpTranslatableNode> nodes, IEnumerable<NodeConnection<ICSharpTranslatableNode>> dataConnections, IEnumerable<NodeConnection<ICSharpTranslatableNode>> execConnections)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var node in nodes)
            {
                builder.Append(node.TranslateVariables());
            }

            foreach (var node in nodes)
            {
                builder.Append(node.TranslatePureFunctions());
            }

            foreach (var node in nodes)
            {
                builder.Append(node.TranslateStates());
            }

            return builder.ToString();
        }

        private abstract class BaseCSharpTranslatableNode : ICSharpTranslatableNode
        {
            public IImmutableList<(Type type, string variableName)> Inputs { get; private set; } = ImmutableList<(Type, string)>.Empty;
            public IImmutableList<(Type type, string variableName)> Outputs { get; private set; } = ImmutableList<(Type, string)>.Empty;

            public virtual bool IsPure { get; }
            public string PureFunctionName { get; }
            private static int nodeCounter;

            public BaseCSharpTranslatableNode()
            {
                PureFunctionName = $"{GetType().Name}_{nodeCounter++}";
            }

            public virtual string TranslateVariables() => string.Concat(Inputs.Concat(Outputs).Select(x => $"{x.type} {x.variableName} = default({x.type});\n"));

            public virtual string TranslatePureFunctions() => "";

            public virtual string TranslateStates() => "";

            protected void AddInput(Type type) => Inputs = Inputs.Add((type, RandomGenerator.GetRandomLowerLetters(8)));
            protected void AddOutput(Type type) => Outputs = Outputs.Add((type, RandomGenerator.GetRandomLowerLetters(8)));
        }

        private class ConcatenateNode : BaseCSharpTranslatableNode
        {
            public ConcatenateNode()
            {
                AddInput(typeof(string));
                AddInput(typeof(string));
                AddOutput(typeof(string));
            }

            public override string TranslateStates() => $"{Outputs.Single().variableName} = {Inputs[0].variableName} + \" \" + {Inputs[1].variableName};\n";
        }

        private class LiteralNode : BaseCSharpTranslatableNode
        {
            public override bool IsPure => true;

            private object value;

            public LiteralNode(Type type, object value)
            {
                this.value = value;
                AddOutput(type);
            }

            public override string TranslateVariables()
            {
                var output = Outputs.Single();
                return $"const {output.type.FullName} {output.variableName} = {value};\n";
            }
        }

        private class ReverseStringNode : BaseCSharpTranslatableNode
        {
            public override bool IsPure => true;

            public ReverseStringNode()
            {
                AddInput(typeof(string));
                AddOutput(typeof(string));
            }

            public override string TranslatePureFunctions()
            {
                return $"{Outputs.Single().variableName} = string.Concat({Inputs.Single().variableName}.Reverse());\n";
            }
        }
    }
}

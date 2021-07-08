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

        private abstract class BaseCSharpTranslatableNode : ICSharpTranslatableNode
        {
            public IImmutableList<(Type type, string variableName)> Inputs { get; private set; } = ImmutableList<(Type, string)>.Empty;
            public IImmutableList<(Type type, string variableName)> Outputs { get; private set; } = ImmutableList<(Type, string)>.Empty;
            public IImmutableList<string> ExecInputs { get; private set; } = ImmutableList<string>.Empty;
            public int ExecOutputCount { get; private set; }

            public virtual bool IsPure { get; }
            public string PureFunctionName { get; }
            private static int nodeCounter;

            public BaseCSharpTranslatableNode()
            {
                PureFunctionName = $"{GetType().Name}_{nodeCounter++}";
            }

            public virtual string TranslateVariables() => string.Concat(Inputs.Concat(Outputs).Select(x => $"{x.type} {x.variableName} = default({x.type});\n"));

            public virtual string TranslatePureFunctions() => "";

            public virtual string TranslateStates(IImmutableList<string> outputExecLabels, string pureCalls) => "";

            protected void AddInput(Type type) => Inputs = Inputs.Add((type, RandomGenerator.GetRandomLowerLetters(8)));
            protected void AddOutput(Type type) => Outputs = Outputs.Add((type, RandomGenerator.GetRandomLowerLetters(8)));
            protected void AddExecInput() => ExecInputs = ExecInputs.Add(RandomGenerator.GetRandomLowerLetters(8));
            protected void AddExecOutput() => ExecOutputCount++;
        }

        private class ConcatenateNode : BaseCSharpTranslatableNode
        {
            public ConcatenateNode()
            {
                AddInput(typeof(string));
                AddInput(typeof(string));
                AddOutput(typeof(string));
                AddExecInput();
                AddExecOutput();
            }

            public override string TranslateStates(IImmutableList<string> outputExecLabels, string pureCalls)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append($"{ExecInputs[0]}:\n");
                builder.Append(pureCalls);
                builder.Append($"{Outputs.Single().variableName} = {Inputs[0].variableName} + \" \" + {Inputs[1].variableName};\n");

                var outputLabel = outputExecLabels.Single();
                if (outputLabel != null)
                {
                    builder.Append($"goto {outputLabel};\n");
                }
                else
                {
                    builder.Append("return;\n");
                }

                return builder.ToString();
            }
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

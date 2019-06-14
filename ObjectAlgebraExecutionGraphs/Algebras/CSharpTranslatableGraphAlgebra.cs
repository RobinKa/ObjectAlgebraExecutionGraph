using ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph;
using ObjectAlgebraExecutionGraphs.Utility;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class CSharpTranslatableGraphAlgebra : IExecutionGraphAlgebra<ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin>, ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin>
    {
        public ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin> CreateLiteralNode(string value)
            => new LiteralNode(value);

        public ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin> CreateConcatenateNode(ICSharpTranslatableOutputDataPin aFrom, ICSharpTranslatableOutputDataPin bFrom, ICSharpTranslatableInputExecPin execTo)
            => new ConcatenateNode(aFrom, bFrom, execTo);

        public ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin> CreateReverseStringNode(ICSharpTranslatableOutputDataPin aFrom)
            => new ReverseStringNode(aFrom);

        private class InputExecPin : ICSharpTranslatableInputExecPin
        {
            public string Label { get; } = RandomGenerator.GetRandomLowerLetters(16);
        }

        private class OutputExecPin : ICSharpTranslatableOutputExecPin
        {

        }

        private class InputDataPin : ICSharpTranslatableInputDataPin
        {
            public string VariableName { get; } = RandomGenerator.GetRandomLowerLetters(16);
        }

        private class OutputDataPin : ICSharpTranslatableOutputDataPin
        {
            public string VariableName { get; } = RandomGenerator.GetRandomLowerLetters(16);
            public bool IsPure => Node.IsPure;
            public ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin> Node { get; }

            public string TranslateCallPureFunction()
            {
                if (!IsPure)
                {
                    throw new Exception();
                }

                return Node.TranslateCallPureFunction();
            }

            public OutputDataPin(ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin> node)
            {
                Node = node;
            }
        }

        private abstract class BaseCSharpTranslatableNode : ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin, ICSharpTranslatableOutputDataPin>
        {
            public IEnumerable<ICSharpTranslatableInputExecPin> InputExecPins => ixps;
            public IEnumerable<ICSharpTranslatableOutputExecPin> OutputExecPins => oxps;
            public IEnumerable<ICSharpTranslatableInputDataPin> InputDataPins => idps;
            public IEnumerable<ICSharpTranslatableOutputDataPin> OutputDataPins => odps;
            
            protected readonly IList<ICSharpTranslatableInputDataPin> idps = new List<ICSharpTranslatableInputDataPin>();
            protected readonly IList<ICSharpTranslatableOutputDataPin> odps = new List<ICSharpTranslatableOutputDataPin>();
            protected readonly IList<ICSharpTranslatableInputExecPin> ixps = new List<ICSharpTranslatableInputExecPin>();
            protected readonly IList<ICSharpTranslatableOutputExecPin> oxps = new List<ICSharpTranslatableOutputExecPin>();

            public virtual bool IsPure { get; }
            public string PureFunctionName { get; }
            private static int nodeCounter;

            public BaseCSharpTranslatableNode()
            {
                PureFunctionName = $"{GetType().Name}_{nodeCounter++}";
            }

            public virtual string TranslateVariables()
            {
                return "";
            }

            public virtual string TranslatePureFunctions()
            {
                return "";
            }

            public virtual string TranslateStates()
            {
                return "";
            }
        }

        private class ConcatenateNode : BaseCSharpTranslatableNode
        {
            private readonly ICSharpTranslatableOutputDataPin aFrom;
            private readonly ICSharpTranslatableOutputDataPin bFrom;
            private readonly ICSharpTranslatableInputExecPin execTo;

            public ConcatenateNode(ICSharpTranslatableOutputDataPin aFrom, ICSharpTranslatableOutputDataPin bFrom, ICSharpTranslatableInputExecPin execTo)
            {
                ixps.Add(new InputExecPin());
                idps.Add(new InputDataPin()); // TODO: Use these instead of using the passed output data pins directly.
                idps.Add(new InputDataPin());
                odps.Add(new OutputDataPin(this));
                oxps.Add(new OutputExecPin());

                this.aFrom = aFrom;
                this.bFrom = bFrom;
                this.execTo = execTo;
            }

            public override string TranslateVariables()
            {
                return $"var {odps.Single().VariableName} = default(string);\n";
            }

            public override string TranslateStates()
            {
                StringBuilder builder = new StringBuilder();

                // Translate label
                builder.Append($"{ixps[0].Label}:\n");

                // Translate pure calls
                var pureCalls = new[] { aFrom, bFrom }
                                    .Distinct()
                                    .Where(pin => pin.IsPure)
                                    .Select(pin => pin.TranslateCallPureFunction());
                builder.Append(string.Concat(pureCalls));

                // Translate actual logic and result assignment
                builder.Append($"{odps.Single().VariableName} = {aFrom.VariableName} + \" \" + {bFrom.VariableName};\n");

                // Translate goto next
                if (execTo != null)
                {
                    builder.Append($"goto {execTo.Label};\n");
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

            private string value;

            public LiteralNode(string value)
            {
                this.value = value;
                odps.Add(new OutputDataPin(this));
            }

            public override string TranslateVariables()
            {
                return $"var {OutputDataPins.Single().VariableName} = {value};\n";
            }

            public override string TranslatePureFunctions()
            {
                StringBuilder builder = new StringBuilder();

                builder.Append($"void {PureFunctionName}()\n");
                builder.Append("{\n");
                //builder.Append($"{OutputDataPins.Single().VariableName} = {value};\n");
                builder.Append("}\n");

                return builder.ToString();
            }
        }

        private class ReverseStringNode : BaseCSharpTranslatableNode
        {
            public override bool IsPure => true;

            private ICSharpTranslatableOutputDataPin aFrom;

            public ReverseStringNode(ICSharpTranslatableOutputDataPin aFrom)
            {
                this.aFrom = aFrom;

                idps.Add(new InputDataPin());
                odps.Add(new OutputDataPin(this));
            }

            public override string TranslateVariables()
            {
                return $"var {OutputDataPins.Single().VariableName} = default(string);\n";
            }

            public override string TranslatePureFunctions()
            {
                StringBuilder builder = new StringBuilder();

                builder.Append($"void {PureFunctionName}()\n");
                builder.Append("{\n");
                builder.Append($"{OutputDataPins.Single().VariableName} = string.Concat({aFrom.VariableName}.Reverse());\n");
                builder.Append("}\n");

                return builder.ToString();
            }
        }
    }
}

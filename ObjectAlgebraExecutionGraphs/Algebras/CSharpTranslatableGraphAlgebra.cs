using ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph;
using ObjectAlgebraExecutionGraphs.Utility;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class CSharpTranslatableGraphAlgebra : IExecutionGraphAlgebra<Type, ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>>, ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>>
    {
        public ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>> CreateLiteralNode(Type type, object value)
            => new LiteralNode(type, value);

        public ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>> CreateConcatenateNode(ICSharpTranslatableOutputDataPin<Type> aFrom, ICSharpTranslatableOutputDataPin<Type> bFrom, ICSharpTranslatableInputExecPin execTo)
            => new ConcatenateNode(aFrom, bFrom, execTo);

        public ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>> CreateReverseStringNode(ICSharpTranslatableOutputDataPin<Type> aFrom)
            => new ReverseStringNode(aFrom);

        public Type TypeFromString(string typeString) => Type.GetType(typeString);

        private class InputExecPin : ICSharpTranslatableInputExecPin
        {
            public string Label { get; } = RandomGenerator.GetRandomLowerLetters(16);
        }

        private class OutputExecPin : ICSharpTranslatableOutputExecPin
        {

        }

        private class InputDataPin : ICSharpTranslatableInputDataPin<Type>
        {
            public string VariableName { get; } = RandomGenerator.GetRandomLowerLetters(16);
            public Type Type { get; }

            private readonly ICSharpTranslatableOutputDataPin<Type> incomingPin;

            public InputDataPin(Type type, ICSharpTranslatableOutputDataPin<Type> incomingPin)
            {
                Type = type;
                this.incomingPin = incomingPin;
            }

            public IEnumerable<string> TranslateCallPureFunction()
            {
                if (incomingPin?.IsPure == true)
                {
                    return incomingPin.TranslateCallPureFunction();
                }

                return new string[0];
            }
        }

        private class OutputDataPin : ICSharpTranslatableOutputDataPin<Type>
        {
            public string VariableName { get; } = RandomGenerator.GetRandomLowerLetters(16);
            public bool IsPure => node.IsPure;
            public Type Type { get; }

            private readonly ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>> node;

            public OutputDataPin(ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>> node, Type type)
            {
                this.node = node;
                Type = type;
            }

            public IEnumerable<string> TranslateCallPureFunction()
            {
                if (!IsPure)
                {
                    throw new Exception();
                }

                return node.TranslateCallPureFunction();
            }
        }

        private abstract class BaseCSharpTranslatableNode : ICSharpTranslatableNode<ICSharpTranslatableInputExecPin, ICSharpTranslatableOutputExecPin, ICSharpTranslatableInputDataPin<Type>, ICSharpTranslatableOutputDataPin<Type>>
        {
            public IEnumerable<ICSharpTranslatableInputExecPin> InputExecPins => ixps;
            public IEnumerable<ICSharpTranslatableOutputExecPin> OutputExecPins => oxps;
            public IEnumerable<ICSharpTranslatableInputDataPin<Type>> InputDataPins => idps;
            public IEnumerable<ICSharpTranslatableOutputDataPin<Type>> OutputDataPins => odps;
            
            protected readonly IList<ICSharpTranslatableInputDataPin<Type>> idps = new List<ICSharpTranslatableInputDataPin<Type>>();
            protected readonly IList<ICSharpTranslatableOutputDataPin<Type>> odps = new List<ICSharpTranslatableOutputDataPin<Type>>();
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

            public IEnumerable<string> TranslateCallPureFunction()
            {
                var pureCalls = idps.SelectMany(idp => idp.TranslateCallPureFunction());

                if (IsPure)
                {
                    pureCalls = pureCalls.Concat(new[] { $"{PureFunctionName}();" });
                }

                return pureCalls.Distinct();
            }
        }

        private class ConcatenateNode : BaseCSharpTranslatableNode
        {
            private readonly ICSharpTranslatableInputExecPin execTo;

            public ConcatenateNode(ICSharpTranslatableOutputDataPin<Type> aFrom, ICSharpTranslatableOutputDataPin<Type> bFrom, ICSharpTranslatableInputExecPin execTo)
            {
                ixps.Add(new InputExecPin());
                idps.Add(new InputDataPin(typeof(string), aFrom));
                idps.Add(new InputDataPin(typeof(string), bFrom));
                odps.Add(new OutputDataPin(this, typeof(string)));
                oxps.Add(new OutputExecPin());

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
                builder.Append(string.Join("\n", TranslateCallPureFunction()));
                builder.Append("\n");

                // Translate actual logic and result assignment
                builder.Append($"{odps.Single().VariableName} = {idps[0].VariableName} + \" \" + {idps[1].VariableName};\n");

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

            private object value;

            public LiteralNode(Type type, object value)
            {
                this.value = value;
                odps.Add(new OutputDataPin(this, type));
            }

            public override string TranslateVariables()
            {
                var odp = OutputDataPins.Single();

                return $"const {odp.Type.FullName} {odp.VariableName} = {value};\n";
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

            public ReverseStringNode(ICSharpTranslatableOutputDataPin<Type> aFrom)
            {
                idps.Add(new InputDataPin(typeof(string), aFrom));
                odps.Add(new OutputDataPin(this, typeof(string)));
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
                builder.Append($"{OutputDataPins.Single().VariableName} = string.Concat({idps.Single().VariableName}.Reverse());\n");
                builder.Append("}\n");

                return builder.ToString();
            }
        }
    }
}

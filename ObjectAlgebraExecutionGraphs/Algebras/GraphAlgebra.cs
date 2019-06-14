using System.Collections.Generic;
using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;
using ObjectAlgebraExecutionGraphs.Variants.ExecutionGraph;

namespace ObjectAlgebraExecutionGraphs.Algebras
{
    public class ExecutionGraphAlgebra : IExecutionGraphAlgebra<INode<IInputExecPin, IOutputExecPin, IInputDataPin, IOutputDataPin>, IInputExecPin, IOutputExecPin, IInputDataPin, IOutputDataPin>
    {
        public INode<IInputExecPin, IOutputExecPin, IInputDataPin, IOutputDataPin> CreateLiteralNode(string value)
            => new LiteralNode(value);

        public INode<IInputExecPin, IOutputExecPin, IInputDataPin, IOutputDataPin> CreateConcatenateNode(IOutputDataPin aFrom, IOutputDataPin bFrom, IInputExecPin execTo)
            => new ConcatenateNode(aFrom, bFrom, execTo);

        private class InputExecPin : IInputExecPin
        {
        }

        private class OutputExecPin : IOutputExecPin
        {
        }

        private class InputDataPin : IInputDataPin
        {
        }

        private class OutputDataPin : IOutputDataPin
        {
        }

        private class BaseNode : INode<IInputExecPin, IOutputExecPin, IInputDataPin, IOutputDataPin>
        {
            public IEnumerable<IInputExecPin> InputExecPins => ixps;
            public IEnumerable<IOutputExecPin> OutputExecPins => oxps;
            public IEnumerable<IInputDataPin> InputDataPins => idps;
            public IEnumerable<IOutputDataPin> OutputDataPins => odps;

            protected readonly IList<IInputDataPin> idps = new List<IInputDataPin>();
            protected readonly IList<IOutputDataPin> odps = new List<IOutputDataPin>();
            protected readonly IList<IInputExecPin> ixps = new List<IInputExecPin>();
            protected readonly IList<IOutputExecPin> oxps = new List<IOutputExecPin>();
        }

        private class LiteralNode : BaseNode
        {
            private string value;

            public LiteralNode(string value)
            {
                this.value = value;
                odps.Add(new OutputDataPin());
            }
        }

        private class ConcatenateNode : BaseNode
        {
            private readonly IOutputDataPin aFrom;
            private readonly IOutputDataPin bFrom;
            private readonly IInputExecPin execTo;

            public ConcatenateNode(IOutputDataPin aFrom, IOutputDataPin bFrom, IInputExecPin execTo)
            {
                ixps.Add(new InputExecPin());
                odps.Add(new OutputDataPin());
                oxps.Add(new OutputExecPin());

                this.aFrom = aFrom;
                this.bFrom = bFrom;
                this.execTo = execTo;
            }
        }
    }
}

using ObjectAlgebraExecutionGraphs.Algebras;
using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;
using ObjectAlgebraExecutionGraphs.Variants.ExecutionGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectAlgebraExecutionGraphs
{
    internal static class Program
    {
        private static IEnumerable<TNode> CreateExecutionGraph<TNode, TIXP, TOXP, TIDP, TODP>(TIXP next, IExecutionGraphAlgebra<TNode, TIXP, TOXP, TIDP, TODP> factory)
            where TNode : INode<TIXP, TOXP, TIDP, TODP>
        {
            var literalNode = factory.CreateLiteralNode("\"hello\"");
            var concatenateNode = factory.CreateConcatenateNode(literalNode.OutputDataPins.Single(), literalNode.OutputDataPins.Single(), next);

            return new[] { literalNode, concatenateNode };
        }

        private static void Main()
        {
            // Create an execution graph without any functionality
            var factory = new ExecutionGraphAlgebra();
            var executionGraph = CreateExecutionGraph(null, factory);

            // Create the same execution graph, but now with elements translatable to C#
            var csharpTranslatableFactory = new CSharpTranslatableGraphAlgebra();
            var csharpTranslatableGraph = CreateExecutionGraph(null, csharpTranslatableFactory);

            Console.WriteLine("void Func()");
            Console.WriteLine("{");
            foreach (var node in csharpTranslatableGraph)
            {
                Console.WriteLine(node.TranslateVariables());
            }
            foreach (var node in csharpTranslatableGraph)
            {
                Console.WriteLine(node.TranslatePureFunctions());
            }
            foreach (var node in csharpTranslatableGraph)
            {
                Console.WriteLine(node.TranslateStates());
            }
            Console.WriteLine("}");
        }
    }
}

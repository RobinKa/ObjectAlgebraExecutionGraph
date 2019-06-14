using ObjectAlgebraExecutionGraphs.Algebras;
using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectAlgebraExecutionGraphs
{
    internal static class Program
    {
        private static IEnumerable<TNode> CreateExecutionGraph<TNode, TIXP, TOXP, TIDP, TODP>(TIXP next, IExecutionGraphAlgebra<TNode, TIXP, TOXP, TIDP, TODP> factory)
            where TNode : IExecutionNode<TIXP, TOXP, TIDP, TODP>
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

            // Generate a DOT graph from the same execution graph's last node
            var dotFactory = new DotExecutionGraphAlgebra();
            var dotExecutionGraph = CreateExecutionGraph(null, dotFactory);
            Console.WriteLine("--- DOT graph ---");
            Console.WriteLine("graph dotGraph {");
            Console.Write(dotExecutionGraph.Last().GenerateDotGraph(null));
            Console.WriteLine("}");
            Console.WriteLine();

            // Create the same execution graph, but now with elements translatable to C#
            var csharpTranslatableFactory = new CSharpTranslatableGraphAlgebra();
            var csharpTranslatableGraph = CreateExecutionGraph(null, csharpTranslatableFactory);

            Console.WriteLine("--- C# translated graph ---");
            Console.WriteLine("void Func()");
            Console.WriteLine("{");
            foreach (var node in csharpTranslatableGraph)
            {
                Console.Write(node.TranslateVariables());
            }
            foreach (var node in csharpTranslatableGraph)
            {
                Console.Write(node.TranslatePureFunctions());
            }
            foreach (var node in csharpTranslatableGraph)
            {
                Console.Write(node.TranslateStates());
            }
            Console.WriteLine("}");
        }
    }
}

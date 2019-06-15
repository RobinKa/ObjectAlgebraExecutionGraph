using ObjectAlgebraExecutionGraphs.Algebras;
using ObjectAlgebraExecutionGraphs.Behaviors.DataGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.ExecutionGraph;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectAlgebraExecutionGraphs
{
    internal static class Program
    {
        private static IEnumerable<TNode> CreateDataGraph<TType, TNode, TIDP, TODP>(IDataGraphAlgebra<TType, TNode, TIDP, TODP> factory)
           where TNode : IDataNode<TIDP, TODP>
        {
            var literalType = factory.TypeFromString(typeof(string).AssemblyQualifiedName);
            var literalNode = factory.CreateLiteralNode(literalType, "\"hello\"");
            var reverseStringNode = factory.CreateReverseStringNode(literalNode.OutputDataPins.Single());
            var reverseStringNode2 = factory.CreateReverseStringNode(reverseStringNode.OutputDataPins.Single());

            return new[] { literalNode, reverseStringNode, reverseStringNode2 };
        }

        private static IEnumerable<TNode> CreateExecutionGraph<TType, TNode, TIXP, TOXP, TIDP, TODP>(TIXP next, IExecutionGraphAlgebra<TType, TNode, TIXP, TOXP, TIDP, TODP> factory)
            where TNode : IExecutionNode<TIXP, TOXP, TIDP, TODP>
        {
            var literalType = factory.TypeFromString(typeof(string).AssemblyQualifiedName);
            var literalNode = factory.CreateLiteralNode(literalType, "\"hello\"");
            var reverseStringNode = factory.CreateReverseStringNode(literalNode.OutputDataPins.Single());
            var concatenateNode = factory.CreateConcatenateNode(literalNode.OutputDataPins.Single(), reverseStringNode.OutputDataPins.Single(), next);

            return new[] { literalNode, reverseStringNode, concatenateNode };
        }

        private static void RunExecutionGraphExamples()
        {
            // Generate a DOT graph from the an execution graph's last node
            var dotFactory = new DotExecutionGraphAlgebra();
            var dotExecutionGraph = CreateExecutionGraph(null, dotFactory);
            Console.WriteLine("--- DOT graph ---");
            Console.WriteLine("digraph dotGraph {");
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

        private static void RunDataGraphExamples()
        {
            // Create an evaluable data graph and evaluate it
            var evaluableFactory = new EvaluableGraphAlgebra();
            var evaluableDataGraph = CreateDataGraph(evaluableFactory);

            Console.Write("--- Evaluable graph ---");
            Console.WriteLine("Last node outputs:");
            foreach ((var odp, var value) in evaluableDataGraph.Last().Evaluate())
            {
                Console.WriteLine(value);
            }

            // Generate a DOT graph from the same data graph's last node
            var dotFactory = new DotExecutionGraphAlgebra();
            var dotExecutionGraph = CreateDataGraph(dotFactory);
            Console.WriteLine("--- DOT graph ---");
            Console.WriteLine("digraph dotGraph {");
            Console.Write(dotExecutionGraph.Last().GenerateDotGraph(null));
            Console.WriteLine("}");
            Console.WriteLine();

            // Create the same data graph, but now with elements translatable to C#
            var csharpTranslatableFactory = new CSharpTranslatableGraphAlgebra();
            var csharpTranslatableGraph = CreateDataGraph(csharpTranslatableFactory);

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

            // Since we don't have any states, translate a call to the last pure node instead.
            Console.WriteLine(string.Join("\n", csharpTranslatableGraph.Last().TranslateCallPureFunction().Distinct()));

            Console.WriteLine("}");
        }

        private static void Main()
        {
            Console.WriteLine("### Data graph examples ###");
            RunDataGraphExamples();

            Console.WriteLine("### Execution graph examples ###");
            RunExecutionGraphExamples();
        }
    }
}

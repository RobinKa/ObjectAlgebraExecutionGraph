using ObjectAlgebraExecutionGraphs.Algebras;
using ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.DotGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph;
using ObjectAlgebraExecutionGraphs.Utility;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs
{
    internal static class Program
    {
        private static (IEnumerable<TNode> nodes, IEnumerable<NodeConnection<TNode>> dataConnections) CreateDataGraph<TType, TNode, TFactory>(TFactory factory)
            where TFactory : IDataGraphAlgebra<TType, TNode>
        {
            var literalType = factory.TypeFromString(typeof(string).AssemblyQualifiedName);
            var literalNode = factory.CreateLiteralNode(literalType, "\"hello\"");
            var reverseStringNode = factory.CreateReverseStringNode();
            var reverseStringNode2 = factory.CreateReverseStringNode();

            var literalToReverse = new NodeConnection<TNode>(literalNode, 0, reverseStringNode, 0);
            var reverseToReverse2 = new NodeConnection<TNode>(reverseStringNode, 0, reverseStringNode2, 0);

            return (new[] { literalNode, reverseStringNode, reverseStringNode2 }, new[] { literalToReverse, reverseToReverse2 });
        }

        private static (IEnumerable<TNode> nodes, IEnumerable<NodeConnection<TNode>> dataConnections, IReadOnlyList<NodeConnection<TNode>> execConnections) CreateExecutionGraph<TType, TNode, TFactory>(TFactory factory)
            where TFactory : IDataGraphAlgebra<TType, TNode>, IExecutionGraphAlgebra<TType, TNode>
        {
            var literalType = factory.TypeFromString(typeof(string).AssemblyQualifiedName);
            var literalNode = factory.CreateLiteralNode(literalType, "\"hello\"");
            var reverseStringNode = factory.CreateReverseStringNode();
            var concatenateNode = factory.CreateConcatenateNode();

            var literalToReverse = new NodeConnection<TNode>(literalNode, 0, reverseStringNode, 0);
            var literalToConcatenate = new NodeConnection<TNode>(literalNode, 0, concatenateNode, 0);
            var reverseToConcatenate = new NodeConnection<TNode>(reverseStringNode, 0, concatenateNode, 1);

            return (new[] { literalNode, reverseStringNode, concatenateNode }, new[] { literalToReverse, literalToConcatenate, reverseToConcatenate }, Array.Empty<NodeConnection<TNode>>());
        }

        private static void RunExecutionGraphExamples()
        {
            // Generate a DOT graph from the an execution graph's last node
            var dotFactory = new DotExecutionGraphAlgebra();
            var dotExecutionGraph = CreateExecutionGraph<string, IDotNode, DotExecutionGraphAlgebra>(dotFactory);
            Console.WriteLine("--- DOT graph ---");
            Console.WriteLine(dotFactory.TranslateImperative(dotExecutionGraph.nodes, dotExecutionGraph.dataConnections, dotExecutionGraph.execConnections));
            Console.WriteLine();

            // Create the same execution graph, but now with elements translatable to C#
            var csharpTranslatableFactory = new CSharpTranslatableGraphAlgebra();
            var csharpTranslatableGraph = CreateExecutionGraph<Type, ICSharpTranslatableNode, CSharpTranslatableGraphAlgebra>(csharpTranslatableFactory);

            Console.WriteLine("--- C# translated graph ---");
            Console.WriteLine(csharpTranslatableFactory.TranslateImperative(csharpTranslatableGraph.nodes, csharpTranslatableGraph.dataConnections, csharpTranslatableGraph.execConnections));
        }

        private static void RunDataGraphExamples()
        {
            // Create an evaluable data graph and evaluate it
            var evaluableFactory = new EvaluableGraphAlgebra();
            (var evaluableNodes, var evaluableConnections) = CreateDataGraph<Type, IEvaluableNode, EvaluableGraphAlgebra>(evaluableFactory);

            Console.Write("--- Evaluable graph ---");
            Console.WriteLine("Last node outputs:");
            /*foreach ((var odp, var value) in evaluableNodes.Last().Evaluate()
            {
                Console.WriteLine(value);
            }*/

            // Generate a DOT graph from the same data graph's last node
            var dotFactory = new DotExecutionGraphAlgebra();
            (var dotNodes, var dotConnections) = CreateDataGraph<string, IDotNode, DotExecutionGraphAlgebra>(dotFactory);
            Console.WriteLine("--- DOT graph ---");
            Console.WriteLine(dotFactory.TranslateImperative(dotNodes, dotConnections, Array.Empty<NodeConnection<IDotNode>>()));
            Console.WriteLine();

            // Create the same data graph, but now with elements translatable to C#
            var csharpTranslatableFactory = new CSharpTranslatableGraphAlgebra();
            (var csharpNodes, var csharpConnections) = CreateDataGraph<Type, ICSharpTranslatableNode, CSharpTranslatableGraphAlgebra>(csharpTranslatableFactory);

            Console.WriteLine("--- C# translated graph ---");
            Console.WriteLine(csharpTranslatableFactory.TranslateImperative(csharpNodes, csharpConnections, Array.Empty<NodeConnection<ICSharpTranslatableNode>>()));

            // We can also combine two algebras
            var tupleFactory = new TupleDataAlgebra<string, Type, IDotNode, ICSharpTranslatableNode>(dotFactory, csharpTranslatableFactory);
            var tupleGraph = CreateDataGraph<(string, Type), (IDotNode, ICSharpTranslatableNode), TupleDataAlgebra<string, Type, IDotNode, ICSharpTranslatableNode>>(tupleFactory);
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

using ObjectAlgebraExecutionGraphs.Algebras;
using ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.DotGraph;
using ObjectAlgebraExecutionGraphs.Behaviors.EvaluableGraph;
using ObjectAlgebraExecutionGraphs.Utility;
using ObjectAlgebraExecutionGraphs.Variants;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ObjectAlgebraExecutionGraphs
{
    internal static class Program
    {
        private static (IImmutableList<TNode> nodes, IImmutableList<NodeConnection<TNode>> dataConnections) CreateDataGraph<TType, TNode, TFactory>(TFactory factory)
            where TFactory : IDataGraphAlgebra<TType, TNode>
        {
            var literalType = factory.TypeFromString(typeof(string).AssemblyQualifiedName);
            var literalNode = factory.CreateLiteralNode(literalType, "\"hello\"");
            var reverseStringNode = factory.CreateReverseStringNode();
            var reverseStringNode2 = factory.CreateReverseStringNode();

            var literalToReverse = new NodeConnection<TNode>(literalNode, 0, reverseStringNode, 0);
            var reverseToReverse2 = new NodeConnection<TNode>(reverseStringNode, 0, reverseStringNode2, 0);

            return (ImmutableArray.Create(literalNode, reverseStringNode, reverseStringNode2), ImmutableArray.Create(literalToReverse, reverseToReverse2));
        }

        private static (IImmutableList<TNode> nodes, IImmutableList<NodeConnection<TNode>> dataConnections, IImmutableList<NodeConnection<TNode>> execConnections) CreateExecutionGraph<TType, TNode, TFactory>(TFactory factory)
            where TFactory : IDataGraphAlgebra<TType, TNode>, IExecutionGraphAlgebra<TType, TNode>
        {
            var literalType = factory.TypeFromString(typeof(string).AssemblyQualifiedName);
            var literalNode = factory.CreateLiteralNode(literalType, "\"hello\"");
            var reverseStringNode = factory.CreateReverseStringNode();
            var concatenateNode = factory.CreateConcatenateNode();

            var literalToReverse = new NodeConnection<TNode>(literalNode, 0, reverseStringNode, 0);
            var literalToConcatenate = new NodeConnection<TNode>(literalNode, 0, concatenateNode, 0);
            var reverseToConcatenate = new NodeConnection<TNode>(reverseStringNode, 0, concatenateNode, 1);

            return (ImmutableArray.Create(literalNode, reverseStringNode, concatenateNode), ImmutableArray.Create(literalToReverse, literalToConcatenate, reverseToConcatenate), ImmutableArray<NodeConnection<TNode>>.Empty);
        }

        private static void RunExecutionGraphExamples()
        {
            // Generate a DOT graph from the an execution graph's last node
            var dotFactory = new DotExecutionGraphAlgebra();
            var dotExecutionGraph = CreateExecutionGraph<string, IDotNode, DotExecutionGraphAlgebra>(dotFactory);
            Console.WriteLine("--- DOT graph ---");
            Console.WriteLine(TranslateToDotGraph(dotExecutionGraph.nodes, dotExecutionGraph.dataConnections, dotExecutionGraph.execConnections));
            Console.WriteLine();

            // Create the same execution graph, but now with elements translatable to C#
            var csharpTranslatableFactory = new CSharpTranslatableGraphAlgebra();
            var csharpTranslatableGraph = CreateExecutionGraph<Type, ICSharpTranslatableNode, CSharpTranslatableGraphAlgebra>(csharpTranslatableFactory);

            Console.WriteLine("--- C# translated graph ---");
            Console.WriteLine(TranslateToCSharp(csharpTranslatableGraph.nodes, csharpTranslatableGraph.dataConnections, csharpTranslatableGraph.execConnections));
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
            Console.WriteLine(TranslateToDotGraph(dotNodes, dotConnections, ImmutableArray<NodeConnection<IDotNode>>.Empty));
            Console.WriteLine();

            // Create the same data graph, but now with elements translatable to C#
            var csharpTranslatableFactory = new CSharpTranslatableGraphAlgebra();
            (var csharpNodes, var csharpConnections) = CreateDataGraph<Type, ICSharpTranslatableNode, CSharpTranslatableGraphAlgebra>(csharpTranslatableFactory);

            Console.WriteLine("--- C# translated graph ---");
            Console.WriteLine(TranslateToCSharp(csharpNodes, csharpConnections, ImmutableArray<NodeConnection<ICSharpTranslatableNode>>.Empty));

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

        private static string TranslateToDotGraph(IImmutableList<IDotNode> nodes, IImmutableList<NodeConnection<IDotNode>> dataConnections, IImmutableList<NodeConnection<IDotNode>> execConnections)
            => $"digraph graph {{\n{string.Join("\n", dataConnections.Concat(execConnections).Distinct().Select(conn => $"{conn.FromNode.DotName} -> {conn.ToNode.DotName}"))}\n}}";

        private static IEnumerable<string> CallPureDependents(ICSharpTranslatableNode node, IImmutableList<NodeConnection<ICSharpTranslatableNode>> dataConnections)
        {
            for (int toIndex = 0; toIndex < node.Inputs.Count; toIndex++)
            {
                var conns = dataConnections.Where(conn => conn.ToNode == node && conn.ToPinIndex == toIndex).ToArray();
                if (conns.Length > 1)
                {
                    throw new Exception();
                }

                if (conns.Length == 1)
                {
                    var conn = conns[0];
                    if (conn.FromNode.IsPure)
                    {
                        foreach (var call in CallPureDependents(conn.FromNode, dataConnections))
                        {
                            yield return call;
                        }
                    }

                    yield return $"{conn.FromNode.PureFunctionName}();\n";
                }
            }
        }

        private static string TranslateToCSharp(IImmutableList<ICSharpTranslatableNode> nodes, IImmutableList<NodeConnection<ICSharpTranslatableNode>> dataConnections, IImmutableList<NodeConnection<ICSharpTranslatableNode>> execConnections)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var node in nodes)
            {
                builder.Append(node.TranslateVariables());
            }

            foreach (var pureNode in nodes.Where(node => node.IsPure))
            {
                builder.Append($"void {pureNode.PureFunctionName}()\n");
                builder.Append("{\n");
                builder.Append(pureNode.TranslatePureFunctions());
                builder.Append("}\n");
            }

            foreach (var stateNode in nodes.Where(node => !node.IsPure))
            {
                IImmutableList<string> pureCalls = CallPureDependents(stateNode, dataConnections).ToImmutableArray();

                if (!stateNode.IsPure)
                {
                    var outputConnectedLabels = new string[stateNode.ExecOutputCount];

                    var nodeExecConnections = execConnections
                        .Where(conn => conn.FromNode == stateNode)
                        .ToArray();

                    foreach (var execConn in nodeExecConnections)
                    {
                        outputConnectedLabels[execConn.FromPinIndex] = execConn.ToNode.ExecInputs[execConn.ToPinIndex];
                    }

                    builder.Append(stateNode.TranslateStates(outputConnectedLabels.ToImmutableArray(), string.Concat(pureCalls.Distinct())));
                }
            }

            return builder.ToString();
        }
    }
}

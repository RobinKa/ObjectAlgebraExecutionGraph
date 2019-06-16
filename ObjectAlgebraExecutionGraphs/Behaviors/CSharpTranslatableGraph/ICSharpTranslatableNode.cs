using System.Collections.Generic;

namespace ObjectAlgebraExecutionGraphs.Behaviors.CSharpTranslatableGraph
{
    /// <summary>
    /// Node that is translatable to C#.
    /// </summary>
    public interface ICSharpTranslatableNode
    {
        /// <summary>
        /// Translate all variables of this node to code.
        /// </summary>
        /// <returns>Code for creating the variables of this node.</returns>
        public string TranslateVariables();

        /// <summary>
        /// Translate the pure functions of this node (if any) into local functions.
        /// </summary>
        /// <returns>Code for the local functions for this node.</returns>
        public string TranslatePureFunctions();

        /// <summary>
        /// Translate this node's stateful operations (ie. node with execution pins) into code.
        /// </summary>
        /// <returns>Code for the stateful operations of this node.</returns>
        public string TranslateStates();

        /// <summary>
        /// Whether this node is pure (ie. has no execution pins of its own but is executed
        /// by being referenceed from an execution node).
        /// </summary>
        public bool IsPure { get; }

        /// <summary>
        /// Name of this node's local function if it is pure.
        /// </summary>
        public string PureFunctionName { get; }
    }
}

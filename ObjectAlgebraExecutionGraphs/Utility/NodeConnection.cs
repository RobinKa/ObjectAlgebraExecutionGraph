namespace ObjectAlgebraExecutionGraphs.Utility
{
    public struct NodeConnection<TNode>
    {
        public TNode FromNode { get; }
        public TNode ToNode { get; }
        public int FromPinIndex { get; }
        public int ToPinIndex { get; }

        public NodeConnection(TNode fromNode, int fromPinIndex, TNode toNode, int toPinIndex)
        {
            FromNode = fromNode;
            FromPinIndex = fromPinIndex;
            ToNode = toNode;
            ToPinIndex = toPinIndex;
        }
    }
}

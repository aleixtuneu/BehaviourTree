using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    private List<Node> _children;
    public Sequence(List<Node> children) => _children = children;

    public override NodeState Evaluate()
    {
        foreach (var child in _children)
        {
            var result = child.Evaluate();
            if (result != NodeState.Success)
            {
                return result;
            }               
        }
        return NodeState.Success;
    }
}

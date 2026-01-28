namespace SurveyApp.Services.Infrastructure.Commands;

/// <summary>
/// Represents a node in the command history graph.
/// Supports branching when user undoes and executes a new command.
/// </summary>
public class CommandHistoryNode
{
    /// <summary>
    /// Gets the command associated with this node.
    /// </summary>
    public IUndoableCommand Command { get; }

    /// <summary>
    /// Gets the parent node (command executed before this one).
    /// Null for the root node.
    /// </summary>
    public CommandHistoryNode? Parent { get; set; }

    /// <summary>
    /// Gets the children nodes (commands executed after this one).
    /// Multiple children indicate branching.
    /// </summary>
    public List<CommandHistoryNode> Children { get; } = new();

    /// <summary>
    /// Gets whether this node is on the current active path.
    /// </summary>
    public bool IsActive { get; set; }

    public CommandHistoryNode(IUndoableCommand command, CommandHistoryNode? parent = null)
    {
        Command = command;
        Parent = parent;
        IsActive = true;
    }

    /// <summary>
    /// Adds a child node to this node's children.
    /// </summary>
    public void AddChild(CommandHistoryNode child)
    {
        Children.Add(child);
        child.Parent = this;
    }
}

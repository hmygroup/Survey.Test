namespace SurveyApp.Services.Infrastructure.Commands;

/// <summary>
/// Manages command history using a graph structure to support branching.
/// Implements undo/redo functionality with a tree-based history.
/// </summary>
public class CommandHistoryManager
{
    private readonly ILogger<CommandHistoryManager> _logger;
    private readonly int _maxHistoryDepth;
    
    private CommandHistoryNode? _root;
    private CommandHistoryNode? _currentNode;

    /// <summary>
    /// Gets the current position in the history.
    /// </summary>
    public CommandHistoryNode? CurrentNode => _currentNode;

    /// <summary>
    /// Gets whether undo is available.
    /// </summary>
    public bool CanUndo => _currentNode != null;

    /// <summary>
    /// Gets whether redo is available.
    /// </summary>
    public bool CanRedo => _currentNode?.Children.Any(c => c.IsActive) == true;

    /// <summary>
    /// Event raised when undo/redo availability changes.
    /// </summary>
    public event EventHandler? CanUndoRedoChanged;

    public CommandHistoryManager(ILogger<CommandHistoryManager> logger, int maxHistoryDepth = 50)
    {
        _logger = logger;
        _maxHistoryDepth = maxHistoryDepth;
    }

    /// <summary>
    /// Executes a command and adds it to the history.
    /// If currently not at the end of history, creates a new branch.
    /// </summary>
    public async Task<bool> ExecuteAsync(IUndoableCommand command)
    {
        _logger.LogInformation("Executing command: {Description}", command.Description);

        try
        {
            var success = await command.ExecuteAsync();
            if (!success)
            {
                _logger.LogWarning("Command execution failed: {Description}", command.Description);
                return false;
            }

            var newNode = new CommandHistoryNode(command);

            if (_currentNode == null)
            {
                // First command - this becomes the root
                _root = newNode;
                _currentNode = newNode;
            }
            else
            {
                // Deactivate current active children (we're creating a new branch)
                foreach (var child in _currentNode.Children.Where(c => c.IsActive))
                {
                    DeactivateBranch(child);
                }

                // Add new command as child of current node
                _currentNode.AddChild(newNode);
                _currentNode = newNode;
            }

            TrimHistory();
            NotifyCanUndoRedoChanged();

            _logger.LogInformation("Command executed successfully: {Description}", command.Description);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command: {Description}", command.Description);
            return false;
        }
    }

    /// <summary>
    /// Undoes the current command and moves back in history.
    /// </summary>
    public async Task<bool> UndoAsync()
    {
        if (!CanUndo)
        {
            _logger.LogWarning("Cannot undo: no command to undo");
            return false;
        }

        _logger.LogInformation("Undoing command: {Description}", _currentNode!.Command.Description);

        try
        {
            var success = await _currentNode.Command.UndoAsync();
            if (!success)
            {
                _logger.LogWarning("Undo failed for command: {Description}", _currentNode.Command.Description);
                return false;
            }

            _currentNode = _currentNode.Parent;
            NotifyCanUndoRedoChanged();

            _logger.LogInformation("Command undone successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error undoing command: {Description}", _currentNode!.Command.Description);
            return false;
        }
    }

    /// <summary>
    /// Redoes a command that was previously undone.
    /// If multiple branches exist, redoes the most recently active one.
    /// </summary>
    public async Task<bool> RedoAsync()
    {
        if (!CanRedo)
        {
            _logger.LogWarning("Cannot redo: no command to redo");
            return false;
        }

        // Find the active child (most recent branch)
        var nextNode = _currentNode!.Children.FirstOrDefault(c => c.IsActive);
        if (nextNode == null)
        {
            _logger.LogWarning("Cannot redo: no active child found");
            return false;
        }

        _logger.LogInformation("Redoing command: {Description}", nextNode.Command.Description);

        try
        {
            var success = await nextNode.Command.RedoAsync();
            if (!success)
            {
                _logger.LogWarning("Redo failed for command: {Description}", nextNode.Command.Description);
                return false;
            }

            _currentNode = nextNode;
            NotifyCanUndoRedoChanged();

            _logger.LogInformation("Command redone successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redoing command: {Description}", nextNode.Command.Description);
            return false;
        }
    }

    /// <summary>
    /// Clears the entire command history.
    /// </summary>
    public void Clear()
    {
        _logger.LogInformation("Clearing command history");
        _root = null;
        _currentNode = null;
        NotifyCanUndoRedoChanged();
    }

    /// <summary>
    /// Gets the command history as a linear list (current active path).
    /// </summary>
    public List<IUndoableCommand> GetHistory()
    {
        var history = new List<IUndoableCommand>();
        var node = _currentNode;

        while (node != null)
        {
            history.Insert(0, node.Command);
            node = node.Parent;
        }

        return history;
    }

    /// <summary>
    /// Gets the full history graph starting from the root.
    /// </summary>
    public CommandHistoryNode? GetHistoryGraph()
    {
        return _root;
    }

    /// <summary>
    /// Deactivates a branch (marks it and all descendants as inactive).
    /// </summary>
    private void DeactivateBranch(CommandHistoryNode node)
    {
        node.IsActive = false;
        foreach (var child in node.Children)
        {
            DeactivateBranch(child);
        }
    }

    /// <summary>
    /// Trims history to the maximum depth by removing oldest commands.
    /// </summary>
    private void TrimHistory()
    {
        var depth = GetDepth(_currentNode);
        
        if (depth <= _maxHistoryDepth)
            return;

        _logger.LogDebug("Trimming history: current depth {Depth}, max depth {MaxDepth}", 
            depth, _maxHistoryDepth);

        // Find the node at max depth from current
        var node = _currentNode;
        for (int i = 0; i < _maxHistoryDepth && node != null; i++)
        {
            node = node.Parent;
        }

        // Disconnect from parent (this becomes the new root)
        if (node?.Parent != null)
        {
            node.Parent.Children.Clear();
            node.Parent = null;
            _root = node;
        }
    }

    /// <summary>
    /// Gets the depth (distance from root) of a node.
    /// </summary>
    private int GetDepth(CommandHistoryNode? node)
    {
        int depth = 0;
        while (node != null)
        {
            depth++;
            node = node.Parent;
        }
        return depth;
    }

    /// <summary>
    /// Notifies subscribers that undo/redo availability has changed.
    /// </summary>
    private void NotifyCanUndoRedoChanged()
    {
        CanUndoRedoChanged?.Invoke(this, EventArgs.Empty);
    }
}

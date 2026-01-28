namespace SurveyApp.Services.Infrastructure.Commands;

/// <summary>
/// Interface for commands that can be executed, undone, and redone.
/// Part of the Command Pattern implementation for undo/redo functionality.
/// </summary>
public interface IUndoableCommand
{
    /// <summary>
    /// Gets a unique identifier for this command.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets a description of what this command does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the timestamp when this command was executed.
    /// </summary>
    DateTime ExecutedAt { get; }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <returns>True if execution succeeded, false otherwise</returns>
    Task<bool> ExecuteAsync();

    /// <summary>
    /// Undoes the command, reverting its effects.
    /// </summary>
    /// <returns>True if undo succeeded, false otherwise</returns>
    Task<bool> UndoAsync();

    /// <summary>
    /// Redoes the command after it has been undone.
    /// </summary>
    /// <returns>True if redo succeeded, false otherwise</returns>
    Task<bool> RedoAsync();
}

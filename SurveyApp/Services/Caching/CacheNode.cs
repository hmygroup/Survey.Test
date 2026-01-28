namespace SurveyApp.Services.Caching;

/// <summary>
/// Represents a node in the cache dependency graph.
/// </summary>
public class CacheNode
{
    /// <summary>
    /// The cache key for this node.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when this node was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The timestamp when this node was last accessed.
    /// </summary>
    public DateTimeOffset LastAccessedAt { get; set; }

    /// <summary>
    /// Dependents of this node (nodes that should be invalidated when this node changes).
    /// </summary>
    public HashSet<string> Dependents { get; set; } = new();

    /// <summary>
    /// Whether this node has been invalidated.
    /// </summary>
    public bool IsInvalidated { get; set; }
}

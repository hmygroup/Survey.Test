using Microsoft.Extensions.Caching.Memory;

namespace SurveyApp.Services.Caching;

/// <summary>
/// Graph-based caching service with dependency tracking and intelligent invalidation.
/// Implements the cache dependency graph pattern from the requirements.
/// </summary>
public class GraphCacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<GraphCacheService> _logger;
    private readonly Dictionary<string, CacheNode> _dependencyGraph = new();
    private readonly object _lockObject = new();

    public GraphCacheService(IMemoryCache memoryCache, ILogger<GraphCacheService> logger)
    {
        _cache = memoryCache;
        _logger = logger;
    }

    /// <summary>
    /// Gets a value from the cache.
    /// </summary>
    public T? Get<T>(string key)
    {
        lock (_lockObject)
        {
            if (_dependencyGraph.TryGetValue(key, out var node))
            {
                node.LastAccessedAt = DateTimeOffset.UtcNow;
            }
        }

        return _cache.Get<T>(key);
    }

    /// <summary>
    /// Sets a value in the cache with optional dependencies.
    /// </summary>
    public void Set<T>(string key, T value, TimeSpan? expiration = null, params string[] dependencies)
    {
        var options = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            // Default expiration: 5 minutes
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        }

        // Register post-eviction callback
        options.RegisterPostEvictionCallback((k, v, r, s) =>
        {
            _logger.LogInformation("Cache entry evicted: {Key}, Reason: {Reason}", k, r);
            lock (_lockObject)
            {
                _dependencyGraph.Remove(k.ToString() ?? string.Empty);
            }
        });

        _cache.Set(key, value, options);

        lock (_lockObject)
        {
            // Create or update cache node
            var node = new CacheNode
            {
                Key = key,
                CreatedAt = DateTimeOffset.UtcNow,
                LastAccessedAt = DateTimeOffset.UtcNow,
                IsInvalidated = false
            };

            _dependencyGraph[key] = node;

            // Add dependencies
            foreach (var dependency in dependencies)
            {
                if (_dependencyGraph.TryGetValue(dependency, out var dependencyNode))
                {
                    dependencyNode.Dependents.Add(key);
                }
                else
                {
                    // Create placeholder for dependency
                    var newDependencyNode = new CacheNode
                    {
                        Key = dependency,
                        CreatedAt = DateTimeOffset.UtcNow,
                        LastAccessedAt = DateTimeOffset.UtcNow,
                        Dependents = new HashSet<string> { key }
                    };
                    _dependencyGraph[dependency] = newDependencyNode;
                }
            }
        }

        _logger.LogInformation("Cache entry added: {Key} with {DependencyCount} dependencies", key, dependencies.Length);
    }

    /// <summary>
    /// Tries to get a value from the cache.
    /// </summary>
    public bool TryGetValue<T>(string key, out T? value)
    {
        lock (_lockObject)
        {
            if (_dependencyGraph.TryGetValue(key, out var node))
            {
                node.LastAccessedAt = DateTimeOffset.UtcNow;
            }
        }

        return _cache.TryGetValue(key, out value);
    }

    /// <summary>
    /// Invalidates a cache entry and all its descendants in the dependency graph.
    /// </summary>
    public void InvalidateNode(string key)
    {
        lock (_lockObject)
        {
            var descendants = GetAllDescendants(key);
            
            foreach (var descendant in descendants)
            {
                _cache.Remove(descendant);
                if (_dependencyGraph.TryGetValue(descendant, out var node))
                {
                    node.IsInvalidated = true;
                }
                _logger.LogInformation("Invalidated cache entry: {Key}", descendant);
            }

            _cache.Remove(key);
            if (_dependencyGraph.TryGetValue(key, out var mainNode))
            {
                mainNode.IsInvalidated = true;
            }
            
            _logger.LogInformation("Invalidated {Count} cache entries starting from {Key}", descendants.Count + 1, key);
        }
    }

    /// <summary>
    /// Gets all descendants of a node in the dependency graph.
    /// </summary>
    private HashSet<string> GetAllDescendants(string key)
    {
        var descendants = new HashSet<string>();
        var toVisit = new Queue<string>();
        
        if (_dependencyGraph.TryGetValue(key, out var node))
        {
            foreach (var dependent in node.Dependents)
            {
                toVisit.Enqueue(dependent);
            }

            while (toVisit.Count > 0)
            {
                var current = toVisit.Dequeue();
                if (descendants.Add(current))
                {
                    if (_dependencyGraph.TryGetValue(current, out var currentNode))
                    {
                        foreach (var dependent in currentNode.Dependents)
                        {
                            toVisit.Enqueue(dependent);
                        }
                    }
                }
            }
        }

        return descendants;
    }

    /// <summary>
    /// Removes a cache entry.
    /// </summary>
    public void Remove(string key)
    {
        _cache.Remove(key);
        lock (_lockObject)
        {
            _dependencyGraph.Remove(key);
        }
        _logger.LogInformation("Removed cache entry: {Key}", key);
    }

    /// <summary>
    /// Clears all cache entries.
    /// </summary>
    public void Clear()
    {
        lock (_lockObject)
        {
            // Note: IMemoryCache doesn't have a Clear method, so we track keys
            var keys = _dependencyGraph.Keys.ToList();
            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
            _dependencyGraph.Clear();
        }
        _logger.LogInformation("Cleared all cache entries");
    }

    /// <summary>
    /// Gets cache statistics.
    /// </summary>
    public CacheStatistics GetStatistics()
    {
        lock (_lockObject)
        {
            return new CacheStatistics
            {
                TotalEntries = _dependencyGraph.Count,
                InvalidatedEntries = _dependencyGraph.Values.Count(n => n.IsInvalidated),
                AverageAccessTime = _dependencyGraph.Values.Any() 
                    ? _dependencyGraph.Values.Average(n => (DateTimeOffset.UtcNow - n.LastAccessedAt).TotalSeconds)
                    : 0
            };
        }
    }
}

/// <summary>
/// Cache statistics information.
/// </summary>
public class CacheStatistics
{
    public int TotalEntries { get; set; }
    public int InvalidatedEntries { get; set; }
    public double AverageAccessTime { get; set; }
}

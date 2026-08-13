using Microsoft.CodeAnalysis;

namespace SEngine.Analyzers;

public static class DiagnosticDescriptors
{
    private const string Category = "Performance";

    public static readonly DiagnosticDescriptor LinqInHotPath = new(
        id: "ENG001",
        title: "LINQ usage in [HotPath] method",
        messageFormat: "LINQ method '{0}' allocates and must not be used inside a [HotPath] method",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ClosureInHotPath = new(
        id: "ENG002",
        title: "Allocating closure in [HotPath] method",
        messageFormat: "Lambda/local function captures '{0}', which allocates a closure; not allowed in [HotPath] methods",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObjectAllocationInHotPath = new(
        id: "ENG003",
        title: "Heap allocation in [HotPath] method",
        messageFormat: "'{0}' allocates on the managed heap and is not allowed in [HotPath] methods",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AsyncInHotPath = new(
        id: "ENG004",
        title: "async/await in [HotPath] method",
        messageFormat: "async methods allocate a state machine and must not be marked [HotPath]",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor BoxingInHotPath = new(
        id: "ENG005",
        title: "Boxing conversion in [HotPath] method",
        messageFormat: "Implicit boxing of '{0}' allocates on the heap and is not allowed in [HotPath] methods",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor StringInterpolationInHotPath = new(
        id: "ENG006",
        title: "String interpolation/format in [HotPath] method",
        messageFormat: "String interpolation/formatting allocates and is not allowed in [HotPath] methods",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
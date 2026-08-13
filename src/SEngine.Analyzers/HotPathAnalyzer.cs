using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SEngine.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HotPathAnalyzer : DiagnosticAnalyzer
{
    private const string HotPathAttributeName = "HotPathAttribute";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        DiagnosticDescriptors.LinqInHotPath,
            DiagnosticDescriptors.ClosureInHotPath,
            DiagnosticDescriptors.ObjectAllocationInHotPath,
            DiagnosticDescriptors.AsyncInHotPath,
            DiagnosticDescriptors.BoxingInHotPath,
            DiagnosticDescriptors.StringInterpolationInHotPath
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }
    
    private static bool HasHotPathAttribute(ISymbol symbol) =>
        symbol.GetAttributes().Any(a => a.AttributeClass?.Name == HotPathAttributeName);

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDecl = (MethodDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(methodDecl);
        if (symbol is null || !HasHotPathAttribute(symbol))
            return;

        if (methodDecl.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.AsyncInHotPath, methodDecl.Identifier.GetLocation()));
        }
        
        foreach (var node in methodDecl.DescendantNodes())
        {
            switch (node)
            {
                case InvocationExpressionSyntax invocation:
                    CheckInvocation(context, invocation);
                    break;

                case ObjectCreationExpressionSyntax or ArrayCreationExpressionSyntax
                    or ImplicitArrayCreationExpressionSyntax:
                    CheckAllocation(context, node);
                    break;

                case LambdaExpressionSyntax lambda:
                    CheckClosure(context, lambda);
                    break;

                case InterpolatedStringExpressionSyntax interpolated:
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.StringInterpolationInHotPath, interpolated.GetLocation()));
                    break;
            }
        }
    }

    private static void CheckInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
    {
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
        if (methodSymbol is null)
            return;

        var containingNamespace = methodSymbol.ContainingNamespace?.ToDisplayString();
        if (containingNamespace == "System.Linq")
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.LinqInHotPath,
                invocation.GetLocation(),
                methodSymbol.Name));
        }
    }

    private static void CheckAllocation(SyntaxNodeAnalysisContext context, SyntaxNode node)
    {
        var typeInfo = context.SemanticModel.GetTypeInfo(node);
        var type = typeInfo.Type;

        if (type is null || type.IsValueType)
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.ObjectAllocationInHotPath,
            node.GetLocation(),
            type.ToDisplayString()));
    }
    
    private static void CheckClosure(SyntaxNodeAnalysisContext context, LambdaExpressionSyntax lambda)
    {
        var dataFlow = ModelExtensions.AnalyzeDataFlow(context.SemanticModel, lambda);
        if (dataFlow is { Succeeded: true, CapturedInside.Length: > 0 })
        {
            var captured = string.Join(", ", dataFlow.CapturedInside.Select(s => s.Name));
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.ClosureInHotPath, lambda.GetLocation(), captured));
        }
    }
}
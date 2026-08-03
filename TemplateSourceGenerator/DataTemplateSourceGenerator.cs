
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TemplateSourceGenerator;

[Generator]
public class DataTemplateSourceGenerator : IIncrementalGenerator
{
    private IncrementalGeneratorInitializationContext _initContext;
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        _initContext = context;
        var classDeclarations = _initContext
            .SyntaxProvider
            .CreateSyntaxProvider((node, _) =>

                node is TypeDeclarationSyntax type && type.AttributeLists.Any(
                    l => l.Attributes.Any(
                        a =>
                        {
                            string? name = a.Name switch
                            {
                                SimpleNameSyntax sns => sns.Identifier.Text,
                                QualifiedNameSyntax qns => qns.Right.Identifier.Text,
                                _ => null
                            };

                            return name == "GeneratesDataTemplate" || name == nameof(GeneratesDataTemplateAttribute);
                        }
                    )
                ),
                (syntax, _) => syntax.Node
            )
            .Collect();

        var compilationAndClasses = _initContext.CompilationProvider.Combine(classDeclarations);
        _initContext.RegisterSourceOutput(compilationAndClasses, GenerateSourceCode);
    }

    private void GenerateSourceCode(SourceProductionContext context, (Compilation Left, ImmutableArray<SyntaxNode> Right) data)
    {
        var (compilation, classes) = data;
        foreach (var classDeclaration in classes)
        {
            var model = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol) continue;

            var sourceCode = GenerateSourceCode(compilation, classSymbol);
            context.AddSource($"{classSymbol.Name}DataTemplate.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private string GenerateSourceCode(Compilation compilation, INamedTypeSymbol classSymbol)
    {
        string classType = classSymbol.ContainingNamespace is null ? 
            classSymbol.Name : $"{classSymbol.ContainingNamespace}.{classSymbol.Name}";

        string properties = string.Empty;
        var members = classSymbol
            .GetMembers()
            .OfType<IPropertySymbol>();

        bool panelMethodAppended = false;
        foreach (var property in members)
        {
            string name = property.Name;
            string? label = property.Name;
            if (property.Type is not INamedTypeSymbol namedType) continue;

            bool isEligible = false;
            string? tooltip = string.Empty;
            foreach (var attribute in property.GetAttributes())
            {
                if (attribute.AttributeClass?.Name == nameof(DataTemplateFieldAttribute))
                {
                    isEligible = true;
                    foreach (var argument in attribute.NamedArguments)
                    {
                        switch (argument.Key)
                        {
                            case nameof(DataTemplateFieldAttribute.Name):
                                label = argument.Value.Value as string;
                                break;
                            case nameof(DataTemplateFieldAttribute.Tooltip):
                                tooltip = argument.Value.Value as string;
                                break;
                        }
                    }
                }
            }

            if (!isEligible) continue;
            if (!panelMethodAppended)
            {
                panelMethodAppended = true;
                properties += $$"""

                void CreatePanel(string? text, string? tooltip, Control control)
                {
                    TextBlock label = new TextBlock { VerticalAlignment = VerticalAlignment.Center, Text = text };
                    ToolTip.SetTip(label, tooltip);
                
                    panel.Children.Add(label);
                    panel.Children.Add(control);
                }
                """;
            }

            properties += "\n\n";

            IEnumerable<INamedTypeSymbol> GetDeriven(INamedTypeSymbol baseTypeSymbol)
            {
                var deriven1 = GetDerivenTypes(compilation.GlobalNamespace, baseTypeSymbol)
                    .Where(d => !GetDerivenTypes(compilation.GlobalNamespace, d).Any());

                var topLevelNamespace = baseTypeSymbol.ContainingNamespace.ConstituentNamespaces.Last();
                var deriven2 = GetDerivenTypes(topLevelNamespace, baseTypeSymbol)
                    .Where(d => !GetDerivenTypes(topLevelNamespace, d).Any());

                return deriven1.Concat(deriven2).OrderBy(p => p.Name);
            }

            var typeName = $"{namedType.ContainingNamespace}.{namedType.Name}";
            switch (namedType.Name)
            {
                case "UInt32" or "Int32":
                    string minimum = namedType.Name == "UInt32" ? "Minimum = 0," : "";
                    properties += $$"""
                            NumericUpDown {{name}}Field = new NumericUpDown
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                {{minimum}}
                                Increment = 1,
                                [!NumericUpDown.ValueProperty] = new Binding
                                {
                                    Source = param,
                                    Path = nameof({{classType}}.{{name}}),
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                }
                            };
                            """;
                    break;

                case "Boolean":
                    properties += $$"""
                            CheckBox {{name}}Field = new CheckBox
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                [!CheckBox.IsCheckedProperty] = new Binding
                                {
                                    Source = param,
                                    Path = nameof({{classType}}.{{name}}),
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                }
                            };
                            """;
                    break;

                case "String":
                    properties += $$"""
                            AutoCompleteBox {{name}}Field = new AutoCompleteBox
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                [!AutoCompleteBox.TextProperty] = new Binding
                                {
                                    Source = param,
                                    Path = nameof({{classType}}.{{name}}),
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                }
                            };
                            """;
                    break;

                case object when namedType.EnumUnderlyingType is not null:
                    var options = namedType.MemberNames.Select(m => $"{typeName}.{m}");
                    properties += $$"""
                            ComboBox {{name}}Field = new ComboBox
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                ItemsSource = new {{typeName}}[] { {{string.Join(", ", options)}} },
                                [!ComboBox.SelectedItemProperty] = new Binding
                                {
                                    Source = param,
                                    Path = nameof({{classType}}.{{name}}),
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                }
                            };
                            """;
                    break;

                case object when namedType.Name.Contains("ObservableCollection"):
                    if (namedType.TypeArguments[0] is not INamedTypeSymbol arg) break;
                    if (arg.Name == "UInt32")
                    {
                        properties += $$"""
                            TextBox {{name}}Field = new TextBox
                            {
                                Text = string.Join("; ", (param as {{classType}}).{{name}})
                            };
                            
                            {{name}}Field.TextChanged += (sender, i) =>
                            {
                                List<uint> list = new List<uint>();
                                foreach (var f in ValuesField.Text.Split(';'))
                                {
                                    if (UInt32.TryParse(f, out uint result))
                                    {
                                        list.Add(result);
                                    }
                                }

                                (param as {{classType}}).{{name}} = [..list];
                            };
                            """;
                    }
                    else
                    {
                        string argType = arg.ContainingNamespace is null ?
                            arg.Name : $"{arg.ContainingNamespace}.{arg.Name}";

                        string s = string.Join(", ",
                            GetDeriven(arg)
                                .Concat([arg])
                                .Select(n => $"typeof({n.ContainingNamespace}.{n.Name})"));

                        properties += $$"""
                            StackPanel addAndRemove = new StackPanel { Orientation = Orientation.Horizontal };
                            AutoCompleteBox box = new AutoCompleteBox { ItemsSource = new Type[] {{{s}}}, FilterMode = AutoCompleteFilterMode.ContainsOrdinal };
                            
                            box.SelectionChanged += (sender, e) =>
                            {
                                if (e.AddedItems.Count < 1) return;
                                object? instance = Activator.CreateInstance((Type)e.AddedItems[0]);
                                (param as {{classType}}).{{name}}.Add(({{argType}})instance);
                                (param as {{classType}}).CollectionChanged(({{argType}})instance);
                            };

                            ListBox {{name}}Field = new ListBox
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                [!ListBox.ItemsSourceProperty] = new Binding
                                {
                                    Source = param,
                                    Path = nameof({{classType}}.{{name}}),
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                }
                            };
                            
                            Button remove = new Button { Content = "Remove" };
                            remove.Click += (s, e) =>
                            {
                                if ({{name}}Field.SelectedItem is {{argType}} o)
                                {
                                    (param as {{classType}}).{{name}}.Remove(o);
                                    (param as {{classType}}).CollectionChanged();
                                }
                            };
                            
                            addAndRemove.Children.Add(box);
                            addAndRemove.Children.Add(remove);
                            panel.Children.Add(addAndRemove);
                            """;
                    }

                    break;

                default:
                    string ss = string.Join(", ", GetDeriven(namedType).Select(n => $"new {n.ContainingNamespace}.{n.Name}()"));
                    if (namedType.NullableAnnotation == NullableAnnotation.Annotated)
                    {
                        ss = $"null, {ss}";
                    }

                    properties += $$"""
                            StackPanel {{name}}Field = new StackPanel {
                                Orientation = Orientation.Horizontal,
                                Spacing = 10
                            };

                            ComboBox {{name}}ItemsComboBox =  new ComboBox
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                ItemsSource = new {{typeName}}[] {{{ss}}},
                                [!ComboBox.SelectedItemProperty] = new Binding
                                {
                                    Source = param,
                                    Path = nameof({{classType}}.{{name}}),
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                }
                            };

                            {{name}}ItemsComboBox.ItemTemplate = new FuncDataTemplate<object>((o, c) =>
                            {
                                return new TextBlock { Text = o?.GetType().Name };
                            });

                            {{name}}Field.Children.Add({{name}}ItemsComboBox);
                            {{name}}Field.Children.Add(
                                new ContentControl
                                {
                                    [!ContentControl.ContentProperty] = new Binding
                                    {
                                        Source = param,
                                        Path = nameof({{classType}}.{{name}}),
                                        Mode = BindingMode.TwoWay,
                                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                                    }
                                }
                            );
                            """;
                    break;
            }

            properties += $"\n\nCreatePanel(\"{label}\", \"{tooltip}\", {name}Field);";
        }

        properties += '\n';

        return
            $$"""
            using Avalonia.Controls;
            using Avalonia.Controls.Templates;
            using Avalonia.Data;
            using Avalonia.Layout;
            using Avalonia.Media;
            
            namespace EngineLib.DataTemplates;

            public class {{classSymbol.Name}}DataTemplate : IDataTemplate
            {
                public Control Build(object? param)
                {
                    StackPanel panel = new StackPanel
                    {
                        Orientation = Orientation.Vertical
                    };
                    
                    if (param is not object) return panel;

                    panel.Children.Add
                    (
                        new TextBlock
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = param.GetType().Name,
                            FontWeight = FontWeight.Bold
                        }
                    );
                    {{properties}}
                    return panel;
                }
                
                public bool Match(object data)
                {
                    return data is {{classType}};
                }
            }
            """;
    }

    private IEnumerable<INamedTypeSymbol> GetDerivenTypes(INamespaceSymbol topNamespace, INamedTypeSymbol baseType)
    {
        foreach (INamedTypeSymbol type in topNamespace.GetTypeMembers())
        {
            INamedTypeSymbol? nts = IsDerivenFrom(type, baseType);
            if (nts is object) yield return nts;
        }

        foreach (var nested in topNamespace.GetNamespaceMembers())
        {
            foreach (INamedTypeSymbol type in nested.GetTypeMembers())
            {
                INamedTypeSymbol? nts = IsDerivenFrom(type, baseType);
                if (nts is object) yield return nts;
            }
        }
    }

    private INamedTypeSymbol? IsDerivenFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        INamedTypeSymbol ancestor = type;
        while (ancestor.BaseType is not null)
        {
            ancestor = ancestor.BaseType;
            if (SymbolEqualityComparer.Default.Equals(ancestor, baseType)) return type;
        }

        return null;
    }
}

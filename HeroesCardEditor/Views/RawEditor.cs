
using Avalonia.Controls;
using TextMateSharp.Grammars;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;

namespace HeroesCardEditor
{
    public partial class RawEditorView : Window
    {
        public RawEditorView()
        {
            InitializeComponent();

            var textEditor = this.FindControl<TextEditor>("RawEditor");
            var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
            var textMateInstallation = textEditor.InstallTextMate(registryOptions);
            textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".json").Id));
        }
    }
}
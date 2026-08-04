
using Avalonia.Controls;
using Avalonia.Interactivity;
using HeroesCardEditor.ViewModels;

namespace HeroesCardEditor
{
    public partial class EditorView : Window
    {
        public EditorView()
        {
            InitializeComponent();
            AddHandler(GotFocusEvent, OnFocusChanged);
            AddHandler(LostFocusEvent, OnFocusChanged);
        }

        private void OnFocusChanged(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not EditorViewModel vm) return;

            var focused = FocusManager?.GetFocusedElement();
            var canUseKeybindings = focused is not (TextBox or AutoCompleteBox or NumericUpDown);

            vm.CanUseKeybindings = canUseKeybindings;
            vm.SelectedFile?.CanUseKeybindings = canUseKeybindings;
            vm.SelectedFile?.SelectedEntry?.CanUseKeybindings = canUseKeybindings;
        }
    }
}
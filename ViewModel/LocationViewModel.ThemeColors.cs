using ActiBoard.Commands;
using System.Windows.Input;

namespace ActiBoard.ViewModel;
public sealed record ThemeColorOption(string Name, string Color);
public partial class LocationViewModel
{
    private string _themeEditorBackgroundColor = "";
    public string ThemeEditorBackgroundColor
    {
        get => _themeEditorBackgroundColor;
        set { _themeEditorBackgroundColor = value; OnPropertyChanged(nameof(ThemeEditorBackgroundColor)); OnPropertyChanged(nameof(ThemeEditorBackgroundPreview)); }
    }
    public string ThemeEditorBackgroundPreview => string.IsNullOrWhiteSpace(ThemeEditorBackgroundColor) ? "#F3F3F3" : ThemeEditorBackgroundColor;
    public ThemeColorOption[] ThemeColorOptions { get; } =
    [
        new("Hvid", "#FFFFFF"), new("Creme", "#F8F6F0"), new("Lysegrå", "#E5E7EB"), new("Mørkegrå", "#30343B"),
        new("Lyseblå", "#DCEEFF"), new("Blå", "#24558C"), new("Mørkeblå", "#162D50"), new("Turkis", "#B8E8E0"),
        new("Lysegrøn", "#E1F0DA"), new("Grøn", "#438347"), new("Julegrøn", "#245C43"), new("Gul", "#FFE08A"),
        new("Orange", "#F4A65D"), new("Rød", "#A63139"), new("Rosa", "#F3D5DF"), new("Lilla", "#40304F")
    ];
    public ICommand SelectThemeColorCommand => new RelayCommand(parameter =>
    {
        if (IsUnlocked && parameter is ThemeColorOption option) ThemeEditorBackgroundColor = option.Color;
    });
    public ICommand ResetThemeColorCommand => new RelayCommand(_ =>
    {
        if (IsUnlocked) ThemeEditorBackgroundColor = "";
    });
}

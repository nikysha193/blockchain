namespace ClientCSharp;

// Хранит общие настройки Client.
// Эти значения используют Login, Register и Forgot Password.
public class AppState
{
    // Выбранный язык.
    // По умолчанию English.
    public string Language { get; set; } = "en";

    // false = Light Mode
    // true = Dark Mode
    public bool DarkTheme { get; set; } = false;
}
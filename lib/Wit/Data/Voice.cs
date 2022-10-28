namespace Wit.Data
{
    public record Voice(
        string Name,
        string Locale,
        string Gender,
        string[] Styles
    );
}
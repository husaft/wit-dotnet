namespace Wit.Data
{
    public record SpeechToken(
        int End,
        int Start,
        string Token
    );
}
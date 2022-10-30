namespace Wit.Data
{
    public record Speech(
        bool? IsFinal,
        FoundSpeech speech,
        string Text
    );
}
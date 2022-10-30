namespace Wit.Data
{
    public record FoundSpeech(
        double Confidence,
        SpeechToken[] Tokens
    );
}
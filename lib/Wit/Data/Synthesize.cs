namespace Wit.Data
{
    public record Synthesize(
        string Q,
        string Voice,
        string Style,
        int Speed,
        int Pitch,
        int Gain
    );
}
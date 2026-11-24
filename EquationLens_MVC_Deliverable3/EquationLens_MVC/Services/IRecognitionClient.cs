namespace EquationLens.Services;

public record RecognitionResult(string Latex, bool IsDemo, string Note);

/// <summary>
/// Turns a photo of an equation into text. This deliverable ships a demo implementation.
/// A later version can register a class that calls a real recognizer instead (see Program.cs).
/// </summary>
public interface IRecognitionClient
{
    Task<RecognitionResult> RecognizeAsync(Stream image, string fileName, CancellationToken cancellationToken = default);
}

public class DemoRecognitionClient : IRecognitionClient
{
    public Task<RecognitionResult> RecognizeAsync(Stream image, string fileName, CancellationToken cancellationToken = default)
    {
        var name = Path.GetFileNameWithoutExtension(fileName).ToLowerInvariant();
        var latex =
            name.Contains("linear") ? "3x + 7 = 22" :
            name.Contains("factor") ? "x^2 - 9 = 0" :
            name.Contains("square") ? "x^2 + 4x + 4 = 0" :
            "2x^2 - 5x + 3 = 0";

        return Task.FromResult(new RecognitionResult(latex, true,
            "Demo recognizer: this is a sample result, not read from your image. Check it and edit if needed."));
    }
}

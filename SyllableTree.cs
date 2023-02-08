namespace Sbox_TTS_POC;

public sealed class SyllableTree
{
    private SyllableNode Root { get; } = new(null);

    public void Add(string word, string path)
    {
        word = word.Trim();

        var wordView = word.AsSpan();
        Root.Add(ref wordView, path);
    }

    public string[] TTS(string input)
    {
        var words = input.Split(' ');
        var result = new List<string>();
        
        foreach (var word in words)
        {
            var view = word.AsSpan();
            while (!view.IsEmpty)
            {
                Console.WriteLine($"DEBUG TTS: Word {word} view {view}");
                var path = Root.FindBestFit(ref view);
                if (path is null)
                    throw new ArgumentException($"Did not manage to find a syllable for a word \"{word}\"");
                result.Add(path);
            }
        }

        return result.ToArray();
    }

    public void Print()
    {
        Root.Print();
    }
}
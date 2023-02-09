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

    public IEnumerable<IEnumerable<string>> TextToSpeech(string input)
    {
        var words = input.Split(' ');
        var result = new List<IEnumerable<string>>();
        
        foreach (var word in words)
        {
            var view = ($"{word}_").AsSpan(); // _ is a marker of the word's end
            var wordSyllables = new List<string>();
            while (!view.IsEmpty)
            {
                Console.WriteLine($"DEBUG TTS: Word {word} view {view}");
                var path = Root.FindBestFit(ref view);
                if (path is null)
                    throw new ArgumentException($"Did not manage to find a syllable for a word \"{word}\"");
                wordSyllables.Add(path);
            }
            result.Add(wordSyllables);
        }

        return result;
    }

    public void Print()
    {
        Console.WriteLine(Root.Print());
    }
}
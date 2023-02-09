namespace Sbox_TTS_POC;

public sealed class SyllableNode
{
    private Dictionary<char, SyllableNode> Branches { get; } = new();
    private string? Path { get; set; }

    public SyllableNode(string? path, params (char character, SyllableNode branch)[] nextSyllables)
    {
        this.Path = path;
        
        foreach(var syllable in nextSyllables)
            Branches.Add(syllable.character, syllable.branch);
    }

    public void Add(ref ReadOnlySpan<char> wordView, string path)
    {
        if (wordView.IsEmpty) // This is the sentence we need
        {
            Path = path;
            return;
        }

        var next = wordView[0]; // Consume a character
        wordView = wordView[1..];

        SyllableNode nextNode;
        if (!Branches.ContainsKey(next))
        {
            if (wordView.IsEmpty)
            {
                Console.WriteLine($"DEBUG: '{wordView}' -> new '{next}'");
                Branches.Add(next, new SyllableNode(path));
                return;
            }
            
            nextNode = new SyllableNode(null);
            Branches.Add(next, nextNode);
        }
        else
        {
            nextNode = Branches[next];
        }

        Console.WriteLine($"DEBUG: '{wordView}' -> '{next}'");
        nextNode.Add(ref wordView, path);
    }
    
    public string? FindBestFit(ref ReadOnlySpan<char> wordView)
    {
        Console.WriteLine($"DEBUG: working with '{wordView}'");

        if (wordView.IsEmpty)
        {
            Console.WriteLine($"DEBUG: this syllable fits! ({Path})");
            return Path;
        }

        var next = wordView[0]; // Peek at a character, implying we have no spaces because we've got a word

        if (!Branches.ContainsKey(next))
        {
            Console.WriteLine($"DEBUG: no more this syllable fits! ({Path})");
            return Path;
        }
        /* If there are no more branches, then this is the best choice
         *
         * For example:
         * Tree:
         *          Root
         *          /  \
         * a.wav - A    C - c.wav
         *        /
         *       D - ad.wav
         * 
         * String: adac
         * We have AD so we'd play `ad.wav`, but we don't have AC so we have to play `a.wav` and *then* `c.wav`
         */
        
        Console.WriteLine($"DEBUG: Passing the torch to the next character...");
        var tempWordView = wordView[1..]; // Consume the character because we've found a plan B in case we won't find a longer word
        var result = Branches[next].FindBestFit(ref tempWordView);
        if (result is null)
            return Path;
        
        wordView = tempWordView;
        return result;

    }

    public string Print(int tabLevel = 0)
    {
        var tabs = new string('\t', tabLevel);
        var s = Path + Environment.NewLine;
        tabLevel++;
        foreach (var branch in Branches)
        {
            s += $"{tabs}{branch.Key} = {branch.Value.Print(tabLevel)}{Environment.NewLine}";
        }

        return s;
    }
}
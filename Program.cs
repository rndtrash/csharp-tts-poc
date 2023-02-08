using SDL2.NET;
using SDL2.NET.SDLMixer;
using Sbox_TTS_POC;

#region Boilerplate
var app = SDLAppBuilder.CreateDefaultInstance();
app.InitializeAudio();
AudioMixer.InitAudioMixer(MixerInitFlags.MP3);
AudioMixer.OpenAudioMixer();

var Sounds = new Dictionary<string, AudioChunk>();

AudioChunk LoadSound(string path)
{
    if (Sounds.ContainsKey(path))
        return Sounds[path];
    
    var ac = new AudioChunk(path);
    Sounds.Add(path, ac);
    return ac;
}
#endregion

SyllableTree tree = new();
foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.wav"))
    tree.Add(Path.GetFileNameWithoutExtension(file), file);

tree.Print();

Console.WriteLine("Enter the line to say or enter a word 'exit' without the quote marks to exit");

while (Console.ReadLine() is { } input && input != "exit")
{
    var speech = tree.TTS(input);

    foreach (var syllable in speech)
    {
        Console.WriteLine($"Playing \"{syllable}\"...");
        var audioChunk = LoadSound(syllable);
        audioChunk.Play();
        SDLApplication.Delay(TimeSpan.FromSeconds(1)); // TODO: get length of a chunk
    }
}

#region Destroy SDL stuff
foreach (var sound in Sounds)
    sound.Value.Dispose();
Sounds.Clear();

app.Quit(SDLSubSystem.Audio);
AudioMixer.Quit();
#endregion
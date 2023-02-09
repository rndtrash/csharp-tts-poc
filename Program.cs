using NAudio.Wave;
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
tree.Add("_", "");
foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.wav"))
    tree.Add(Path.GetFileNameWithoutExtension(file).ToLowerInvariant(), file);

tree.Print();

Console.WriteLine("Enter the line to say or enter a word 'exit' without the quote marks to exit");

while (Console.ReadLine() is { } input && input != "exit")
{
    var speech = tree.TextToSpeech(input);

    foreach (var word in speech)
    {
        foreach (var syllable in word)
        {
            if (syllable == "")
                continue;
            
            Console.WriteLine($"Playing \"{syllable}\"...");
            var audioChunk = LoadSound(syllable);
            audioChunk.Play();
            
            // TODO: perhaps don't use two sound libraries???
            using var wf = new WaveFileReader(syllable);
            SDLApplication.Delay(wf.TotalTime);
        }

        SDLApplication.Delay(TimeSpan.FromSeconds(0.2));
    }
}

#region Destroy SDL stuff
foreach (var sound in Sounds)
    sound.Value.Dispose();
Sounds.Clear();

app.Quit(SDLSubSystem.Audio);
AudioMixer.Quit();
#endregion
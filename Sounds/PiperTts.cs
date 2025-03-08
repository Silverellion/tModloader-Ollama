using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ID;

namespace OllamaPlayer.Sounds;

public class PiperTts
{
    public static async Task TtsHandler(String input)
    {
        if(Main.netMode == NetmodeID.SinglePlayer)
            await PiperTts.SpeakAsync(input);
        else
            await Task.Delay(1000); //TODO actually implementing a tts method that works with multiplayer
    }
    private static async Task SpeakAsync(string input)
    { 
        string piperDir = "C:\\Users\\Impasta\\Documents\\Mein Scheisse\\Mein Works\\Mein Libs\\Piper";
        string piperExe = piperDir + "\\piper.exe";
        string modelFile = "cy_GB-gwryw_gogleddol-medium.onnx";
        string configFile = "cy_GB-gwryw_gogleddol-medium.json";
        
        string outputFile = Path.Combine(Path.GetTempPath(), "PiperTts.ogg");  

        string command = $"echo {input} | \"{piperExe}\" -m \"{modelFile}\" -c \"{configFile}\" -f \"{outputFile}\"";
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = piperDir
            }
        };

        process.Start();
        await process.WaitForExitAsync();
        
        if (File.Exists(outputFile))
        {
            try
            {
                byte[] soundBytes = File.ReadAllBytes(outputFile);
                SoundEffect soundEffect = SoundEffect.FromStream(new MemoryStream(soundBytes));
                SoundEffectInstance instance = soundEffect.CreateInstance();
        
                instance.Play();
                while (instance.State == SoundState.Playing)
                    await Task.Delay(100); 
            }
            catch (Exception ex)
            {
                StringUtility.DebugMessage($"Piper failed: {ex.Message}");
            }
        }
        else
            StringUtility.DebugMessage("Piper failed: Output file not found.");
    }
}
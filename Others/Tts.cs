using System;
using System.Threading;
using Terraria;

namespace OllamaPlayer.Others;

public class Tts
{
    private static Thread _speechThread;

    public static void Speak(string input)
    {
        
        if (_speechThread == null || !_speechThread.IsAlive)
        {
            _speechThread = new Thread(() => SpeakInBackground(input));
            _speechThread.IsBackground = true;
            _speechThread.Start();
        }
    }

    private static void SpeakInBackground(string input)
    {
        try
        {
            if(!OperatingSystem.IsWindows())
                return;
            Type sapi = Type.GetTypeFromProgID("SAPI.SpVoice");
            if (sapi == null)
                return;

            dynamic voice = Activator.CreateInstance(sapi);
            if(voice == null)
                return;
            voice.Rate = 3;
            voice.Speak(input);
        }
        catch (Exception ex)
        {
            Main.NewText(ex.Message);
        }
    }
}
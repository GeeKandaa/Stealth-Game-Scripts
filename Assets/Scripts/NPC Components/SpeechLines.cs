using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class SpeechLines
{
    public List<SpeechLine> speechLines;
}
[Serializable]
public class SpeechLine
{
    public string id;
    public List<SpeechClip> clips;
}
[Serializable]
public class SpeechClip
{
    public string Subtitle;
    public string MaleRec;
    public string FemaleRec;
}

//{
//    "id": "Chasing",
//    "clips": [
//      {
//        "Subtitle": "Hey! You! Stop!",
//        "MaleRec": "./Some/Path/To/audio.wav",
//        "FemaleRec": "./Some/Path/To/audio.wav"
//      },
//      {
//        "Subtitle": "You'll never get away from me!'",
//        "MaleRec": "./Some/Path/To/audio.wav",
//        "FemaleRec": "./Some/Path/To/audio.wav"
//      }
//    ]
//  },
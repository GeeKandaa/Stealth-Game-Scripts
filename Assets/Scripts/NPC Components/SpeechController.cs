using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using TextAsset = UnityEngine.TextAsset;

public class SpeechController : MonoBehaviour
{
    public enum speechStatus
    {
        Default,
        Patrol,
        Distracted,
        Chasing
    }
    [SerializeField]
    public LayerMask f_ignoreLayer;
    [SerializeField]
    private Camera PlayerCam;
    private RangeCollider VocalRange;
    private IEnumerator VoiceBox;
    [SerializeField]
    private TextAsset myDialogConf;
    // Character dialog script
    private SpeechLines CharacterLines;
    // Current lines
    private Queue<SpeechClip> speechClips;
    // Next lines
    private Queue<SpeechClip> newSpeechClips;
    // Voice
    private bool gender;
    // Subtitle Display
    private TextMeshPro TextComponent;
    // Subtitle visuals
    private CanvasGroup alphaControl;
    private RectTransform sizeControl;
    private Material fontControl;
    // Controller
    private speechStatus status;
    public speechStatus Status
    {
        get
        {
            return status;
        }
        set
        {
            switch (value)
            {
                case speechStatus.Default:
                    if (status != speechStatus.Default)
                    {
                        status = value;
                        PlaySpeechFromName("default");
                    }
                    break;
                case speechStatus.Patrol:
                    if (status != speechStatus.Patrol)
                    {
                        status = value;
                        PlaySpeechFromName("patrol");
                    }
                    break;
                case speechStatus.Distracted:
                    if (status != speechStatus.Distracted)
                    {
                        status = value;
                        PlaySpeechFromName("distracted");
                    }
                    break;
                case speechStatus.Chasing:
                    if (status != speechStatus.Chasing)
                    {
                        status = value;
                        PlaySpeechFromName("chasing");
                    }
                    break;
            }

        }
    }

    // Off switch for co-routine.
    private bool speaking;

    // Start is called before the first frame update
    void Start()
    {
        status = speechStatus.Default;
        // ignore layer is actually everything but ignore layer..
        f_ignoreLayer = ~f_ignoreLayer;
        TextComponent = GetComponent<TextMeshPro>();
        alphaControl = GetComponent<CanvasGroup>();
        sizeControl = GetComponent<RectTransform>();
        fontControl = TextComponent.fontSharedMaterial;
        // intialise character dialogs
        CharacterLines = JsonUtility.FromJson<SpeechLines>(myDialogConf.text);
        PlaySpeechFromName("default");

        VocalRange = GetComponentInChildren<RangeCollider>();
        VocalRange.onTriggerEnter_Action += VocalCollider_TriggerEnter;
        VocalRange.onTriggerStay_Action += VocalCollider_TriggerStay;
        VocalRange.onTriggerExit_Action += VocalCollider_TriggerExit;

        TextComponent.alpha = 0;

    }

    private void VocalCollider_TriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            TextComponent.alpha = 1 - ((other.transform.position - transform.position).magnitude / VocalRange.GetComponent<SphereCollider>().radius);
        }
    }
    private void VocalCollider_TriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {

            TextComponent.alpha = 1 - ((other.transform.position - transform.position).magnitude / VocalRange.GetComponent<SphereCollider>().radius);
            if (Physics.Raycast(transform.position, (other.transform.position - transform.position).normalized, out RaycastHit hit, VocalRange.GetComponent<SphereCollider>().radius * 2, f_ignoreLayer))
            {
                if (hit.transform.CompareTag("Player")) fontControl.SetFloat(ShaderUtilities.ID_FaceDilate, 0);
                else fontControl.SetFloat(ShaderUtilities.ID_FaceDilate, 1f);
            }
        }
    }
    private void VocalCollider_TriggerExit(Collider other)
    {
        TextComponent.alpha = 0;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - PlayerCam.transform.position);
    }
    public void PlaySpeechFromName(string name)
    {
        // We can assume that a coroutine is running if speaking is true, as false implies coroutine has passed loop and ended.
        if (speaking) StopCoroutine(VoiceBox);
        foreach (SpeechLine line in CharacterLines.speechLines)
        {
            if (line.id.ToLower() == name.ToLower())
            {
                speechClips = new Queue<SpeechClip>();
                foreach (SpeechClip clip in line.clips)
                {
                    speechClips.Enqueue(clip);
                }
            }
        }
        // Start playing new speech queue
        VoiceBox = Speaking(speechClips);
        StartCoroutine(VoiceBox);
    }
    private IEnumerator Speaking(Queue<SpeechClip> clips)
    {
        speaking = true;
        //WaitForSecondsRealtime(aud.length());
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(7);
        while (speaking)
        {
            // pop, play and requeue the clip.
            SpeechClip clip = clips.Dequeue();
            PlayClip(clip);
            clips.Enqueue(clip);
            yield return wait;
        }
    }
    void PlayClip(SpeechClip clip)
    {
        string subtitle = clip.Subtitle;
        // load audio
        //Audio aud = PathUtility.FromAssets(male ? clip.MaleRec : clip.FemaleRec);
        //? aud.play()
        TextComponent.text = subtitle;
        //Delay for the duration of the audio clip.
        //? Delay(aud.length())
    }
}

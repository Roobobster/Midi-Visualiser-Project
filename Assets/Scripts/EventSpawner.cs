using Multimedia.Midi;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class EventSpawner : MonoBehaviour
{

    [SerializeField] private GameObject eventPrefab;

    [SerializeField] private Transform playerTransform;

    [SerializeField] private List<Transform> patternTransforms;

    [SerializeField] private Slider baiEnergyBar;
    [SerializeField] private Slider maoEnergyBar;
    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject powerUpPrefab;

    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private List<string> midiFileNames;


    private float travelTime;


    private Sequence midiSequence;

    private string baseMidiFilePath = @"C:\Users\Robert\Desktop\Coding Projects\Summer Projects\Neon Bouncer\Assets\Sounds\Music\";

    private List<List<PlayNote>> noteActions;


    private Random random;


    private double time;

    private double secondsPerTick;

    private int trackNumber = 0;
    private float currentTrackLength;

    

    private bool songStarted = false;

    private float loadTime = 2;


    private bool gameStarted;

    private void ParseMidiFile()
    {

        float ticksPerQuarter = midiSequence.Division ;



        List<Tempo> tempos = midiSequence.GetTempo();
        UnityEngine.Debug.Log(tempos.Count + " Tempo Length");

        Debug.Log("Time Signature: " + midiSequence.GetTimeSignature()[0] + " / " + midiSequence.GetTimeSignature()[1]);
        Debug.Log("Tracks: " + midiSequence.Count);
        Debug.Log("Tempo: " + 60000000f / tempos[0].value);
        Debug.Log("Length: " + midiSequence.GetLength());
        Debug.Log("Division: " + midiSequence.Division);
        float bpm = 60000000f / tempos[0].value;

        //Still need to work out time signature for none 4/4 
        secondsPerTick = 60.0 / (bpm  * ticksPerQuarter);


        double firstNote = -1;

        for (int i = 0; i < midiSequence.GetTrackCount(); i++)
        {

            foreach (MidiEvent midiEvent in midiSequence.GetTrack(i).GetMidiEvents())
            {

                if (midiEvent.Message.GetType() == (typeof(ChannelMessage)))
                {
                    ChannelMessage channelMessage = (ChannelMessage)midiEvent.Message;
                    if (channelMessage.Command == ChannelCommand.NoteOn || channelMessage.Command == ChannelCommand.NoteOff)
                    {

                        double noteTime = midiEvent.Ticks * secondsPerTick;

                        if (firstNote == -1 || firstNote > noteTime)
                        {
                            firstNote = noteTime;
                        }


                    }
                }


            }
        }


        for (int i = 0; i < midiSequence.GetTrackCount(); i++)
        {
            

            GameObject eventClone = Instantiate(eventPrefab, Vector3.zero, Quaternion.identity);

            Color currentTrackColour = eventClone.GetComponent<SpriteRenderer>().material.GetColor("_GlowColour");

            float colourIntensity = 5;
            currentTrackColour.r = (float)(random.NextDouble() * colourIntensity);

            currentTrackColour.g = (float)(random.NextDouble() * colourIntensity);

            currentTrackColour.b = (float)(random.NextDouble() * colourIntensity);
            
            Destroy(eventClone);
            // First Track/Instrument
            List<PlayNote> trackList = new List<PlayNote>();



            int tempoIndex = -1;
            foreach (MidiEvent midiEvent in midiSequence.GetTrack(i).GetMidiEvents())
            {

                if (midiEvent.Message.GetType() == typeof(ChannelMessage)) 
                {
                    ChannelMessage channelMessage = (ChannelMessage) midiEvent.Message;
                    if (channelMessage.Command == ChannelCommand.NoteOn || channelMessage.Command == ChannelCommand.NoteOff) 
                    {
                        //Changes the bpm if needed
                        if (tempoIndex + 1 < tempos.Count && midiEvent.Ticks >= tempos[tempoIndex + 1].ticks ) 
                        {
                            
                    
                            tempoIndex++;
                            float newBPM = 60000000f / tempos[tempoIndex].value;
                            //Still need to work out time signature for none 4/4 
                            secondsPerTick = 60.0 / (newBPM * ticksPerQuarter);                       

                        }

                        double noteTime = midiEvent.Ticks * secondsPerTick;


                        PlayNote noteAction = new PlayNote(channelMessage.Data1, noteTime - firstNote , channelMessage.Command, currentTrackColour);
                        //Debug.Log("Time: " + noteTime);
                        trackList.Add(noteAction);

                    }
                }

  


            }
            
            noteActions.Add(trackList);
        }


    }


    // Start is called before the first frame update
    void Start()
    {
        
        travelTime = eventPrefab.GetComponent<EnemyHandler>().GetReachTime();

        time = travelTime * -1f;
        string midiFilePath = Application.dataPath + "/Sounds/Music/" + midiFileNames[trackNumber] + ".mid";
        Debug.Log(midiFilePath);
        GetComponent<AudioSource>().clip = audioClips[trackNumber];
        currentTrackLength = audioClips[trackNumber].length;
        trackNumber++;

        noteActions = new List<List<PlayNote>>();
        MidiFileReader reader = new MidiFileReader(midiFilePath);

        midiSequence = reader.Sequence;

        random = new Random();

        

        ParseMidiFile();
        

        gameStarted = false;

        StartCoroutine("WaitForProccessing");

        StartCoroutine("SyncMusic");

        StartCoroutine("WaitForPowerUp");
    }

    IEnumerator WaitForProccessing()
    {
        //yield on a new YieldInstruction that waits for a second for all things to load
        yield return new WaitForSeconds(loadTime);     
        gameStarted = true;

    }

    IEnumerator SyncMusic()
    {
        //Waits the travel time for the first note so they sync up with the actual music 
        yield return new WaitForSeconds(loadTime + travelTime );
        GetComponent<AudioSource>().Play();
    }


    void Update()
    {

        maoEnergyBar.value += 3f * Time.deltaTime;
        baiEnergyBar.value += 3f * Time.deltaTime;
        if (gameStarted) {
            SpawnEvents();
            time += Time.deltaTime;
            if (currentTrackLength < time && trackNumber < audioClips.Count )
            {
                StartCoroutine("StartSongLevel");
                time = 0;
                gameStarted = false;

            }
            
            
        }
        

    }


    IEnumerator StartSongLevel()
    {

        yield return new WaitForSeconds(4f);
        Start();
        
    }

    IEnumerator WaitForPowerUp()
    {

        yield return new WaitForSeconds(UnityEngine.Random.Range(4f, 15f));
        float x = UnityEngine.Random.Range(transform.position.x + -120f, transform.position.x + 120f);
        Vector3 powerUpPosition = new Vector3(x, 0f, -5f);
        Instantiate(powerUpPrefab, powerUpPosition, Quaternion.identity);
        StartCoroutine("WaitForPowerUp");
    }


    private void SpawnEvents() 
    {
      
        
        // Loops for every track 
        for (int currentTrack = 0; currentTrack < noteActions.Count; currentTrack++)
        {

            if (noteActions[currentTrack].Count > 0) 
            {
                int nextNoteIndex = 0;
                PlayNote noteAction = noteActions[currentTrack][nextNoteIndex];

                while (noteAction.getTime() - travelTime <= time)
                {


                    if (noteAction.getEventType() == ChannelCommand.NoteOn)
                    {
                        // Increase players energy to shoot 
                        maoEnergyBar.value += 1f;
                        baiEnergyBar.value += 0.5f;

                        //Random placeo on screen
                        float y = UnityEngine.Random.Range(transform.position.y + 20f, transform.position.y + 120f);
                        float x = UnityEngine.Random.Range(transform.position.x + -120f, transform.position.x + 120f);

                        int patternIndex = UnityEngine.Random.Range(0, patternTransforms.Count);

                        // Generates event sprite
                        Vector3 randomPosition = new Vector3(x, y, 1);
                        randomPosition = patternTransforms[patternIndex].GetComponent<LineRenderer>().GetPosition(0) + patternTransforms[patternIndex].position;
                        GameObject eventClone = Instantiate(eventPrefab, randomPosition, Quaternion.identity);
                        eventClone.GetComponent<EnemyHandler>().SetMoveTransforms(playerTransform, patternTransforms[patternIndex]);
                        eventClone.GetComponent<EnemyHandler>().SetScoreText(scoreText);

                        //Sets sprites glow to be tracks glow colour
                        eventClone.GetComponent<SpriteRenderer>().material.SetColor("_GlowColour", noteAction.GetColor());


                        // Removes the action after processed
                        noteActions[currentTrack].RemoveAt(0);


                    }

                    // Incase you are at tend of track it jumps out before trying next
                    if (nextNoteIndex + 1 >= noteActions[currentTrack].Count)
                        break;


                    nextNoteIndex++;
                    noteAction = noteActions[currentTrack][nextNoteIndex];

                }
 
                
                
                
            }
            
                    


        }


        
    }

}

public class PlayNote
{
    private int note;
    //This is the number of quarters to wait
    private double time;
    private ChannelCommand eventType;

    private Color colour;

    public PlayNote(int note, double time, ChannelCommand eventType, Color colour)
    {
        this.colour = colour;
        this.note = note;
        this.time = time;
        this.eventType = eventType;
    }

    public Color GetColor()
    {
        return colour;
    }

    public int getNote()
    {
        return note;
    }

    public double getTime()
    {
        return time;
    }

    public ChannelCommand getEventType()
    {
        return eventType;
    }



}

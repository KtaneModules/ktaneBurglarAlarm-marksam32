
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

using Rnd = UnityEngine.Random;

public class BurglarAlarmScript : MonoBehaviour {

    public KMAudio Audio;
    public KMSelectable[] Buttons;
    public KMBombModule Module;
    public TextMesh DisplayText;
    public KMBombInfo Info;
    public KMSelectable ActivateButton;
    public KMSelectable SubmitButton;

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private bool isSolved = false;

    private int[] moduleNumber;
    private IList<INumberHandler> numberHandlers;
    private int[] answers;
    private bool activated;

    private Regex TwitchPlayRegex = new Regex(@"^submit +(\d\s*\d\s*\d\s*\d\s*\d\s*\d\s*\d\s*\d)$");


    private KMAudio.KMAudioRef activationSound;

    private int noPressed;
    void Activate()
    {
        this.activated = false;
        this.moduleNumber = new int[8];
        this.answers = new int[8];

        for (int i = 0; i < this.moduleNumber.Length; ++i)
        {
            this.moduleNumber[i] = Rnd.Range(0, 10);
        };

        var burglarAlarmHelper = new BurglarAlarmHelper(this.moduleNumber, this.Info);
        this.numberHandlers = CreateNumberHandlers(burglarAlarmHelper);

        Debug.LogFormat("[Burglar Alarm #{0}] Module number is: {1}.", this._moduleId, string.Join(string.Empty, this.moduleNumber.Select(x => x.ToString()).ToArray()));
        LogSolutionAlternatives(this._moduleId, this.moduleNumber, this.Info);

        this.DisplayText.text = burglarAlarmHelper.ToStringNumber;

        this.Info.OnBombExploded += delegate ()
        {
            this.StopSound();
        };

        for (int i = 0; i < this.Buttons.Count(); ++i)
        {
            var myIndex = i;
            this.Buttons[i].OnInteract += delegate ()
            {
                Audio.PlaySoundAtTransform("Button sound", this.Buttons[myIndex].transform);
                this.Buttons[myIndex].AddInteractionPunch();

                if (!activated || this.isSolved)
                {
                    return false;
                }

                if (this.noPressed >= 8)
                {
                    Debug.LogFormat("[Burglar Alarm #{0}] Pressed too many numbers! Strike!", this._moduleId);
                    this.HandleStrike();
                    return false;
                }

                else
                {
                    this.answers[this.noPressed++] = myIndex;
                }

                return false;
            };
        }

        this.ActivateButton.OnInteract += delegate ()
        {
            Audio.PlaySoundAtTransform("Button sound", this.ActivateButton.transform);
            ActivateButton.AddInteractionPunch();
            if (this.isSolved)
            {
                return false;
            }

            if (this.activated)
            {
                this.HandleStrike();
            }
            else
            {
                Debug.LogFormat("[Burglar Alarm #{0}] Module activated!", this._moduleId);
                Debug.LogFormat("[Burglar Alarm #{0}] Started at {1} solves.", this._moduleId, this.Info.GetSolvedModuleNames().Count);
                Debug.LogFormat("[Burglar Alarm #{0}] Expected input: {1}.", this._moduleId, LogModuleSolutionNumber(numberHandlers));
                StartCoroutine("Countdown");
                this.activationSound = Audio.PlaySoundAtTransformWithRef("Activation sound", Module.transform);
                this.activated = true;
            }

            return false;
        };

        this.SubmitButton.OnInteract += delegate ()
        {
            if (this.activated)
            {
                Debug.LogFormat("[Burglar Alarm #{0}] Submitted: {1}.", this._moduleId, string.Join(string.Empty, this.answers.Select(x => x.ToString()).ToArray()));
            }

            Audio.PlaySoundAtTransform("Button sound", this.SubmitButton.transform);
            SubmitButton.AddInteractionPunch();
            if (this.isSolved)
            {
                return false;
            }

            if (!this.activated)
            {
                Debug.LogFormat("[Burglar Alarm #{0}] Submitted a number before activating the module. Strike!", this._moduleId);
                this.HandleStrike();
                return false;
            }

            bool success = true;
            for (int i = 0; i < this.answers.Count(); ++i)
            {
                if (this.answers[i] != this.numberHandlers[i].GetNumber())
                {
                    success = false;
                }
            }

            if (success)
            {
                Debug.LogFormat("[Burglar Alarm #{0}] Module passed!", this._moduleId);
                this.HandlePass();
            }
            else
            {
                Debug.LogFormat("[Burglar Alarm #{0}] Wrong answer!", this._moduleId);
                this.HandleStrike();
            }

            return false;
        };

    }
    // Use this for initialization
    void Start()
    {      
        _moduleId = _moduleIdCounter++;
        Module.OnActivate += Activate;      
    }

    private void HandleStrike()
    {
        StopCoroutine("Countdown");
        Audio.PlaySoundAtTransform("Strike sound effect", this.Module.transform);
        this.Module.HandleStrike();
        this.activated = false;
        this.isSolved = false;
        this.StopSound();
        this.noPressed = 0;
    }

    private void HandlePass()
    {
        StopCoroutine("Countdown");
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, this.Module.transform);
        this.Module.HandlePass();
        this.activated = false;
        this.isSolved = true;
        this.DisplayText.text = ("");
        this.StopSound();
        this.noPressed = 0;
    }

    private void StopSound()
    {
        if (this.activationSound != null)
        {
            this.activationSound.StopSound();
            this.activationSound = null;
        }
    }

    private IEnumerator Countdown()
    {
        for (int i = 1; i <= 15; i++)
        {
            yield return new WaitForSeconds(1f);
        }

        this.HandleTimeout();
        yield return null;
    }

    private void HandleTimeout()
    {
        if (this.activated == true)
        {
            Debug.LogFormat("[Burglar Alarm #{0}] Strike! Time ran out.", _moduleId);
            this.HandleStrike();
        }
    }

    private static IList<INumberHandler> CreateNumberHandlers(BurglarAlarmHelper burglarAlarmHelper)
    {
        var  numberHandlers = new List<INumberHandler>();

        numberHandlers.Add(new NumberHandlerPos1(burglarAlarmHelper));
        numberHandlers.Add(new NumberHandlerPos2(burglarAlarmHelper));
        numberHandlers.Add(new NumberHandlerPos3(burglarAlarmHelper));
        numberHandlers.Add(new NumberHandlerPos4(burglarAlarmHelper));
        numberHandlers.Add(new NumberHandlerPos5(burglarAlarmHelper));
        numberHandlers.Add(new NumberHandlerPos6(burglarAlarmHelper));
        numberHandlers.Add(new NumberHandlerPos7(burglarAlarmHelper));
        numberHandlers.Add(new NumberHandlerPos8(burglarAlarmHelper));

        return numberHandlers;
    }

    private static void LogSolutionAlternatives(int moduleId, int[] a, KMBombInfo bomboInfo)
    {
        Debug.LogFormat("[Burglar Alarm #{0}] Solution for even solves and Number of solves <= (Batteries x portplates): {1}.", moduleId, LogModuleSolutionNumber(CreateNumberHandlers(new BurglarAlarmHelper(a, bomboInfo, true, false))));
        Debug.LogFormat("[Burglar Alarm #{0}] Solution for even solves and Number of solves > (Batteries x portplates): {1}.", moduleId, LogModuleSolutionNumber(CreateNumberHandlers(new BurglarAlarmHelper(a, bomboInfo, true, true))));
        Debug.LogFormat("[Burglar Alarm #{0}] Solution for odd solves and Number of solves <= (Batteries x portplates): {1}.", moduleId, LogModuleSolutionNumber(CreateNumberHandlers(new BurglarAlarmHelper(a, bomboInfo, false, false))));
        Debug.LogFormat("[Burglar Alarm #{0}] Solution for odd solves and Number of solves > (Batteries x portplates): {1}.", moduleId, LogModuleSolutionNumber(CreateNumberHandlers(new BurglarAlarmHelper(a, bomboInfo, false, true))));
    }

    private static string LogModuleSolutionNumber(IList<INumberHandler> handlers)
    {
        return string.Join(string.Empty, handlers.Select(x => x.GetNumber().ToString()).ToArray());
    }
    

    //Twitch plays:
    public KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();

        if (command.Equals("activate"))
        {
            return new[] { ActivateButton };
        }

        var match = TwitchPlayRegex.Match(command);
        if (match.Success)
        {
            return match.Groups[1].Value.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").Select(x => Buttons[int.Parse(x.ToString())]).Concat(new[] { SubmitButton }).ToArray();
        }

        return null;
    }

    public IEnumerator TwitchHandleForcedSolve()
    {
        var solution = numberHandlers.Select(x => x.GetNumber()).ToList();

        ActivateButton.OnInteract();
        for (int i = 0; i < 8; i++)
        {
            Buttons[solution[i]].OnInteract();
            yield return new WaitForSeconds(.15f);
        }

        SubmitButton.OnInteract();
        yield return true;
    }

    public string TwitchHelpMessage = "Activate the module using !{0} activate , Submit the correct answer using !{0} submit ######## .";

    // Update is called once per frame
    void Update () {	
	}
}

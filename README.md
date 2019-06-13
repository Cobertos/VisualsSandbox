<p align="center">
    <a href="https://twitter.com/cobertos" target="_blank"><img alt="twitter" src="https://img.shields.io/badge/twitter-%40cobertos-0084b4.svg"></a>
    <a href="https://cobertos.com" target="_blank"><img alt="twitter" src="https://img.shields.io/badge/website-cobertos.com-888888.svg"></a>
</p>

# Visuals Sandbox for Unity3D 2019.1+

Tools and sandbox environment for making music visuals in Unity3D. Provides nice interfaces and debugging visuals for BPM sources, Time and Frequence space data, etc...

It requires 2019.1+ to use `VisualElement`s. The code _could_ be backported to 2017.1+ but requires some modification...

originally based on [my similar experiments/toolkit for THREE.js](https://github.com/Cobertos/MIDI-Experiment)

### Installation

1. Download or clone the repository, `git clone https://github.com/Cobertos/VisualsSandbox`
2. Open it as a Unity project

### Usage

There's not much here yet, but below is the list of components that currently exist:

#### `BPMSource`

![Bpm Source gif](./Media/bpmSource.gif)

Represents a BPM source. The base class requires manually entering the BPMSource, but can be extended to infer BPM from audio intensity data or through a MIDI input from you or your friends DAW. Has a bunch of useful properties

```
.bpm - The current bpm
.bpms - The number of beats in a millisecond
.beat - The current beat of your song, given a proper epoch
.getLastBeatTime(double frac = 1, double offset = 0) - The last time in milliseconds a beat occured
.onceEvery(double frac, double offset, OnceEveryFunc func) - Executes a function every beat
```

check out [BPMSource.cs](./Assets/Scripts/VisualsTools/BPMSource.cs) for more info

#### `AudioSourceVisualizer`

![Audio source visualizer, log spectrum](./Media/audioVisualizer.gif)

Plots the spectrum data of an audio source live.

#### `MicrophoneAudioSource`

![Microphone audio source GUI](./Media/microphoneAudioSource.png)

Working with Microphone sound is a massive pain in Unity. This `MonoBehavior` sets up the attached `AudioSource` with Microphone input. Using something like [`Virtual Audio Cable/Volumeeter`](https://www.vb-audio.com/Cable/index.htm) on Windows, you can pipe music to a Microphone input and do stuff with it in Unity.

Note:

* There seems to be latency based on the length of the clip in `Microphone.Start()`. It's an integer, so the lowest it goes is 1, 1 second of latency seemingly. This is unfixable right now :(
* If you experience garbage crackling and stuff in your microphone, make sure that your audio source outputs to a mixer. Once you start your game in the editor, edit the pitch of that mixer and set it back to 100.00%, it should go away while the game is still active. Annoying af, but I don't know how to fix.
* Sometimes Unity will get really nasty memory leaks when working with `Microphone.Start()`. If your console fills up with them just restart Unity. Though it's usually stable...

### Contributing

No specific instructions as of now
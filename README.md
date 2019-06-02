<p align="center">
    <a href="https://twitter.com/cobertos" target="_blank"><img alt="twitter" src="https://img.shields.io/badge/twitter-%40cobertos-0084b4.svg"></a>
    <a href="https://cobertos.com" target="_blank"><img alt="twitter" src="https://img.shields.io/badge/website-cobertos.com-888888.svg"></a>
</p>

# Visuals Sandbox for Unity3D

Tools and sandbox environment for making music visuals in Unity3D. Provides nice interfaces and debugging visuals for BPM sources, Time and Frequence space data, etc...

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

### Contributing

No specific instructions as of now
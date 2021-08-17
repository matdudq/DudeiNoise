# Dudei Noise
[![Unity 2019.3+](https://img.shields.io/badge/unity-2020.1%2B-blue.svg)](https://unity3d.com/get-unity/download) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Table Of Contents

- [Introduction](#introduction)
- [Features](#features)
- [System Requirements](#system-requirements)
- [Dependencies](#dependencies)
- [Installation](#installation)
- [Overview](#overview)
	- [Dudei Noise Window](#dudei-noise-window)
	- [Texture Settings](#texture-settings)
	- [Space Mode Toolbar](#space-mode-toolbar)
	- [Noise Type Settings](#noise-type-settings)
	- [Frequency Space](#frequency-space)
	- [Noise Type Settings](#noise-type-settings)
	- [Octaves Settings](#octaves-settings)
	- [Custom Patterns](#custom-patterns)
	- [Falloff Settings](#falloff-settings)
	- [Bottom Panel](#bottom-panel)
- [License](#license)

## Introduction <a name="introduction"></a>

**Unity Dudei Noise Editor** is an open-source tool for creating and defining custom noise textures. 

Tool was created to:
- simplify process of getting desired custom noises inside unity editor.
- optimize caching more than one noise textures.
- allow user to use noise library with easy to understand API.
## Features <a name="features"></a>

- Separate window for Noise Editor, which consists of Noise texture chanel preview and editor itself.
- Possibility to define noises with combined patterns and settings.
- Possibility to define 1/2/3-dimentional noises.
- Possibility to define noises of default, value and perlin type.
- Possibility to manipulate of noise space (position/rotation/frequency).
- Possibility to define noise with custom octaves settings.
- Possibility to add custom patterns like wood or turbulence.
- Possibility to add falloff map to noise.
- Possibility to save one noise map on each texture chanel.
- Possibility to export texture to png file. 
- Library that allows us to generate noises runtime on single thread as well as multi-threaded with use of JOBS system.

## System Requirements <a name="system-requirements"></a>

Unity 2019.3 or newer.

## Dependencies <a name="dependencies"></a>

[Burst](https://docs.unity3d.com/Packages/com.unity.burst@1.4/manual/index.html)

[Unity.Mathematics](https://docs.unity3d.com/Packages/com.unity.mathematics@1.0/manual/index.html)

## Installation <a name="installation"></a>

1 The package is available in Unity Package Manager via git URL. Follow up [this](https://docs.unity3d.com/Manual/upm-ui-giturl.html) Unity page for detailed instructions. Git URL:
```
https://github.com/matdudq/DudeiNoise
```
2 You can also install Dudei Noise by simply downloading repository zip file and copying Assets folder content to your Unity project.
3 You can also download tool as sub-module repository.

## Overview <a name="overview"></a>

### Dudei Noise Window <a name="dudei-noise-window"></a>

Editor window can be opened by going to `Tools->Noise Generator Window` on the Unity toolbar. Noise texture preview for one individual chanel will also show when enabling editor. Windows can be manipulated separately. Mainly we will be working with generator editor which allows us to define our noise behaviour.

 <img src="https://i.imgur.com/dsCZO9H.gif">

### Texture Settings <a name="texture-settings"></a>

In texture settings panel we can manipulate export folder resolution of texture as well as filter mode of exported texture.

 <img src="https://i.imgur.com/jHc3PGy.png">

### Space Mode Toolbar <a name="space-mode-toolbar"></a>

Space mode tool bar allows us to define if we want to have tilling texture with only option to manipulate frequency of texture or we wan to have texture with custom space.

 <img src="https://i.imgur.com/PWRtSZD.gif">

### Noise Type settings <a name="noise-type-settings"></a>

Noise type options give you choice of noise type that you will be working on. You can choose default which is simple random, value or perlin noise. Except that you can define dimension of noise that you want to generate.

 <img src="https://i.imgur.com/hxTJTQB.gif">
 
### Frequency/Custom Space settings  <a name="frequency-space"></a>

Depends on mode that you have chosen here you will be able to manipulate space of noise. Moving rotating or scaling frequency of noise.

 <img src="https://i.imgur.com/5uq5XSf.gif">
 
 ### Octaves settings  <a name="octaves-settings"></a>

Here you can manipulate all the layers of noise. To use that settings layer efficiently you should play with it a little bit. 

 <img src="https://i.imgur.com/EGvimei.gif">
 
 ### Custom patterns <a name="custom-patterns"></a>

Custom patterns give us possibility to create really custom noises. Right now wood pattern and turbulence are implemented. To see what they do, you should also play with them.

 <img src="https://i.imgur.com/WCudLYP.gif">  

### Falloff settings <a name="falloff-settings"></a>

It is control panel of falloff mask which allows us to create blurred borders of noise - useful for example when use want to create some kind of islands based or noise.

 <img src="https://i.imgur.com/ZHKp8os.gif">
 
### Bottom panel <a name="bottom-panel"></a>

Switches that allows you to swap currently working chanel and save the texture.

 <img src="https://i.imgur.com/EooqKp7.png">
 
 ## License <a name="license"></a>
 
[MIT](https://opensource.org/licenses/MIT)

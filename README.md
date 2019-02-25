# Heads Up!

I'm working on a new version written completely from scratch using the job system right now.
Here's a little teaser. Follow me on twitter [@keenanwoodall](https://twitter.com/keenanwoodall) for updates. 

![Teaser](https://media.giphy.com/media/2ifaiLjklPTpskqO21/giphy.gif)

# Deform

Deform is a framework for deforming meshes in the editor and at runtime that comes with a component based deformation system build on top.
If you don't want to make your own deformers, it comes with many standard ones you'd find in 3D modeling packages.

**IMPORTANT**
1. If you are using this in a pre-existing project, you need to go to Edit/Project Settings/Player/ and set the Scripting Runtime Version (under the Other Settings dropdown) to 4.6.
2. Right now this project is not suitable for professional development. Don't use it on large projects unless you are happy with the feature-set at the time of downloading/cloning it. If you aren't using version control DO NOT update to new versions of this system without backing up your project. Stuff you've made will break 99% of the time because I'm doing large/sweeping changes almost every time I push.

**How it works**
1. Find a game object (with a mesh filter or skinned mesh renderer) in your scene.
2. Add any deformer component.
3. That's it. Your mesh is deformed.
![Tutorial 1](https://media.giphy.com/media/3xIEzQcdH52oUzz8Zq/giphy.gif)

**Tutorials**
- [Introduction](https://youtu.be/1cFPUsI57hM?list=PLrWlVANGG-iitlKSK5Pr8eUCi48gznygc)
- [Playlist](https://www.youtube.com/watch?v=1cFPUsI57hM&list=PLrWlVANGG-iitlKSK5Pr8eUCi48gznygc)

**Features**
- Runs in edit and play mode
- Multithreaded (optional)
- Meshes can be saved
- Deformers can be stacked and reordered
- Works with skinned meshes (kinda)
- Easily Extendable

**Built-in Deformers**
- Bend
- Color Mask
- Curve
- Cylindrify
- Noise (Value, Perlin, Simplex, Cellular, Cubic)
- Pivot To Bounds
- Pivot To Center
- Ripple
- Scale Along Axis
- Scale Along Normal
- Sine
- Skew
- Spherify
- Squash and Stretch
- Taper
- Texture Mask
- Transform
- Twist
- Wave

**Releases**

- [v0.1](https://github.com/keenanwoodall/Deform/releases/tag/v0.1)

**FAQ**

_I don't want all the fluff. What can I safely remove?_
- You can delete everything except for the Code and Plugins folder.

_How do I make my own deformer?_
1. Make a script that uses the Deform namespace
2. Inherit from the 'DeformerComponent' class.
3. Override the 'Modify' method.
4. Make changes to the mesh data and return it.
5. Drag your script onto any object with a MeshFilter or SkinnedMeshRenderer.
6. Mission complete.

<br />

_What is the MeshData struct?_
- It holds the positions and normals (as well as some other stuff) of a mesh.

<br />

_Why am I getting the error,_ `xxx can only be called from the main thread`_?_
- Unity locks access to most things from other threads. You are probably accessing something like a Transform component from inside the `Modify` method, which runs on another thread when UpdateMode is set to UpdateAsync. I recommend overriding the virtual method, `PreModify`, and caching anything you need that is inaccessable from another thread. `PreModify` is called on the main thread right before any deformations calculations are run.

<br />

_Why am I getting the error,_ `xxx asynchronous functions cannot be used because it is not part of the C# 4.0 language specification`_?_
- You need to go to Edit/Project Settings/Player/ and set the Scripting Runtime Version (under the Other Settings dropdown) to 4.6.

<br />

**Help**

- Please send any suggestions/feedback to keenanwoodall@gmail.com
- Pull requests are welcome!

<br />
<br />

![1](https://i.redd.it/k4ttrf6p78l01.gif)
![2](https://i.imgur.com/IszhtkM.gif)
![3](https://i.redd.it/phjbwgdih7n01.gif)

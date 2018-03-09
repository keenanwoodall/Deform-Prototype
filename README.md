Deform is a framework for deforming meshes in the editor and at runtime that comes with a component based deformation system build on top.
If you don't want to make your own deformations, it comes with many standard deformers/modifiers you'd find in 3D modeling packages.

**How it works**
1. Find a game object in your scene.
2. Add any deformer component.
3. That's it; your mesh is deformed.

**Features**
- Runs in edit and play mode
- Multithreaded (optional)
- Meshes can be saved
- Deformers can be stacked and reordered
- Extendable

**Built-in Deformers**
- Bend
- Color Mask
- Curve
- Cylindrify
- Perlin
- Pivot To Bounds
- Pivot To Center
- Ripple
- Scale Along Axis
- Scale Along Normal
- Sin
- Skew
- Spherify
- Squash and Stretch
- Taper
- Transform
- Twist

**FAQ**

_How do I make my own deformer?_
1. Make a script that uses the Deform namespace
2. Inherit the 'DeformerComponent' class.
3. Override the 'Modify' method.
4. Make changes to the vertex data and return it.
5. Drag your script onto any object with a MeshFilter or SkinnedMeshRenderer.
6. Mission complete.

_What is the VertexData struct?_
- It holds the position and normal (as well as some other stuff) of a vertice.

_Why am I getting the error,_ `xxx can only be called from the main thread`_?_
- Unity locks access to most things from other threads. You are probably accessing something like a Transform component from inside the `Modify` method, which runs on another thread when UpdateMode is set to UpdateAsync. I recommend overriding the virtual method, `PreModify`, and caching anything you need that is inaccessable from another thread. `PreModify` is called on the main thread right before any deformations calculations are run.

_I don't like component based deformation, how can I make my own system?_
- Inherit from DeformerBase. To see how to use it you can use DeformerComponentManager as a reference.

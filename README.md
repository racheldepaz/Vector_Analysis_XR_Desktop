# Vector_Analysis_XR_Desktop
The actual scenes/elements used in the visual demo is in the Control/Assets/VectorsInSpace folder. 

"_ Placement.cs" connects all the scripts and acts as the main class for the placement scene. 
"CanvasScript.cs" controls all the text-related elements, even if they aren't in the canvas. Sorry for the bad naming! This script controls the TextMeshPro texts for the distance, axes, and unit vectors, but not the actual value of the line renderers they represent. 
"VectorMath.cs" calculates the position of each line renderer. This is where the math behind the visualization happens. 

You'll notice that angle arcs are programmed to be displayed as a dynamic 3D mesh attached to the Content Root empty game object. For some reason, they're not working. The angle arcs are programmed in the VectorMath.cs method under the VisualizeComponents method. 

If you want to add horizontal rotation, you'll need to add that in the main class. 

Newest build: visBUILD2.mpk

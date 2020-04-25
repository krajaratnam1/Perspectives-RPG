This asset is a tool to make outlines on everything you put on your scene.
The effect of outlines is render on the camera and not on the mesh, 
this makes the effect lighter than regular outline, it is render in just one pass.

There are 6 shaders on this asset:
 Outlines:
	Color Based : it randes the outline using the variation of color
                      if the color changes intensity it does not out line.

	Simple Normals: It uses the camrea rendered normals and depth to generate
			out lines, mobile don't render depth, so you might have a diferent
			result on andorid, but it still works

	Contrast based: it randes lines where there are abrupt changes of color and 
			intensity, it outlines shadows.

	Normal Based with contrast correction: This is the haviest one, it requares 
			two cameras, one that will render the transparent shaders, and one that 
			will render everything including the transparent shaders,it gives the best
			results, but keep in mind that it might not be ideal to weak 
			smartphones. The camera for the transparet objects must have a solid
			color black backgrould  

Object Shaders:
	LightToon with shadows and light toon without shadows
	They just darks the color where thare isn't direct light. One can recive shadows and 
	the other can't.

All materials have cool proprieties to play with, get the result that you want.


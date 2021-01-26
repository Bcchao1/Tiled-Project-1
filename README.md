Group Members: 

--Anya, Bryan, Bjarke

Example Output: 

--in containing folder as well as "neat images" folder

How to run code:

Use the .bat files, either Runwfc_auto or Runwfc_standard OR run "dotnet run WaveFunctionCollapse.csproj [true/false]" in CMD.
* auto will setup and run on any images in the "samplesauto" folder.
* standard requires setup of samples.xml, and the corresponding image to be in the "samples" folder.

Use settings.xml to change type of output:
* saveRegularSize: saves regular 100x100 output images, directly from WFC output.
* saveUpscaleds: saves upscaled versions of the 100x100 images, unchanged.
* saveRegularCombined: saves regular (100x100) images combined into a 10x10 tiled grid.
* saveUpscaledCombined: saves upscaled images, combined in a 2x2 grid.
* saveflipped: saves upscaled version, flipped four times in each cardinal direction, in a single pattern.

Rules and Constraints:

* Image Size is 30 x 30 pixels max
* Image must be represented by one simple slice of a pattern (ex. spiral, square, curve, etc.)

* Pattern Size: N=3/4
* Symmetry = 2/4/8
* Periodic=True
* Dimensions 100x100


Explain your process in creating a good output:

* First we illustrate our own tiles in the app Tiled
* We export multicolored tiles as large as 30x30 with simple geometric designs integrated into them.
* We plug the tiles into the algorithm and through our constraints and receive aesethtically pleasing backgrounds that have a 
consistent geometric pattern aesthetic.
* Experiment with tiling and upscaling images, and flipping them for other types of results with the same output.
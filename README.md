# model-edit
A tool for editing detail information of SWBF and SWBFII .model files. 

Essentially the game stores things like the triangle counts used for LOD control seperately 
from the buffers used by the renderer itself. This enables use to edit that information and 
trick the engine into thinking it is drawing cheaper models than it actually is.

The tool is designed such that you can easilly integrate it into the munge process for your
side/world/whatever. Because it is quite simple to do I'll leave it as an exercise for the
reader. (For now at least, Pull Requests welcome.)

## Usage
```
model-edit <directory> [<factor>]
   All *.model files found in <directory> will have detail information edited.
   The detail information for all modes will be divided by <factor>, it defaults to four.
```

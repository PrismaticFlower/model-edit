# model-edit
A tool for editing detail information of SWBF and SWBFII .model files. 

Essentially the game stores things like the triangle counts used for LOD control seperately 
from the buffers used by the renderer itself. This enables use to edit that information and 
trick the engine into thinking it is drawing cheaper models than it actually is.

The tool is designed such that you can easilly integrate it into the munge process for your
side/world/whatever. Because it is quite simple to do I'll leave it as an exercise for the
reader. 

Ideally you could do something along the lines of have have `porp_msh`, `soldier_msh`, `building_msh`, `weapon_msh`, etc folders for each model type. You would invoke model munge over them and have it output their contents to a discrete folder that the tool could run over, seperating model types would let you use different factors for each. Then after the tool had run you would copy the .model (not forgetting the .req files!) files out of those intermediate folders and into the main munged folder so `levelpack` could find them.

Basically given you want to change your model munging process to this.

* Munge the models normally.
* Invoke the tool over the munged models.
* If munging to an intermediate directory for fine grained factor control copy the files into the main `munged` directory.

## Usage
```
model-edit <directory> [<factor>]
   All *.model files found in <directory> will have detail information edited.
   The detail information for all modes will be divided by <factor>, it defaults to four.
```

So for example `model-edit ./ 8` would adjust the detail information of all .model files found in the current working directory by a factor of 8. Higher factors will result in the game thinking the models are less detailed, a factor of one represents no change. (Remember it uses the factor as the divisor for the face count. Also don't pass it 0.). It is up to you to find the ideal factor for your models/map/sides/whatever. Have fun!

## Great, so how does it work?
First become aquinted with the basics of SWBF's [file structure](https://github.com/SleepKiller/swbf-unmunge/wiki/Core-File-Structure). Assuming you've done that the main change this performs is that each `modl` chunk has an `INFO` data chunk, this stores various information about the model (if you want to know more about it you can see what I use for unmunging it in `read_model_info` over [here](https://github.com/SleepKiller/swbf-unmunge/blob/master/src/handle_model.cpp)). But specifically we're interested in the last field the 32-bit integer containing the face count for the model.

So this field is what the game's rendering engine uses to work out how hard it is working. This is fortunate for us because it the vertex buffers stored [elsewhere](https://github.com/SleepKiller/swbf-unmunge/blob/master/src/vbuf_reader.cpp) are what actually determine what's drawn. Using this simple peice of information we can change the engine's perceived "cost" of rendering a model to something a bit more inline with 2017 hardware. (Though the tool leaves it up to you to decide what exent.)

Each segment and shadow also have similar fields, the tool edit's them as well although that is likely unnecessary. It didn't cause harm in my limited tests so I've left it in for completeness.

Finally the game will also adjust the LOD info (stored in the `gmod` chunk). Each LOD for the model in this chunk also carries the face count of the model used for that step of LOD. These will also be edited. (What it doesn't do is touch LOD Bias as that can be controlled through the munger.

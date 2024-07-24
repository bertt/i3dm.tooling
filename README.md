# i3dm.tooling

tooling for instanced 3D Tiles (i3dm)

Global tooling for handling i3dm files, like getting information about the i3dm (info), pack (create i3dm from glb and instance metadata) and 
unpack (extract glb and instance metadata).

## API

Verbs:

```
  pack       pack i3dm

  unpack     unpack i3dm

  info       info i3dm

  help       Display more information on a specific command.

  version    Display version information.
```

Info options:

```
  -i, --input    Required. Input path of the .i3dm

  -b, --batchtablejson (Default: false) display batchTableJSON
```

Pack options:

```
  -i, --input     Required. Input path of the glb file

  -o, --output    (Default: ) Output path of the resulting .i3dm

  -f, --force     (Default: false) force overwrite output file
```


Unpack options:

```
  -i, --input     Required. Input path of the .i3dm

  -o, --output    (Default: ) Output path of the resulting .glb

  -f, --force     (Default: false) force overwrite output file
```


## Installation

- Install from NuGet

https://www.nuget.org/packages/i3dm.tooling/

```
$ dotnet tool install -g i3dm.tooling
```

or update:

```
$ dotnet tool update -g i3dm.tooling

```

## Running

### Info

Command Info i3dm_file gives header info about i3dm file

Example:

```
$ i3dm info -i tree.i3dm

Action: Info
i3dm file: tree.i3dm
i3dm header version: 1
i3dm GltfFormat: 1
i3dm header magic: i3dm
i3dm header bytelength: 282072
i3dm header featuretablejson length: 72
i3dm header batchtablejson length: 88
Batch table json: {"Height":[20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20]}
Feature table json: {"INSTANCES_LENGTH":25,"EAST_NORTH_UP":true,"POSITION":{"byteOffset":0}}
positions: <1214947.2, -4736379, 4081540.8>,<1214986, -4736369, 4081540.8>,<1215024.8, -4736359, 4081540.8>,<1215063.5, -4736349.5, 4081540.8>,<1215102.2, -4736339.5, 4081540.8>,<1214940.9, -4736354, 4081571.5>,<1214979.6, -4736344, 4081571.5>,<1215018.4, -4736334.5, 4081571.5>,<1215057.1, -4736324.5, 4081571.5>,<1215095.9, -4736314.5, 4081571.5>,<1214934.5, -4736329, 4081602>,<1214973.1, -4736319.5, 4081602>,<1215011.9, -4736309.5, 4081602>,<1215050.6, -4736299.5, 4081602>,<1215089.4, -4736289.5, 4081602>,<1214928, -4736304.5, 4081632.8>,<1214966.8, -4736294.5, 4081632.8>,<1215005.5, -4736284.5, 4081632.8>,<1215044.2, -4736274.5, 4081632.8>,<1215083, -4736264.5, 4081632.8>,<1214921.6, -4736279.5, 4081663.2>,<1214960.4, -4736269.5, 4081663.2>,<1214999.1, -4736259.5, 4081663.2>,<1215037.9, -4736249.5, 4081663.2>,<1215076.6, -4736239.5, 4081663.2>
glTF model is loaded
glTF generator: COLLADA2GLTF
glTF version:2.0
glTF primitives: 2
glTF extensions used:
glTF extensions required:
glTF primitive mode: TRIANGLES
```

### Unpack

Command unpack -i i3dm_filename 

unpacks a i3dm file to GLB format and creates csv files with .batch file when containing batch information, 
positions, normal_ups, normal_rights, scale_non_uniforms, scales.

Example:

```
$ i3dm unpack -i tree.i3dm

Action: Unpack
Input: tree.i3dm
i3dm version: 1
Glb created: tree.glb
Positions file created: tree.positions.csv
normalups file created: tree.normal_ups.csv
normalrights file created: tree.normal_rights.csv
batch file created: tree.batch.csv
feature file created: tree.feature.csv
```

tree.feature.csv contains a json file like:

```
{"INSTANCES_LENGTH":2326,"POSITION":{"byteOffset":0},"NORMAL_UP":{"byteOffset":27912},"NORMAL_RIGHT":{"byteOffset":55824},"EAST_NORTH_UP":true} 
```

tree.batch.csv contains a json file with instance metadata like:

```
{"Tree":["1","2","3",...,"2326"]}
```

### Pack

Command pack -i <glb_file> 

packs a glb file to i3dm. Reads featuretable, batchtable, positions, normal_ups, normal_rights, scale_non_uniforms, scales from 
csv files with same prefix name as glb when available.

```
Action: Pack
Input: trees.glb
Input batchtable json file: trees.batch.csv
Input featureTable json file: trees.feature.csv
Input normal_upsfile file: trees.normal_ups.csv
Input normal_rightsfile file: trees.normal_rights.csv
I3dm created trees.i3dm
```
## Building from source

```
$ cd i3dm-tile-cs\i3dm.tooling
$ dotnet pack
$ dotnet tool install --global --add-source ./nupkg i3dm.tooling
```

or update:

```
$ dotnet tool update --global --add-source ./nupkg i3dm.tooling
```

## History

24-07-2024: release 1.8.1: print geodedic positions in info

23-07-2024: release 1.8.0

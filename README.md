# i3dm.tooling
tooling for instanced 3D Tiles (i3dm)

Global tooling for handling i3dm files, like getting information about the i3dm (info)

todo: add support for unpacking to glb (unpack) or creating i3dm from glb file (pack).

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

1] Command Info i3dm_file gives header info about i3dm file

Example:

```
$ i3dm info -i test.i3dm

i3dm header version: 1
i3dm header magic: i3dm
i3dm header bytelength: 69658
i3dm header featuretablejson length: 20
i3dm header batchtablejson length: 521
Batch table json: {"hoehe":["17.386000000000024","18.34499999999997","18.58699999999999","21.860000000000014","10.168000000000006","20.584000000000003","19.70599999999996","19.817000000000007","20.000999999999976","16.577999999999975","17.865999999999985","17.745000000000005"],"citygml_class":["BB01","BB01","BB01","BB01","BB01","BB01","BB01","BB01","BB01","BB01","BB01","BB01"],"surfaceType":["roof","roof","roof","roof","roof","roof","roof","roof","roof","roof","roof","roof"],"Region":["5","5","5","5","5","5","5","5","5","5","5","5"]}
Feature table json: {"BATCH_LENGTH":12}
glTF model is loaded
```

2] Command unpack -i i3dm_filename 

unpacks a i3dm file to GLB format and creates .batch file when containing batchTableJson information

Example:

```
$ i3dm unpack -i test.i3dm

```
Action: Unpack
Input: 1.i3dm
i3dm version: 1
glTF asset generator: py3dtiles
glTF version: 2.0
Buffer bytes: 167832
Glb created: 1.glb
batch file created: 1.batch
```

3] Command pack -i glb_filename to pack a glb to i3dm file and importing batchTableJson when .batch file exists.

Example:

```
$ i3dm pack -i test.glb

Action: Pack
Input: 1.i3dm
Input batch file: 1.batch
i3dm created output.i3dm
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


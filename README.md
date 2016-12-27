<h1 style="text-align: center;">.MSET Decompiler</h1>
<h3 style="text-align: center;">Brought to you by John Cena Collective and the Plumber Task Force</h3>

Brought to you by hard work, effort, and research, this tool lets you decompile Cryptic Engine's .mset files into WaveFront OBJ (.obj) or Stanford (.ply) mesh files.

##Acknowledgements

You have now violated Perfect World's/Cryptic's Terms of Service. I will not be held responsible for any bans, warning, and other unintended repercussions resulting from thereof.

MSet (Model Set) format is not documented. I poured in virtually unlimited amount of work over the period of 8 months I have been developing this tool. Even then, this tool is highly experimental. It might work, it might not. I will not release any documentation for the format, nor will I provide support for the tool itself.

I am also not responsible for any damage caused by its correct or otherwise use.

You have been warned.

##Notes

The models are stored mirrored. This means that you will have to mirror them along one of the axes.

The resulting files will contain the following data:

* Vertices
* Normals
* Texture Coordinates
* Faces

This means that they are pretty much production-ready, so from there you can texture them, convert them to other formats, and so on.

##Requirements

This is a .NET 4.6 application, and as such, it requires a .NET implementation to be available on your system.

###Microsoft™ Windows™

If you are using Windows, make sure you are using Windows Vista or newer (and make sure you have the latest Service Pack installed). If your Windows is older than Windows 10, you might need to install .NET 4.6 manually. It is available for download [here](https://www.microsoft.com/en-us/download/details.aspx?id=48130).

###GNU/Linux

If you are using GNU/Linux, you need to make sure that Mono 4.0 or newer is available on your machine. If you are not sure how to install Mono on your GNU/Linux distribution, you may refer to [this link](http://www.mono-project.com/docs/getting-started/install/linux/).

###Apple™ Mac™ OS X

If using Mac OS X, you need to make sure that Mono 4.0 or newer is available on your machine. If you are not sure how to install Mono on your Mac OS X, you may refer to [this link](http://www.mono-project.com/docs/getting-started/install/mac/).

##Usage

This is a commandline program, as such, it needs to be invoked from a terminal emulator. Options are as follows:

    Extraction options:
      -e:<encoder>
      Specifies output encoder. <encoder> must be a valid encoder. For available encoders, see readme.
      Default value: OBJ
      
      -d:<path>
      Specifies output directory. <path> must be a valid directory path.
      Default value: <null> (outputs to source files' directory)
      
      -l
      Lists models in a file then exists.
      
      -m:<model_name>
      Specifies extracting a specific model from input file(s). <model_name> is a model to extract.
      Default value: <null> (extracts all models)
      
      -c:<color>
      Overrides vertex colors on resulting mesh. <color> is an unsigned 32-bit integer containing ARGB information.
      Default value: <null> (does not override colors)
      
      -o:<options>
      Specifies selected encoder's options. <options> is a string of comma-separated key-value pairs. For examples and available encoder options, see readme.
      Default value: <null> (use default options)

      -r
      Creates raw mesh dumps alongside output files. These are UTF-8-encoded text files describing raw input data.
      
      Program options:
      -v
      Enables verbose logging to console.
      
      -v:<path>
      Enables logging to file. Supersedes -v. <path> must be a valid file path.

To use arguments with spaces, you must wrap the entire switch in quotes, for instance:

`mset2 -v:log.txt`

`mset2 "-v:log with spaces.txt"`

###Decoder options

The encoders support various output modifying-options. Here's a full documentation:

* OBJ encoder
   * `usemtl`
      * Values: `true`/`false`
	  * Description: Saves material information to .mtl files instead of encoding it with vertices.
* PLY encoder
   * `fmt`
      * Values: `ascii`, `bin_le`
	  * Description: Determines the PLY type to use. Currently supported variants are ASCII and Little-Endian Binary.

###Microsoft™ Windows™

Invoke as:<br>
`mset2 [argument1] [argument2] [argument...]`

Alternatively, you can just drag and drop a list of files over the program's executable in Windows Explorer. The files will be converted, and the results will be placed alongside the original files.

###GNU/Linux

You need to make sure the program is executable. You do that by executing the following:<br>
`chmod +x mset2.exe`

Then, depending on your system's configuration, you will need to invoke it as:<br>
`mono mset2.exe [argument1] [argument2] [argument...]`

Or, if your distribution's Kernel supports Binary Formats, and it is configured for use with Mono, you can invoke it as:<br>
`./mset2.exe [argument1] [argument2] [argument...]`

###Apple™ Mac™ OS X

You need to make sure the program is executable. You do that by executing the following:<br>
`chmod +x mset2.exe`

Invoke as:<br>
`mono mset2.exe [argument1] [argument2] [argument...]`
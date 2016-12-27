                        MSET DECOMPILER
                          VERSION 3.0

  Brought to you by John Cena Collective and Plumber Task Force
       https://github.com/PlumberTaskForce/MSET-Decompiler

ACKNOWLEDGEMENTS
===

You have now violated Perfect World's/Cryptic's Terms of 
Service. I will not be held responsible for any bans, warning, 
and other unintended repercussions resulting from thereof.

MSet (Model Set) format is not documented. I poured in virtually 
unlimited amount of work over the period of 8 months I have 
been developing this tool. Even then, this tool is highly 
experimental. It might work, it might not. I will not release 
any documentation for the format, nor will I provide support for 
the tool itself.

I am also not responsible for any damage caused by its correct 
or otherwise use.

You have been warned.

---

NOTES
===

The models are stored mirrored. This means that you will have 
to mirror them along one of the axes.

The resulting files will contain the following data:
* Vertices
* Normals
* Texture Coordinates
* Faces

This means that they are pretty much production-ready, so from 
there you can texture them, convert them to other formats, and 
so on.

---

REQUIREMENTS
===

This is a .NET 4.6 application, and as such, it requires a .NET 
implementation to be available on your system.


MICROSOFT WINDOWS
===

If you are using Windows, make sure you are using Windows Vista 
or newer (and make sure you have the latest Service Pack 
installed). If your Windows is older than Windows 10, you might 
need to install .NET 4.6 manually. It is available for download 
here:
https://www.microsoft.com/en-us/download/details.aspx?id=48130


GNU/LINUX
===

If you are using GNU/Linux, you need to make sure that Mono 4.0 
or newer is available on your machine. If you are not sure how 
to install Mono on your GNU/Linux distribution, you may refer to 
this link:
http://www.mono-project.com/docs/getting-started/install/linux/


MAC OS X
===

If using Mac OS X, you need to make sure that Mono 4.0 or newer 
is available on your machine. If you are not sure how to 
install Mono on your Mac OS X, you may refer to this link:
http://www.mono-project.com/docs/getting-started/install/mac/

---

USAGE
===

This is a commandline program, as such, it needs to be invoked 
from a terminal emulator.

To get full parameter documentation, run it without any 
parameters.


MICROSOFT WINDOWS
===

Invoke as:
mset2 [argument1] [argument2] [argument...]

Alternatively, you can just drag and drop a list of files over 
the program's executable in Windows Explorer. The files will be 
converted, and the results will be placed alongside the 
original files.


GNU/LINUX
===

You need to make sure the program is executable. You do that by 
executing the following:
chmod +x mset2.exe

Then, depending on your system's configuration, you will need 
to invoke it as:
mono mset2.exe [argument1] [argument2] [argument...]

Or, if your distribution's Kernel supports Binary Formats, and 
it is configured for use with Mono, you can invoke it as:
./mset2.exe [argument1] [argument2] [argument...]


MAC OS X
===

You need to make sure the program is executable. You do that by 
executing the following:
chmod +x mset2.exe

Invoke as:
mono mset2.exe [argument1] [argument2] [argument...]
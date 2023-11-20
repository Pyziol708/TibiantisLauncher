# Tibiantis Launcher [![.NET Core Desktop](https://github.com/Pyziol708/TibiantisLauncher/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Pyziol708/TibiantisLauncher/actions/workflows/dotnet-desktop.yml)

Client config profile manager for Tibiantis.online Open Tibia Server

## Features

1.  client configuration profile management (including hotkeys, vip
    lists, open channels),
2.  shared map files between profiles,
3.  cam files storage in one place, shared between profiles
4.  exp/h counter
5.  time to level
6.  few quick links including map viewer showing your current location

## Will I get banned for using it?

The goal of the project is **not** and **never will** be cheating or
gaining the advantage over other players.

### How do I start with pre-compiled binary?

1.  Place the binaries and .dll's directly into the client folder and
    run *TibiantisLauncher.exe*. The game client should **not** be
    running!
2.  Create your profile by clicking *ADD PROFILE* button in the
    bottom-left corner of the window. Enter the desired profile name and
    confirm.
3.  New profile should now appear on the list. You can double-click it
    or select and click *START CLIENT*.
4.  That's it! Configure your client and join the game! Note that you
    can also edit and delete any profile by right-clicking it.

## How do I import/duplicate configuration file between profiles?

At the time of writing this tutorial, it is not possible to
import/duplicate profiles using GUI. You have to do it manually. At
first, make sure your created your profile as described above. If your
client is now running, please close it down. 1. Find your desired .cfg
file you wish to be replaced: 1. open Tibiantis client installation
folder and go to *profiles* subfolder, 1. edit *profiles.xml* file, 1.
search for your profile name in *Name* attribute of *Profile* tag, 1.
the *CfgId* attribute is the equivalent of .cfg file name in *profiles*
folder, 1. close *profiles.xml* editor. 1. Make a copy of your source
.cfg file: - the default Tibiantis config file is placed in client
installation folder: *Tibiantis/game/Tibiantis.cfg*, - you may also copy
a .cfg file of desired profile from *profiles* folder. 1. Rename the
file to match your desired .cfg and replace it.

1.  Start Tibiantis Launcher and enjoy!

## Contribution

1.  Submit an issue - you should include as much description as possible
    of the issue you are observing or feature you're suggesting,
2.  Fork the repository on GitHub
3.  Create a topic branch from where you want to base your work. Please
    avoid working directly on the *main* branch.
4.  Push your changes to a topic branch in your fork of the repository.
5.  Submit a pull request to this repository.

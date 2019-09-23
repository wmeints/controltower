# Control Tower

A .NET Core based 3D Printer controller.

## Goals

* Provide a .NET Core based solution for controlling your 3D printer
* Demonstrate how easy it is to run .NET Core on Raspberry PI
* Demonstrate how to use Blazor to build rich clientside applications

## Current status

I'm in the process of settings things up and learning how my own 3D printer communicates.
The project structure is set up, but definitely not complete!

## Project structure

There's quite a few components that come with the project. Below is an overview of everything that's included.

```
├─ ControlTower - The main application that you can use to control a 3D printer
│  ├─ ControlTower.Infrastructure - Contains the integration code for storage
│  ├─ ControlTower.Infrastructure.Tests - Tests for the infrastructure project
│  ├─ ControlTower.Core - Contains the core code for the control tower
│  ├─ ControlTower.Core.Tests - The tests for the control tower core 
│  └─ ControlTower.Web - Contains the web components for the control tower
├─ VirtualPrinter - The digital twin against which control tower is tested
│  ├─ VirtualPrinter.Tests - The tests for the virtual printer
│  └─ VirtualPrinter - The implementation of the virtual printer
└─ GCodeParser - The GCode Parser library used by the digital twin and the control tower application.
   ├─ GCodeParser.Tests - The tests for the GCode Parser
   └─ GCodeParser - The implementation for the GCode Parser
```

## Helping out

Want to chip in and write some code? Cool, create a pull request and submit issues!
I'd love to get some help with this project.

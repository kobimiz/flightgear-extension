# Flightgear extension
This app facilitates analysis of a flight. It uses Flightgear to simulate a flight
given by an input file. Given a flight, this extension can show you
- Graphs of all features given in the input file.
- Correlation between all features.
- Linear regression between correlated features.

# HOW TO USE
When you open the application, you need to go to settings and set the
locaiton of the flightgear excecutable on your computer and the location
of the simlutated flight as a CSV file. Additionaly, you need to have 
an XML file named playback_small.xml containing the data structure of
the csv file in the directory [FG-Base-Dir]/data/Protocol.
After you set the settings, **you need to restart the application**, (your
settings are saved) and click the "Launch Flightgear" button. After Flightgear is ready to go,
hit "Start simulation", and you are ready to go.

# How to download
Can be cloned from https://github.com/kobimiz/flightgear-extension

# Prerequisites
This project was built and tested on Windows 10 using .NET core (version 3.1).
You need to have flightgear installed. It can be installed from https://www.flightgear.org/.
In addition, this project uses the OxyPlot library for .NET (version 2.0).

# Structure of the project
The project contains the following folder:
- classes
- Interfaces
- ViewModels
- Views
- Windows

The project is implemented using the MVVM design pattern, so the folders ViewModels and Views are part of that.
the folder Windows contains the two main windows of the application: the main window and the settings windows.
According to the MVVM pattern, each main graphical component need to have a View component and a View Model component
(which can be seen in the project). Each view component contains a reference to the main Model component that 
defines the logic of the application. Each View Model component inherits from a main View Model class to
ease the implementation of new View Models.

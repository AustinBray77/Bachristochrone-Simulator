Known Bugs: 
 The computer running the simulation does not execute or start at a consistent frame time, meaning the time it is run for may be slightly off, possibly skewing the results and training
The unity project timescale automatically is set to 20 every time the testing is started and has to be manually set to 1 to obtain proper results. 
If the output.txt file contains a large number of data, the graph will take a long time to generate.
There were some errors with the output repeating itself as well, although this doesn’t happen 100% of the time.
Due to the AI training running, transitioning from the start menu to the AI training scene causes extreme lag, and neither the AI training nor transition can be viewed.
Occasionally part of the path may be generated inside the ball, causing 

How to use the Program (Machine Learning)
From the command line(Command Prompt, PowerShell), download ml-agents package and enable a virtual environment
Change directory to the unity project location
Run the ml-agents script and press the play button in the unity editor, click on the machine learning button to begin the training
The program will run until it is stopped, and the time taken for each successful trial will be written in the output.txt file
The best results will be written in the best.txt file, along with all points used in that test


How to use the Program (Custom Path):
Left click to place points, left click and drag to draw a path.
Once you are satisfied with your path, click the spacebar to start the simulation.
The ball will then travel along the path, once it reaches the end point or 10 seconds have passed, the simulation will reset and the path will be deleted.

How to use the Program (Data viewing):
Make sure that in the program directory there is output.txt and best.txt in the program directory, these files should contain auto generated or provided data in order to make sure it is correctly formatted
After starting the program, click Best Result to see the best path the AI was able to create.
Click Graphs to see the time the AI took to get the ball to the end over time; this shows its improvements over time.

Important Lines:
Functions.cs Line 48: Recursive sorting
Ball.cs Line 251: File IO
Ball.cs Line 33: Main List
DataPoint.cs (Entire File): Generics
Best.cs Line 12: Asynchronous Method
Functions.cs Line 7: Error Safe Function / Try Catch
DataPoint.cs Line 63: ToString Method
DataPoint.cs Line 44-59: Operator Overloading
Grapher.cs Line 80: HashSet
Grapher.cs Line 81: Linear Search

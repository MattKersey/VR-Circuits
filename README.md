# VR Circuits
*A VR application for Oculus Quest that teaches people how to build basic electonic circuits.*

#### Description
**VR Circuits** is a virtual version of a basic electronics kit. Users are given large electronic elements and have the ability to connect them with wires. Elements are meant to be responsive - the wires can be added or removed, and the switch can be thrown. If users create a circuit, they will see the wires being used change color, and the lightbulb in the circuit light up just like it would in real life. To aid exploration and learning, I created four levels using four basic elements (battery, lightbulb, resistor, and switch). Users can follow these levels to build circuits of increasing complexity and difficulty, from a simple light to a dimmable one.

#### Tools Used
- Unity 2019.4.18f1
- EchoAR
- Oculus Integration
- Blender
- Photoshop

## Setup
**Please read this as it is absolutely necessary for running the project**  
In order to deploy, you will need to build the project in Unity and deploy it to an Oculus Quest. Before doing this, you need to set up EchoAR to support the project. All necessary files are located in Assets/Models/. You will need to upload all .glb files as well as the associated metadata in the .csv files. From here, all you will need to do is add your API Key into the EchoAR game object in the main scene (Scenes/VR Circuits).

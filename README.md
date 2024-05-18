# AlienUI
Create UGUI interfaces in a WPF manner
I love WPF and hope to have a similar development experience in Unity
However, this project does not aim to replicate WPF 100%; the goal is to implement MVVM while maintaining a WPF-like development experience

The project is still in the development phase, with the basic framework largely complete
However, many basic controls are yet to be implemented
The designer also has a lot of features to be completed

# Use XML to define interface layout
In AlienUI, it's called AML, which has an AML designer
You can write XML code by hand, or generate XML code through the designer

# Support for templates
Templates can be used to define the appearance of controls, and the template itself is also an XML, allowing specified controls to use different templates upon creation

# Support for data binding
The data binding mechanism is an important foundation for implementing MVVM
Data binding has various modes: one-way, two-way, or one-time
Data binding can specify converters to support binding of different types of data
All properties that can be defined in AML are capable of data binding

# Support for triggers
With triggers, you can achieve rich dynamic interface changes without writing code

# Storyboard
A keyframe-based animation system that can control the values of all dependency properties

# Event system
The event system is the UGUI event system, but with some upper-level encapsulation, allowing events to be conveniently propagated upwards.
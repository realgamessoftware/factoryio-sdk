FACTORY I/O (SDK)
=================

FACTORY I/O SDK (Software Development Kit) provides tools so that developers can create custom applications that access the simulation I/O points. These applications can be used as interfaces between FACTORY I/O and virtually any type of technologies, such as PLC, microcontrollers, databases, spreadsheets and so on.

All of the simulation I/O points can be accessed through the SDK independently of the selected driver. Select None from the I/O drivers drop-down list when accessing the I/O points only through the SDK. Please note that Ultimate Edition is required in order to use the SDK.

Engine I/O
----------

Engine I/O is a .NET Standard 2.0 assembly for inter-process communication (IPC) between FACTORY I/O and custom applications. It gives access to the simulation I/O points through a memory mapped file. The memory is divided in three groups: Inputs, Outputs and Memories. Standing from a controller point of view, Inputs are used to read sensors values, Outputs to write actuators and Memories to exchange generic data. An I/O point is made of a memory address, name and value representing a specific data type.

Some considerations to take into account when using Engine I/O:

* The MemoryMap class is a singleton, meaning that only one instance can exist. This instance represents a cached copy of the memory mapped file.
* The MemoryMap.Instance.Update() method is responsible for synchronizing cached data with the actual memory. This method must be called every time there is the need to read/write the latest I/O points or receive event notifications.

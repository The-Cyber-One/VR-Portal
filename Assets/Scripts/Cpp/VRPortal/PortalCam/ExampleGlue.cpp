#include "pch.h"
#include "ExampleGlue.h"

// Link all the C# functions so that they can be called from here
int (*Log)(const char[]);
int (*CsharpFunction)(const int);

void Init(
	int (*logPointer)(const char[]),
	int (*csharpFunctionPointer)(const int))
{
	Log = logPointer;
	CsharpFunction = csharpFunctionPointer;
}

// Use the normal Start and Update functions because they will be called from the glue script
void Start()
{
	// Will be called once when the gameobject becomes active, just as normal
	Log("hey");
}

void Update()
{
	// Will be called every frame, just as normal
}

void FixedUpdate() 
{
	// Will be called every fixed update, just as normal
}
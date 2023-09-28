#include "pch.h"
#include <string>
using namespace std;
#include "PortalCam.h"

// Link all the C# functions so that they can be called from here
int (*GameObjectGetTransform)(int thisHandle);
int (*Log)(string);

void Init(
	int (*gameObjectGetTransformPointer)(int),
	int (*logPointer)(string))
{
	GameObjectGetTransform = gameObjectGetTransformPointer;
	Log = logPointer;
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
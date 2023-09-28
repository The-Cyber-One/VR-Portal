#pragma once
#define DLLEXPORT __declspec(dllexport) // Needed export macro for windows
#include <string>
using namespace std;

extern "C" // Make sure the naming doesn't change so that linking doesn't break
{
	DLLEXPORT void Init(
		int (*gameObjectGetTransformPointer)(int),
		int (*logPointer)(string));

	DLLEXPORT void Start();
	DLLEXPORT void Update();
}
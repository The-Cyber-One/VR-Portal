#pragma once
#define DLLEXPORT __declspec(dllexport) // Needed export macro for windows

extern "C" // Make sure the naming doesn't change so that linking doesn't break
{
	DLLEXPORT void Init(
		int (*logPointer)(const char[]),
		int (*csharpFunctionPointer)(const int));

	DLLEXPORT void Start();
	DLLEXPORT void Update();
	DLLEXPORT void FixedUpdate();
}
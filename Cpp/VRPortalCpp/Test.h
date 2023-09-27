#pragma once

#ifdef VRPORTALCPP_EXPORTS
#define TEST_API __declspec(dllexport)
#else
#define TEST_API __declspec(dllimport)
#endif

extern "C" TEST_API int Add(const int a, const int b);
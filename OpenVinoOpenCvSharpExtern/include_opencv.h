#ifndef _INCLUDE_OPENCV_H_
#define _INCLUDE_OPENCV_H_

//#define ENABLED_CONTRIB
//#undef ENABLED_CONTRIB

#ifdef _MSC_VER
#define NOMINMAX
#define _CRT_SECURE_NO_WARNINGS
#pragma warning(push)
#pragma warning(disable: 4251)
#pragma warning(disable: 4996)
#endif


#include <opencv2/opencv.hpp>
#include <opencv2/core/core_c.h>


#include <vector>
#include <algorithm>
#include <iterator>
#include <sstream>
#include <iterator>
#include <fstream>
#include <iostream>
#include <cstdio>
#include <cstring>
#include <cstdlib>
#ifdef _MSC_VER
#pragma warning(pop)
#endif
// Additional types
#include "my_types.h"

// Additional functions
#include "my_functions.h"

#endif

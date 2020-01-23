find_package(OpenCV REQUIRED)
message(STATUS "OpenCV library status:")
message(STATUS "    version: ${OpenCV_VERSION}")
message(STATUS "    libraries: ${OpenCV_LIBS}")
message(STATUS "    include path: ${OpenCV_INCLUDE_DIRS}")

file(GLOB OPENVINOOPENCVSHARP_FILES *.cpp)
list(FILTER OPENVINOOPENCVSHARP_FILES EXCLUDE REGEX "^.*/(aruco|bgsegm|calib3d|face|img_hash|optflow|photo|quality|shape|superres|text|tracking|xfeatures2d|ximgproc|xphoto).*.cpp$")

message(STATUS " files: ${OPENVINOOPENCVSHARP_FILES}")

find_package(OpenCV REQUIRED)
find_package(InferenceEngine REQUIRED)

add_library(OpenVinoOpenCvSharpExtern SHARED ${OPENVINOOPENCVSHARP_FILES})
target_link_libraries(OpenVinoOpenCvSharpExtern ${OpenCV_LIBS} ${InferenceEngine_LIBRARIES})

install(TARGETS OpenVinoOpenCvSharpExtern
    RUNTIME DESTINATION bin
    LIBRARY DESTINATION lib
    ARCHIVE DESTINATION lib/static
)

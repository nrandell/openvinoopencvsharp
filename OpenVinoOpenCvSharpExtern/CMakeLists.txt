find_package(OpenCV REQUIRED)
message(STATUS "OpenCV library status:")
message(STATUS "    version: ${OpenCV_VERSION}")
message(STATUS "    libraries: ${OpenCV_LIBS}")
message(STATUS "    include path: ${OpenCV_INCLUDE_DIRS}")

file(GLOB OPENVINOOPENCVSHARP_FILES *.cpp)

find_package(OpenCV REQUIRED)

add_library(OpenVinoOpenCvSharpExtern SHARED ${OPENVINOOPENCVSHARP_FILES})
target_link_libraries(OpenVinoOpenCvSharpExtern ${OpenCV_LIBS})

install(TARGETS OpenVinoOpenCvSharpExtern
    RUNTIME DESTINATION bin
    LIBRARY DESTINATION lib
    ARCHIVE DESTINATION lib/static
)

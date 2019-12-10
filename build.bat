call "C:\Program Files (x86)\IntelSWTools\openvino\bin\setupvars.bat"

PATH="C:\Program Files\CMake\bin\";%PATH%

rmdir /s/q build
mkdir build
cd build

cmake ^
        -D CMAKE_INSTALL_PREFIX=..\install ^
        -D CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS=TRUE ^
        ..

cmake --build . --target install --config Release
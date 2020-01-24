$Env:HDDL_INSTALL_DIR = "C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\inference_engine\external\hddl"
$Env:InferenceEngine_DIR = "C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\inference_engine\share"
$Env:INTEL_CVSDK_DIR = "C:\Program Files (x86)\IntelSWTools\openvino"
$Env:INTEL_DEV_REDIST = "C:\Program Files (x86)\Common Files\Intel\Shared Libraries\"
$Env:INTEL_LICENSE_FILE = "C:\Program Files (x86)\Common Files\Intel\Licenses"
$Env:INTEL_OPENVINO_DIR = "C:\Program Files (x86)\IntelSWTools\openvino"
$Env:MIC_LD_LIBRARY_PATH = "C:\Program Files (x86)\Common Files\Intel\Shared Libraries\compiler\lib\intel64_win_mic"
$Env:OpenCV_DIR = "C:\Program Files (x86)\IntelSWTools\openvino\opencv\cmake"
$Env:OPENVX_FOLDER = "C:\Program Files (x86)\IntelSWTools\openvino\openvx"
$Env:ROOT = "C:\Program Files (x86)\IntelSWTools\openvino"

$Env:Path += "C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\ngraph\lib"
$Env:Path += "C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\inference_engine\bin\intel64\Release"
$Env:Path += ";C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\inference_engine\bin\intel64\Debug"
$Env:Path += "C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\inference_engine\external\hddl\bin"
$Env:Path += "C:\Program Files (x86)\IntelSWTools\openvino\openvx\bin"
$Env:Path += "C:\Program Files (x86)\IntelSWTools\openvino\opencv\bin"
$Env:Path += "C:\Program Files (x86)\Common Files\Intel\Shared Libraries\redist\intel64_win\compiler"
$Env:Path += "C:\Program Files\CMake\bin\"

. ..\..\variables.ps1

Remove-Item -Recurse .\build -ErrorAction Ignore
New-Item -ItemType Directory .\build

Push-Location .\build

try {
        Invoke-WebRequest -Uri https://github.com/shimat/opencvsharp/archive/${OPENCVSHARP_VERSION}.zip -OutFile opencvsharp.zip
        Expand-Archive -Path opencvsharp.zip -DestinationPath .
        Move-Item -Path opencvsharp-${OPENCVSHARP_VERSION} opencvsharp
        Copy-Item -Path ..\..\CMakeLists.txt,..\..\include_opencv.h -Destination opencvsharp\src\OpenCvSharpExtern\
        New-Item -ItemType Directory .\build
        Set-Location .\build

        cmake `
                -D CMAKE_INSTALL_PREFIX=..\install `
                -D CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS=TRUE `
                ..\opencvsharp\src

        cmake --build . --config Release
        cmake --build . --target install --config Release
        Copy-Item ..\install\bin\OpenVinoOpenCvSharpExtern.dll ..\..\..\..\openvinocvsharp\
}

finally {
        Pop-Location
}


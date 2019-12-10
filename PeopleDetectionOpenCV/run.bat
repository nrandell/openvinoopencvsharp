@echo off
set PATH=C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\inference_engine\bin\intel64\Release;C:\Program Files (x86)\IntelSWTools\openvino\opencv\bin;%PATH%
dotnet run --no-build dshow "video=Razer Kiyo" "c:\users\nick\downloads\models\opencv_face_Detector_uint8.pb" "c:\temp\pddumps"


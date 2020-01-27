#! /bin/bash

set -evx

pushd debian-buster-arm32v7-opencv-openvino; ./build.sh; popd
pushd debian-buster-arm64v8-opencv-dldt; ./build.sh; popd
pushd ubuntu-bionic-x64-opencv-openvino; ./build.sh; popd

pushd openvino-dotnet-core-runtime; ./build.sh; popd

pushd openvino-aspnet-arm32; ./build.sh; popd
pushd openvino-aspnet-arm64; ./build.sh; popd
pushd openvino-aspnet-x64; ./build.sh; popd

pushd openvino-aspnet; ./build.sh; popd


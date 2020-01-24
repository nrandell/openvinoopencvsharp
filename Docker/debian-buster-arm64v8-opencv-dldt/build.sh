#! /bin/bash

. ../variables.sh


docker buildx build --platform linux/arm64 -t "nrandell/openvino-dotnet-core-runtime:$ARM64_VERSION" -f Dockerfile --push .



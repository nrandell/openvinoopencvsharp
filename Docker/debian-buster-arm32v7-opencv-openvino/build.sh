#! /bin/bash

# docker buildx build --platform linux/arm64 -t "nrandell/xxx" -f Dockerfile --load .
# exit 0
VERSION="3.1.1-2019.3.334-2"
docker buildx build --platform linux/arm/v7 -t "nrandell/openvino-dotnet-core-runtime:$VERSION" -f Dockerfile --push .



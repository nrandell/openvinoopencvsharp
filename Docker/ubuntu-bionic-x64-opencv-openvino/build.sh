#! /bin/bash

. ../variables.sh

docker buildx build --platform linux/arm64 -t "${BASE_IMAGE}:${X64_VERSION}" -f Dockerfile --push .



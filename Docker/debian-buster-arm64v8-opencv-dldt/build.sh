#! /bin/bash

. ../variables.sh


docker buildx build --platform linux/arm64 -t "${BASE_IMAGE}:${ARM64_VERSION}" -f Dockerfile --push .



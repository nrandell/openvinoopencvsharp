#! /bin/bash

. ../variables.sh

docker buildx build --platform linux/amd64 -t "${BASE_IMAGE}:${X64_VERSION}" -f Dockerfile --push .



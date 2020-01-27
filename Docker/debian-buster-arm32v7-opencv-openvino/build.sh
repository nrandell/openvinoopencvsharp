#! /bin/bash

. ../variables.sh

docker buildx build --platform linux/arm/v7 -t "${BASE_IMAGE}:${ARM32_VERSION}" -f Dockerfile --push .



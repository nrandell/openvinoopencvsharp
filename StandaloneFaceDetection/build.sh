#! /bin/bash


set -evx

. ../Docker/variables.sh

OUTPUT_VERSION="1.14"

docker buildx build --platform linux/arm64,linux/arm/v7,linux/amd64 --build-arg IMAGE_VERSION=${VERSION} -t "nrandell/standalonefacedetection:${OUTPUT_VERSION}" -f Dockerfile --push ..

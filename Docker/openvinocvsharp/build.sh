#! /bin/bash

. ../variables.sh

docker build -t "nrandell/openvinocvsharp:build" --build-arg OPENCVSHARPEXTERN_VERSION=${OPENCVSHARPEXTERN_VERSION} --build-arg OPENCVSHARP_VERSION=${OPENCVSHARP_VERSION} --build-arg NUGET_API_KEY=$NUGET_API_KEY -f Dockerfile .



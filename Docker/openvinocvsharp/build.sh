#! /bin/bash

docker build -t "nrandell/openvinocvsharp:build" --build-arg NUGET_API_KEY=$NUGET_API_KEY -f Dockerfile .



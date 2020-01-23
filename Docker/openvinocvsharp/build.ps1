
docker buildx build -t "nrandell/openvinocvsharp:build" --build-arg NUGET_API_KEY=$env:NUGET_API_KEY --push -f Dockerfile .



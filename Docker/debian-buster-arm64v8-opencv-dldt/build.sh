VERSION="3.1.1_4.2.0_2019_R3.1-1"
docker buildx build --platform linux/arm64 -t "nrandell/openvino-dotnet-core-runtime:$VERSION" -f Dockerfile --push .



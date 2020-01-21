VERSION="1.0.0"

docker buildx build --platform linux/arm64 -t "nrandell/xxx:$VERSION" -f Dockerfile --load .



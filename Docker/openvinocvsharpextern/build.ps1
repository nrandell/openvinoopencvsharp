
docker buildx build --platform linux/amd64 -t "nrandell/openvinocvsharpextern:x64-3.1.1-2019.3.376-2" -f Dockerfile.ubuntu-bionic-x64 --push .

docker buildx build --platform linux/arm64 -t "nrandell/openvinocvsharpextern:arm64-3.1.1-2019.3.376-2" -f Dockerfile.debian-buster-arm64 --push .


$VERSION="1.0.4"

docker build -t "nrandell/nick-builder:$VERSION" -f Dockerfile ../..
docker push "nrandell/nick-builder:$VERSION"



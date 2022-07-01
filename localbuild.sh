
rm -rf out
dotnet publish -c Release -o out

docker build -t votemap:latest .
docker stop votemap
docker rm votemap
docker run -it -d --name votemap -p 50080:80 -e "TZ=Asia/Taipei" -e "ASPNETCORE_ENVIRONMENT=production" votemap:latest
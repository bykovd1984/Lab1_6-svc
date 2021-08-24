docker build . -f .\Lab1_6\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-web:1.4 -t bykovd1984/softwarearchitect-lab1.6-web:latest

docker push bykovd1984/softwarearchitect-lab1.6-web:1.4

docker build . -f .\Lab1_6.Data.Migrations\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-migrations:1.8

http://127.0.0.1:56813/?url=http://echoserver
docker build . -f .\Lab1_6\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-web:1.5
docker push bykovd1984/softwarearchitect-lab1.6-web:1.6

docker build . -f .\Lab1_6.OrderSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-ordersvc:1.0
docker push bykovd1984/softwarearchitect-lab1.6-ordersvc:1.0

docker build . -f .\Lab1_6.BillingSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-billingsvc:1.0
docker push bykovd1984/softwarearchitect-lab1.6-billingsvc:1.0

docker build . -f .\Lab1_6.NotifierSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-notifiersvc:1.0
docker push bykovd1984/softwarearchitect-lab1.6-notifiersvc:1.0

docker build . -f .\Lab1_6.Data.Migrations\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-migrations:1.9

http://127.0.0.1:56813/?url=http://echoserver
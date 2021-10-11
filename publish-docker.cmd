docker build . -f .\Lab1_6\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-web:1.8
docker push bykovd1984/softwarearchitect-lab1.6-web:1.8

docker build . -f .\IdentityServerAspNetIdentity\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-auth:1.4
docker push bykovd1984/softwarearchitect-lab1.6-auth:1.4

docker build . -f .\Lab1_6.OrderSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-ordersvc:1.2
docker push bykovd1984/softwarearchitect-lab1.6-ordersvc:1.2

docker build . -f .\Lab1_6.BillingSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-billingsvc:1.2
docker push bykovd1984/softwarearchitect-lab1.6-billingsvc:1.2

docker build . -f .\Lab1_6.NotifierSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-notifiersvc:1.2
docker push bykovd1984/softwarearchitect-lab1.6-notifiersvc:1.2

docker build . -f .\Lab1_6.DeliverySvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-deliverysvc:1.2
docker push bykovd1984/softwarearchitect-lab1.6-deliverysvc:1.2

docker build . -f .\Lab1_6.WarehouseSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-warehousesvc:1.2
docker push bykovd1984/softwarearchitect-lab1.6-warehousesvc:1.2

docker build . -f .\Lab1_6.Data.Migrations\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-migrations:1.10
docker push bykovd1984/softwarearchitect-lab1.6-migrations:1.10

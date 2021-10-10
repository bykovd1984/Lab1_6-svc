docker build . -f .\Lab1_6\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-web:1.7
docker push bykovd1984/softwarearchitect-lab1.6-web:1.7

docker build . -f .\Lab1_6.OrderSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-ordersvc:1.1
docker push bykovd1984/softwarearchitect-lab1.6-ordersvc:1.1

docker build . -f .\Lab1_6.BillingSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-billingsvc:1.1
docker push bykovd1984/softwarearchitect-lab1.6-billingsvc:1.1

docker build . -f .\Lab1_6.NotifierSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-notifiersvc:1.1
docker push bykovd1984/softwarearchitect-lab1.6-notifiersvc:1.1

docker build . -f .\Lab1_6.DeliverySvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-deliverysvc:1.1
docker push bykovd1984/softwarearchitect-lab1.6-deliverysvc:1.1

docker build . -f .\Lab1_6.WarehouseSvc\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-warehousesvc:1.1
docker push bykovd1984/softwarearchitect-lab1.6-warehousesvc:1.1

docker build . -f .\Lab1_6.Data.Migrations\Dockerfile -t bykovd1984/softwarearchitect-lab1.6-migrations:1.10
docker push bykovd1984/softwarearchitect-lab1.6-migrations:1.10

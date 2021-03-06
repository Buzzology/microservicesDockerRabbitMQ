# Tutorial followed to learn Microservices with Docker, .NET Core and RabbitMQ
I followed this tutorial to help me learn the basics of Microservices with RabbitMQ, .NET Core and Docker. I then went through adding a number of the features included in the eShopContainers repository to get the hang of how they're all implemented.

## Tutorial and project setup  
Tutorial: https://medium.com/trimble-maps-engineering-blog/getting-started-with-net-core-docker-and-rabbitmq-part-1-a62601e784bb  
GitHub: https://github.com/matthew-harper/dotnet-docker-rabbitmq-tutorial 


## Additional custom enhancements  
I've added a few minor changes of my own in order to test out other components that I'd seen Microsoft's eShopContainer repo.
- Upgraded to .NET Core 3.0
- Implemented Azure Key Vault
- Swagger UI using Swashbuckle
- Implemented gRPC
- Implemented Serilog and Seq
- Webhooks
- Health checks
- Retry using Polly
- GraphQL



### Commands
- Create new project: dotnet new webApi -o "publisher_api"
- Create new solution: dotnet new sln
- Add project to solution: dotnet sln add publsiher_api/publisher_api.csproj
- Run project: dotnet run
- Add a package: dotnet add package Newtonsoft.Json


## Docker setup  
Tutorial: https://medium.com/trimble-maps-engineering-blog/getting-started-with-net-core-docker-and-rabbitmq-part-2-e41a0961292a  
GitHub: https://github.com/matthew-harper/dotnet-docker-rabbitmq-tutorial  

Dockerfile is used in each microservice to define invidual build settings.  
Docker-compose "orchestration" for containers. It allows you to run multi-container applications using docker-compose.yml in the root directory.

### Commands
- Build: docker build -t my_publisher_api .
- Run single container: docker run my_publisher_api
- Run multi-container: docker-compose up --build
- View list of containers: docker container ls
- Inspect container details: docker inspect CONTAINER_ID
- Stop a single microservice after starting with docker-compose: docker stop {instance-id}
- View running containers: docker ps
- Stop all running containers (powershell): docker stop $(docker ps -a -q)
- Remove all containers (powershell): docker rm $(docker ps -a -q)

### Additional info
A default network is created for our application when using docker-compose to launch it. Each container is reachable by other containers and discoverable via a hostname identicaly to the container name. e.g. http://localhost:5001 becomes http://publisher_api:80.  

Dockerfile defines the image to create, docker compose defines the container.


## RabbitMQ setup
Tutorial: https://medium.com/trimble-maps-engineering-blog/  getting-started-with-net-core-docker-and-rabbitmq-part-3-66305dc50ccf  
GitHub: https://github.com/matthew-harper/dotnet-docker-rabbitmq-tutorial  

RabbitMQ is used for queueing service messages and decoupling them. Added as a new container to docker-compose. 

### Commands  
- Add client to api: dotnet add package RabbitMQ.Client  

### Additional info  
View the RabbitMQ managment console by browsing to http://localhost:15672


## Swagger setup  
GitHub: https://github.com/domaindrivendev/Swashbuckle.AspNetCore  

### Additional info  
JSON feed: http://localhost/swagger/v1/swagger.json  
UI: http://localhost/swagger/index.html  


## gRPC Setup  
Tutorial: https://docs.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-3.1&tabs=visual-studio-code  
GitHub Reference: https://github.com/aspnet/AspNetCore.Docs/tree/master/aspnetcore/tutorials/grpc/grpc-start/sample  


## Serilog with Seq setup
Github: https://github.com/serilog/serilog-aspnetcore
Docker setup for Seq: https://github.com/dotnet-architecture/eShopOnContainers/wiki/Serilog-and-Seq

### Additional info
UI: http://localhost:5340/#/events
Internal Logging Target: http://seq:5341


# Webhooks  
Tutorial: https://medium.com/@nelson.souza/net-core-webhooks-7a51e113f9f6
ngrok: https://dashboard.ngrok.com/get-started

App received a notification every time a change is made to the repository on Github. Requires ngrok to be running if hosting locally.


# Healthchecks
Github: https://github.com/dotnet-architecture/eShopOnContainers/wiki/Using-HealthChecks
Doco: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health

Used to check liveness and health of services. If using in a real project would probably create as a separate microservice or use the provided docker file. The health checks ui observes each of the provided endpoints and provides a spa to display the result.

## Additional info
UI: http://localhost/healthchecks-ui
Endpoint to check: http://localhost/hc


# Polly
Background Info (including service meshes): https://github.com/dotnet-architecture/eShopOnContainers/wiki/Resiliency-and-Service-Mesh
Doco: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly

Used to maximise availability and handle scenarios where retry logic may be required.


# GraphQL
Tutorial: https://hotchocolate.io/docs/tutorial-01-gettingstarted
How to set it all up with ASP.NET Core: https://hotchocolate.io/docs/aspnet

I'd heard that the RSPCA is using Hot Chocolate on Microservices with AWS for their newer projects so I'm trying it out.

Added seq, added mongodb, added to docker-compose. 


## Additional info
Playground endpoint: http://localhost:5000/playground/
Running locally: dotnet watch --project src/Server/ run
Run mongodb locally: docker run --name mongo -p 27017:27017 -d mongo mongod

Twitter Service Initial Tutorial: https://hotchocolate.io/docs/
Data Loader Tutorial: https://hotchocolate.io/docs/dataloaders
Added graphql logger: https://chillicream.com/blog/2019/03/19/logging-with-hotchocolate
Linked logger to SEQ sink.

## Sample mutation
mutation {
  createUser(userInput:{
    country: "Australia"
    name: "tesT"
  }) {
    id
  }
}










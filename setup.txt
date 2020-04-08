1) Pull Postres
    docker pull postgres

2) Create a volume to run the database on
    docker volume create <VolumeName>

3) locate MountPoint
    docker inspect <VolumeName>

4) Run postgres on the above mentioned volume
    docker run -p 5432:5432 --name postgresql -e POSTGRES_PASSWORD=password -d -v 'postgresql-volume:<Mountpoint>' postgres:12

5) Pull pgAdmin
    docker pull dpage/pgadmin4

    docker run -p 8080:80 --name pgAdmin -e "PGADMIN_DEFAULT_EMAIL=<EmailAddress>" -e "PGADMIN_DEFAULT_PASSWORD=password" -d dpage/pgadmin4

6) Locate ip address of postgres, it will be listed under Networks towards the bottom. This IP will be used to configure pgAdmin
    docker inspect postgresql

7) Launch pgAdmin, url shoule be
    http://localhost:8080

    - Log in with the credentials specified above

    - configure a new server connection, using the IP address of postgres, using postgres:password as the default username:password

8) Build the DockerWeb images from the docker-compose
     - navigate to the folder where docker-compose exist

    docker-compose build

    docker-compose up -d

    - this compose will create the dockerweb and dockerwebapi.  it will create a network which will bind these and the database to.

    - verify the network exist: dockerweb_dockerweb-net
    docker network ls

    - verify the containers exist for: dockerweb and dockerwebdbservice
    docker container ps -a

    - inspect the network to get the ip address for the postgres instance. you will use this IP to configure pgAdmin as you did prior
    docker inspect dockerweb_dockerweb-net

9) This project contains a database migration, which needs to be executed
    - from bash navigate to the project root, and run:

    dotnet ef database update

    - log into pgAdmin and verify the database has been created

10) there is no gui interface, yet...  for now use swagger to verify everything is connecting as it should

    - dockerWeb:
        http://localhost:5003/swagger
        http://localhost:5003/weatherforecast

    - dockerWebService:
        http://localhost:5002/swagger
        http://localhost:5002/api/weatherforecast






    

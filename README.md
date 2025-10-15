# paperlessdmc
Semester Project: Document Management Systema Document management system for archiving documents in a FileStore, with automatic OCR (queue for OC-recognition), automatic summary generation (using Gen-AI), tagging and full text search (ElasticSearch).

The technology stack is: 

PostgresSQL for Data Storage. 
ASP.net Backend for REST API.
Angular for the user frontend.
Rabbit MQ is for the queues for intercomponent communciation.   


In order to run the contaners run "docker compose up --build" in the director paperlessdmc directory.  

The running containers are:

- The "file_management_gui" for nginx webserver and angular app.
- The "web-1" is the ASP.net Backend handling the API requests. 
- "pgadmin-1" is the database control application  
- "postgres-1" is the actual database 
- "rabbitmq-1" is for the 


Currently if there are changes to the user interface the nginx image has to be rebuilt manually change to paperlessdmc/PresentationLayer/file-managemant-gui directory and execute docker:
build . -t nginx3:nginx3


Use "docker build . -t nginx3:nginx3" in PresentationLayer/file-managment-gui to build docker image 

node_modules directory removed due to size make sure it installed when running program
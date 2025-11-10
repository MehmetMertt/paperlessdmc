docker compose down -v
docker build ./PresentationLayer/file-managment-gui -t nginx3:nginx3 
docker compose up --build -d
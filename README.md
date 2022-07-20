# Subscriber
 

# Description:
This app uses the data from Facebook Marketplace. You can select a topic to subscribe and monitor the market place.
When there are new listings available you will get an SMS/Email right away, that way you will be first to contact seller.


# Technologies used 

- [x] Angular 12 for the frontend
- [x] Twilio/SandGrid
- [x] Kendo front end library
- [x] Pupoteer Sharp for scraping data
- [x] FIrebase Database

In this project I used the Provider Pattern along with the built in .net5 dependency injection.

![alt text](https://github.com/Stanmozolevskiy/Subscriber/blob/main/AplicationLogic.jpg)

<br>
<br>

![alt text](https://github.com/Stanmozolevskiy/Subscriber/blob/main/Subscriber_Arcitecture.jpg)

# Deploy to Heroku with Docker

Runt the following commands in the terminal with **adnim rights** in the root folder

- **DOCKER**
- ```docker build -t [projectname] .```
- ```docker run -d -p 5000:80 --name myapp [projectname]```
- ```heroku container:login```
- ```docker build -t registry.heroku.com/[heroku-app-mane]/web .```
- ```docker push registry.heroku.com/[heroku-app-mane]/web```
- ```heroku container:release web --app [heroku-app-mane]```

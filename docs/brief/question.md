A.
You have been hired by ArgiStream IoT: A company that provides smart monitoring solutions for large scale commercials farms. the platform receives massive surges of data from millions of soil sensors daily. Alongside sudden burst of high resolution aerial imagery auploaded by field drones.  You need to build a robust system that can relianly process these dadta streams during peak daylight operational hours without losing data.

You are requireed to build a management web application (MVC) and implement a scalable realtime data processing system using azure services. To achieve this you must complete the following.

a) Web hosting : create an Azure App service to host the main MVC web application use this format http://studentnumber.azurewebsites.net

b) you have to create a relational database to manage user account. it should be simple to store login credentials (username, passsword) and assigned roles ( farmer, Agronomist and Admin). Provide evidence of the Azure hosting and successful connection to MVC project.

c) Cloud Storable (Data and Media) 
You have to use both unstructured and NoSQL storage solutions within Azure storage
	- Aure Blob storage: use this to store drome images or other farms-related media uploaded via the web application
	- Azure table storage: to store high frequency, non relational telemetry data such as real time sensor readings, irrigation eventsor environmental monitoring logs

d) Serverless Pipeline: Build a scalable data processing pipeline using only 4 azure functions to handle different aspects of the system Your implementation should include :


	- Blob Triggered Function; when a drone image is uploaded to Blob storage this function is triggered to log the upload details and write a processing status audit directly to axure table storage 
	- HTTP_TrigeredFucntion (Get): when a farmer opnes their field dashboard the MVC app calls this function to retrive the latest telemetry data from azure table storage near instant rendering
 	-HTTP_TrigeredFucntion (Post):when an automated sensor detects a problem or a user clicks “Trigger irrigation” in the app the MVC app calls this function to place an irrigation command message into azure storage table
	- Queue triggered Function: this function listens to the azure storage Queue processes the irrigation command and updates the devices’s operational status in the master registery table in Azure sql databbase


e) Load testing: simulate heavy traffic loads to test the scallability of your sstem. use a tool like Azure Load testing, apacle Jmeter, or a simple automated script to simulatre multiple concurrent users accessing your app service

f) Monitoring: monitor the performances and resources of your azure services during the simulated peak load. use azure monitor or application insight to capture and analyse the system response wether it met the expected scalability and reliability requirements
g) Auto Scaling: provide auto scaling tules for your azure app services based on the CPU or memory utilisaton trreshold. provide screenshots shwoing how yuour app successfully scled to handle the traffic generated the load test in Task(e)




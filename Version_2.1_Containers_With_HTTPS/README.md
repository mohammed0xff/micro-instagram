
## [How to run](https://github.com/mohammed0xff/micro-instagram/blob/master/Version_2.1_Containers_With_HTTPS/README-How-To-Run.md)

# Introduction

### What did we add to this version?

In this version we configured https to enhance the security of our containers.

## Why HTTPS? 

I mean HTTP is fine but:

 * HTTPS encrypts the data exchanged between clients and servers, 
 ensuring that sensitive information remains confidential. With HTTP, data is transmitted in plain text,
 making it vulnerable to interception and unauthorized access. 
 HTTPS protects against eavesdropping and data tampering by encrypting the communication channel.

* HTTPS relies on digital certificates issued by trusted certificate 
authorities. These certificates validate the identity of the server, assuring clients that 
they are communicating with the intended service. By using HTTPS, we establish trust and 
provide authentication, reducing the risk of impersonation or man-in-the-middle attacks.

* And in a microservices architecture, different services often need to communicate 
with each other securely. By standardizing on HTTPS, we ensure interoperability and seamless 
integration between microservices. HTTPS allows services to communicate securely across networks 
and facilitates secure.


There are many other reasons why to use https over http check this [link](https://www.guru99.com/difference-http-vs-https.html) to learn more.

## How to HTTPS? 


1. Generate and trust an HTTPS certificate, run this command:

```shell
dotnet dev-certs https -ep "$HOME/.aspnet/https/cert.pfx" -p cert-password --trust 
```
* `-p` for passowrd
* `-ep` export path for the generated certificate
* `--turst` By default, self-signed certificates are not trusted. this option indicates that the generated certificate should be trusted

2. Edit `docker-compose.override.yml` file to add Certificates environment variables :

* `ASPNETCORE_Kestrel__Certificates__Default__Password` 
* `ASPNETCORE_Kestrel__Certificates__Default__Path` 
That we just used to generate our certificate.


```docker
version: '3.9'

services:

  notification_service:
    environment:
      - ASPNETCORE_ENVIRONMENT=Docker
      - ASPNETCORE_URLS=https://+:7001;http://+:7000
      - ASPNETCORE_HTTPS_PORT=7001
      - ASPNETCORE_Kestrel__Certificates__Default__Password=cert-password
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
    volumes:
      - ~/.aspnet/https:/https:ro

  user_service:
    environment:
      - ASPNETCORE_ENVIRONMENT=Docker
      - ASPNETCORE_URLS=https://+:8001;http://+:8000
      - ASPNETCORE_HTTPS_PORT=8001
      - ASPNETCORE_Kestrel__Certificates__Default__Password=cert-password
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
    volumes:
      - ~/.aspnet/https:/https:ro
```

## Is that it?
Yes, so simple right? But why another version only for https?
Cause i want everything to be as clear as possible. aslo this didn't seem as clear and intuitive first time i encountered.

## Also see this 

[![](https://img.youtube.com/vi/T4Df5_cojAs/0.jpg)](https://www.youtube.com/watch?v=T4Df5_cojAs)



## What's next 
k8s i guess, let's do this! 😃

